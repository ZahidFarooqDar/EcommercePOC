using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePOCThirdPartyAPI.Data;
using EcommercePOCThirdPartyAPI.DomainModals;

namespace EcommercePOCThirdPartyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly ProjectEcommerceContext _context;

        public SellersController(ProjectEcommerceContext context)
        {
            _context = context;
        }

        // GET: api/Sellers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seller>>> GetSellers()
        {
          if (_context.Sellers == null)
          {
              return NotFound();
          }
            return await _context.Sellers.ToListAsync();
        }

        // GET: api/Sellers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Seller>> GetSeller(string id)
        {
          if (_context.Sellers == null)
          {
              return NotFound();
          }
            var seller = await _context.Sellers.FindAsync(id);

            if (seller == null)
            {
                return NotFound();
            }

            return seller;
        }

        // PUT: api/Sellers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeller(string id, Seller seller)
        {
            if (id != seller.SellerId)
            {
                return BadRequest();
            }

            _context.Entry(seller).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sellers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Sellers
        [HttpPost]
        public async Task<ActionResult<Seller>> PostSeller(Seller seller)
        {
            if (_context.Sellers == null)
            {
                return Problem("Entity set 'ProjectEcommerceContext.Sellers' is null.");
            }

            // Generate a unique alphanumeric ID for the seller
            seller.SellerId = GenerateUniqueId();

            _context.Sellers.Add(seller);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SellerExists(seller.SellerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSeller", new { id = seller.SellerId }, seller);
        }

        // Helper method to generate a unique alphanumeric ID
        private string GenerateUniqueId()
        {
            // Generate a GUID without hyphens and convert it to a string
            return Guid.NewGuid().ToString("N");
        }


        // DELETE: api/Sellers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(string id)
        {
            if (_context.Sellers == null)
            {
                return NotFound();
            }
            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }

            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SellerExists(string id)
        {
            return (_context.Sellers?.Any(e => e.SellerId == id)).GetValueOrDefault();
        }
    }
}
