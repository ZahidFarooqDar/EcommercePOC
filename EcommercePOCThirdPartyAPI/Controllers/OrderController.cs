using AutoMapper;
using Azure;
using EcommercePOCThirdPartyAPI.Data;
using EcommercePOCThirdPartyAPI.DomainModals;
using EcommercePOCThirdPartyAPI.Helpers;
using EcommercePOCThirdPartyAPI.TrackerResponseTR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EcommercePOCThirdPartyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ProjectEcommerceContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        // API key for Ship24 (replace with your actual API key)
        private const string Ship24ApiKey = "apik_EvLtppUyD7XmmgfwF7wuyLCiscjyw1";

        public OrderController(IHttpClientFactory httpClientFactory, ProjectEcommerceContext projectEcommerceContext, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _context = projectEcommerceContext;
            _mapper = mapper;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyProduct([FromBody] BuyProductRequest buyProductRequest)
        {
            try
            {
                // Validate the request, authenticate the buyer, and calculate the total price
                var product = await _context.Products.FindAsync(buyProductRequest.ProductId);
                if (product == null)
                    return NotFound("Product not found");

                // Check if the product is available in sufficient quantity
                if (product.Quantity < buyProductRequest.Quantity)
                    return BadRequest("Insufficient quantity available");

                // Create a new order with a unique Tracker ID
                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = buyProductRequest.ProductId,
                    Quantity = buyProductRequest.Quantity,
                    TotalPrice = product.Price * buyProductRequest.Quantity,
                    BuyerId = buyProductRequest.BuyerId,
                    SellerId = product.SellerId, // Assuming product has a SellerId property
                    TrackerId = Guid.NewGuid().ToString() // Generate a unique Tracker ID for each order
                };

                // Save the order details in your database
                _context.Orders.Add(order);

                // Update the product quantity after purchase
                product.Quantity -= buyProductRequest.Quantity;

                await _context.SaveChangesAsync();

                // Map the order to an OrderDto using AutoMapper
                var orderDto = _mapper.Map<OrderDto>(order);

                // Make a request to the Ship24 API to create a new tracker for this order
                string trackingNumber = "S24DEMO" + GenerateRandomDigits(6);
                var createTrackerRequest = new CreateTrackerRequest
                {
                    trackingNumber = trackingNumber,
                    shipmentReference = Guid.NewGuid().ToString(),
                    originCountryCode = "CN",
                    destinationCountryCode = "US",
                    destinationPostCode = "94901",
                    shippingDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),//"2021-03-01T11:09:00.000Z",
                    courierCode = new List<string> { "us-post" },
                    courierName = "USPS Standard",
                    trackingUrl = "https://tools.usps.com/go/TrackConfirmAction?tLabels="+trackingNumber,
                    orderNumber = Guid.NewGuid().ToString()
                };

                // Update the order with the tracker details
                
                var trackerResponse = await CreateTracker(createTrackerRequest);
                
                if (trackerResponse != null && trackerResponse.Data != null && trackerResponse.Data.Trackings != null)
                {
                    string trackerId = trackerResponse.Data.Trackings.FirstOrDefault().Tracker.TrackerId;
                    var trackingData = trackerResponse.Data?.Trackings?.FirstOrDefault()?.Tracker;
                    var shipmentData = trackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment;
                    var recipientData = trackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment.Recipient;
                    var deliveryData = trackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment.Delivery;
                    order.TrackerId = trackerId;

                    var tracker = new DomainModals.Tracker
                    {
                        TrackerId = Guid.NewGuid().ToString(),
                        TrackingNumber = trackingData.TrackingNumber,
                        IsSubscribed = trackingData.IsSubscribed,
                        CreatedAt = trackingData.CreatedAt
                        // Map other properties here
                    };

                    var recipient = new Recipient
                    {
                        RecipientId = Guid.NewGuid().ToString(),
                        Name = recipientData.Name,
                        Address = recipientData.Address,
                        PostCode = recipientData.PostCode,
                        City = recipientData.City,
                        Subdivision = recipientData.Subdivision
                        // Map other properties here
                    };

                    var delivery = new Delivery
                    {
                        DeliveryId = Guid.NewGuid().ToString(),
                        EstimatedDeliveryDate = deliveryData.EstimatedDeliveryDate,
                        Service = deliveryData.Service,
                        SignedBy = deliveryData.SignedBy,
                        
                        // Map other properties here
                    };

                    var shipment = new Shipment
                    {
                        ShipmentId = Guid.NewGuid().ToString(),
                        StatusCode = shipmentData.StatusCode,
                        StatusCategory = shipmentData.StatusCategory,
                        StatusMilestone = shipmentData.StatusMilestone,
                        OriginCountryCode = shipmentData.OriginCountryCode,
                        DestinationCountryCode = shipmentData.DestinationCountryCode,
                        Delivery = delivery,
                        TrackingNumbers = shipmentData.TrackingNumbers.Select(tn => new TrackingNumber
                        {
                            TrackingNumberId = Guid.NewGuid().ToString(),
                            Tn = tn.Tn
                            // Map other properties here
                        }).ToList(),
                        Recipient = recipient
                        // Map other properties here
                    };
 /*                   shipment.Delivery.Service = "";
                    shipment.Delivery.SignedBy = " ";*/

                    // Add the entities to your database context
                    _context.Trackers.Add(tracker);
                    _context.Recipient.Add(recipient);
                    _context.DeliveryInfos.Add(delivery);
                    _context.Shipments.Add(shipment);

                    // Save changes to the database
                    //await _context.SaveChangesAsync();

                    // Save the updated order in your database
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();

                    // Map the order to an OrderDto using AutoMapper
                    var updatedOrderDto = _mapper.Map<OrderDto>(order);

                    // Return the order with the tracker details to the client
                    return Ok(trackerResponse);
                }
                else
                {
                    
                    // Handle the error response from the Ship24 API
                    return BadRequest("Error creating tracker" );
                    /*var errorResponse = await response.Content.ReadAsStringAsync();

                    // Log the error or handle it in some way (e.g., throw an exception)
                    // In this case, we return the error response content as a string
                    return new CreateTrackerResponse*/
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during order creation
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("track/{trackerId}")]
        public async Task<IActionResult> TrackOrder(string trackerId)
        {
            try
            {
                // Use HttpClient to make a GET request to the Ship24 API to track an order
                var trackingResponse = await TrackOrderWithShip24(trackerId);

                if (trackingResponse != null)
                {
                    // Handle the tracking response as needed (e.g., update order status)
                    return Ok(trackingResponse);
                }
                else
                {
                    // Handle the error response from the Ship24 API
                    Console.WriteLine(Console.Error);
                    return BadRequest("Error tracking order");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API request
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("trackers")]
        public async Task<IActionResult> ListTrackers()
        {
            #region Ship24 list Of Trackers

            /*try
            {
                // Create a new HttpClient and set the Authorization header
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Ship24ApiKey);

                // Make a GET request to the Ship24 API to list existing trackers
                var response = await client.GetAsync("https://api.ship24.com/public/v1/trackers");

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the response manually into a list of TrackerDM
                    var trackers = JsonConvert.DeserializeObject<List<TrackerDM>>(responseBody);

                    // Handle the list of trackers as needed
                    return Ok(trackers);
                }
                else
                {
                    // Handle the error response from the Ship24 API
                    return BadRequest("Error from Ship24 API: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API request
                return StatusCode(500, "Internal server error: " + ex.Message);
            }*/
            #endregion
            var result = _context.Trackers.ToList();
            if (result == null || result.Count == 0)
            {
                return BadRequest("Tracker List is Empty...");
            }
            else
            {
                return Ok(result);
            }
        }
        /*[HttpGet("search/{trackingNumber}/results")]
public IActionResult GetTrackingResults(string trackingNumber)
{
    // Call the service method to retrieve tracking data based on the trackingNumber
    var tracking = _context.Trackers
        .Include(t => t.Shipment)
        .ThenInclude(s => s.Delivery)
        .Include(t => t.Shipment)
        .ThenInclude(s => s.TrackingNumbers)
        .Include(t => t.Shipment)
        .ThenInclude(s => s.Recipient)
        .Include(t => t.Events)
        .Include(t => t.Statistics)
        .FirstOrDefault(t => t.TrackingNumber == trackingNumber);

    if (tracking == null)
    {
        // If tracking data is not found, return a NotFound response
        return NotFound();
    }

    // Create the response structure manually based on the provided data
    var response = new CreateTrackerResponse
    {
        Data = new Data
        {
            Trackings = new List<Tracking>
            {
                new Tracking
                {
                    Tracker = new Tracker
                    {
                        TrackerId = tracking.TrackerId,
                        TrackingNumber = tracking.TrackingNumber
                    },
                    Shipment = new Shipment
                    {
                        ShipmentId = tracking.Shipment.ShipmentId,
                        StatusCode = tracking.Shipment.StatusCode,
                        StatusCategory = tracking.Shipment.StatusCategory,
                        StatusMilestone = tracking.Shipment.StatusMilestone,
                        OriginCountryCode = tracking.Shipment.OriginCountryCode,
                        DestinationCountryCode = tracking.Shipment.DestinationCountryCode,
                        Delivery = new Delivery
                        {
                            DeliveryId = tracking.Shipment.Delivery.DeliveryId,
                            EstimatedDeliveryDate = tracking.Shipment.Delivery.EstimatedDeliveryDate,
                            Service = tracking.Shipment.Delivery.Service,
                            SignedBy = tracking.Shipment.Delivery.SignedBy
                        },
                        TrackingNumbers = tracking.Shipment.TrackingNumbers.Select(tn => new TrackingNumber
                        {
                            TrackingNumberId = tn.TrackingNumberId,
                            Tn = tn.Tn
                        }).ToList(),
                        Recipient = new Recipient
                        {
                            RecipientId = tracking.Shipment.Recipient.RecipientId,
                            Name = tracking.Shipment.Recipient.Name,
                            Address = tracking.Shipment.Recipient.Address,
                            PostCode = tracking.Shipment.Recipient.PostCode,
                            City = tracking.Shipment.Recipient.City,
                            Subdivision = tracking.Shipment.Recipient.Subdivision
                        }
                    },
                    Events = tracking.Events.Select(e => new Event
                    {
                        EventId = e.EventId,
                        TrackingNumber = e.TrackingNumber,
                        EventTrackingNumber = e.EventTrackingNumber,
                        Status = e.Status,
                        OccurrenceDatetime = e.OccurrenceDatetime,
                        Order = e.Order,
                        Location = e.Location,
                        SourceCode = e.SourceCode,
                        CourierCode = e.CourierCode,
                        StatusCode = e.StatusCode,
                        StatusCategory = e.StatusCategory,
                        StatusMilestone = e.StatusMilestone
                    }).ToList(),
                    Statistics = new Statistics
                    {
                        Timestamps = new Timestamps
                        {
                            InfoReceivedDatetime = tracking.Statistics.Timestamps.InfoReceivedDatetime,
                            InTransitDatetime = tracking.Statistics.Timestamps.InTransitDatetime,
                            OutForDeliveryDatetime = tracking.Statistics.Timestamps.OutForDeliveryDatetime,
                            FailedAttemptDatetime = tracking.Statistics.Timestamps.FailedAttemptDatetime,
                            AvailableForPickupDatetime = tracking.Statistics.Timestamps.AvailableForPickupDatetime,
                            ExceptionDatetime = tracking.Statistics.Timestamps.ExceptionDatetime,
                            DeliveredDatetime = tracking.Statistics.Timestamps.DeliveredDatetime
                        }
                    }
                }
            }
        }
    };

    return Ok(response);
}
*/

        [HttpGet]
        [Route("webhook")]
        public IActionResult HeadWebhook()
        {
            // Ship24 will send a HEAD request to verify your endpoint
            // Return a 200 OK HTTP code to confirm readiness
            return Ok();
        }
        [HttpPost]
        [Route("webhook")]
        public IActionResult PostWebhook(dynamic data)
        {
            try
            {
                // Process the webhook data here
                string eventName = data.event_name;
                // Access other properties as needed
                // Return a success response
                return Ok("Webhook received successfully");
            }
            catch (Exception ex)
            {
                // Handle any errors here
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Add other order-related endpoints as needed

        private async Task<TrackerResponseTR.TrackerResponse> TrackOrderWithShip24(string trackerId)
        {
            try
            {
                // Create an HttpClient and set the authorization header
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Ship24ApiKey);

                // Construct the URL for tracking the order
                var trackUrl = $"https://api.ship24.com/public/v1/trackers/{trackerId}";
                var requestBody = JsonConvert.SerializeObject(trackerId);

                // Create the HTTP content with JSON format
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

                // Send the GET request to track the order
                var response = await client.GetAsync(trackUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response into a TrackerResponse object
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var trackerResponse = JsonConvert.DeserializeObject<TrackerResponseTR.TrackerResponse>(responseBody);
                    return trackerResponse;
                }
                else
                {
                    // Handle the error response from the Ship24 API
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API request
                return null;
            }
        }

        private async Task<CreateTrackerResponse> CreateTracker(CreateTrackerRequest createTrackerRequest)
        {
            try
            {
                // Create an HttpClient and set the authorization header
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Ship24ApiKey);

                // Construct the URL for creating a tracker
                var createTrackerUrl = "https://api.ship24.com/public/v1/trackers/track";

                // Serialize the request body to JSON
                var requestBody = JsonConvert.SerializeObject(createTrackerRequest);

                // Create the HTTP content with JSON format
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                // Send the POST request to create the tracker
                var response = await client.PostAsync(createTrackerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response into a CreateTrackerResponse object
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var createTrackerResponse = JsonConvert.DeserializeObject<CreateTrackerResponse>(responseBody);

                    // Create instances of your database entities and map the data
                    /*var trackerData = createTrackerResponse.Data?.Trackings?.FirstOrDefault()?.Tracker;
                    if (trackerData != null)
                    {
                        *//* var tracker = new Tracker
                         {
                             TrackerId = trackerData.TrackerId,
                             TrackingNumber = trackerData.TrackingNumber,
                             IsSubscribed = trackerData.IsSubscribed,
                             CreatedAt = trackerData.CreatedAt
                             // Map other properties here
                         };*//*

                        // Add the tracker entity to your database context
                        trackerData.TrackerId = Guid.NewGuid().ToString();
                        _context.Trackers.Add(trackerData);
                    }
                    var recipientData = createTrackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment.Recipient;
                    if (recipientData != null)
                    {
                        *//*var recipient = new Recipient
                        {
                            RecipientId = Guid.NewGuid().ToString(),
                            Name = recipientData.Name,
                            Address = recipientData.Address,
                            PostCode = recipientData.PostCode,
                            City = recipientData.City,
                            Subdivision = recipientData.Subdivision,
                        };
                        var test = new Recipient();
                        test = recipientData;*//*

                        recipientData.RecipientId = Guid.NewGuid().ToString();
                        _context.Recipient.Add(recipientData);
                    }

                    var trackingNumbersData = createTrackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment?.TrackingNumbers?.FirstOrDefault();
                    if (trackingNumbersData != null)
                    {
                        // Add the trackingNumber entity to your database context
                        trackingNumbersData.TrackingNumberId = Guid.NewGuid().ToString();
                        _context.TrackingNumber.Add(trackingNumbersData);
                    }
                    var deliveryData = createTrackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment.Delivery;
                    if (deliveryData != null)
                    {
                        *//*var delivery = new Delivery
                        {
                            EstimatedDeliveryDate = deliveryData.EstimatedDeliveryDate,
                            Service = deliveryData.Service,
                            SignedBy = deliveryData.SignedBy,
                        };*//*
                        deliveryData.DeliveryId = Guid.NewGuid().ToString();
                        _context.DeliveryInfos.Add(deliveryData);
                    }
                    var shipmentData = createTrackerResponse.Data?.Trackings?.FirstOrDefault()?.Shipment;
                    if (shipmentData != null)
                    {
                        *//*var shipment = new Shipment
                        {
                            ShipmentId = Guid.NewGuid().ToString(),
                            StatusCode = shipmentData.StatusCode,
                            StatusCategory = shipmentData.StatusCategory,
                            StatusMilestone = shipmentData.StatusMilestone,
                            OriginCountryCode = shipmentData.OriginCountryCode,
                            DestinationCountryCode = shipmentData.DestinationCountryCode,
                            *//* Delivery = shipmentData.Delivery,
                             Recipient = shipmentData.Recipient,*//*
                            // Map other properties here
                        };*//*
                        //shipmentData.Delivery = null;

                        shipmentData.ShipmentId = Guid.NewGuid().ToString();
                        //shipmentData.StatusCategory = "";
                        _context.Shipments.Add(shipmentData);
                    }
                      
                    
                    *//*var eventData = createTrackerResponse.Data?.Trackings?.FirstOrDefault()?.Events;
                    if(eventData.Count() > 0 && eventData != null)
                    {
                        var _events = new List<Event>();
                        _events.AddRange(eventData);

                        foreach(var item in _events)
                        {
                            item.EventId = Guid.NewGuid().ToString();
                        };
                        await _context.Events.AddRangeAsync(_events);
                    }*//*

                    



                    // Save changes to the database
                    await _context.SaveChangesAsync();*/

                    return createTrackerResponse;
                }


                else
                {
                    // Handle the error response from the Ship24 API
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var test = JsonConvert.DeserializeObject(errorResponse);
                    throw new Exception($"Ship24 API Error Response: {test}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API request
                return null;
            }
        }

        private string GenerateRandomDigits(int numberOfDigits)
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numberOfDigits; i++)
            {
                sb.Append(random.Next(10)); // Appends a random digit (0-9)
            }

            return sb.ToString();
        }
    }
}
