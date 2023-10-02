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
    public class BuyersController : ControllerBase
    {
        private readonly ProjectEcommerceContext _context;

        public BuyersController(ProjectEcommerceContext context)
        {
            _context = context;
        }

        // GET: api/Buyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Buyer>>> GetBuyers()
        {
          if (_context.Buyers == null)
          {
              return NotFound();
          }
            return await _context.Buyers.ToListAsync();
        }

        // GET: api/Buyers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Buyer>> GetBuyer(string id)
        {
          if (_context.Buyers == null)
          {
              return NotFound();
          }
            var buyer = await _context.Buyers.FindAsync(id);

            if (buyer == null)
            {
                return NotFound();
            }

            return buyer;
        }

        // PUT: api/Buyers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuyer(string id, Buyer buyer)
        {
            if (id != buyer.BuyerId)
            {
                return BadRequest();
            }

            _context.Entry(buyer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuyerExists(id))
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

        // POST: api/Buyers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Buyers
        [HttpPost]
        public async Task<ActionResult<Buyer>> PostBuyer(Buyer buyer)
        {
            if (_context.Buyers == null)
            {
                return Problem("Entity set 'ProjectEcommerceContext.Buyers' is null.");
            }

            // Generate a unique alphanumeric ID for the buyer
            buyer.BuyerId = GenerateUniqueId();

            _context.Buyers.Add(buyer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BuyerExists(buyer.BuyerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBuyer", new { id = buyer.BuyerId }, buyer);
        }

        // Helper method to generate a unique alphanumeric ID
        private string GenerateUniqueId()
        {
            // Generate a GUID without hyphens and convert it to a string
            return Guid.NewGuid().ToString("N");
        }


        // DELETE: api/Buyers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuyer(string id)
        {
            if (_context.Buyers == null)
            {
                return NotFound();
            }
            var buyer = await _context.Buyers.FindAsync(id);
            if (buyer == null)
            {
                return NotFound();
            }

            _context.Buyers.Remove(buyer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BuyerExists(string id)
        {
            return (_context.Buyers?.Any(e => e.BuyerId == id)).GetValueOrDefault();
        }
    }
}
