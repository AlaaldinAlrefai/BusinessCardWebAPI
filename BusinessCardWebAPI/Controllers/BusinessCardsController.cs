using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessCardWebAPI.Core.Data;

namespace BusinessCardWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardsController : ControllerBase
    {
        private readonly BusinessCardDbContext _context;

        public BusinessCardsController(BusinessCardDbContext context)
        {
            _context = context;
        }

        // GET: api/BusinessCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessCards>>> GetBusinessCards()
        {
          if (_context.BusinessCards == null)
          {
              return NotFound();
          }
            return await _context.BusinessCards.ToListAsync();
        }

        // GET: api/BusinessCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessCards>> GetBusinessCards(int id)
        {
          if (_context.BusinessCards == null)
          {
              return NotFound();
          }
            var businessCards = await _context.BusinessCards.FindAsync(id);

            if (businessCards == null)
            {
                return NotFound();
            }

            return businessCards;
        }

        // PUT: api/BusinessCards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusinessCards(int id, BusinessCards businessCards)
        {
            if (id != businessCards.Id)
            {
                return BadRequest();
            }

            _context.Entry(businessCards).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessCardsExists(id))
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

        // POST: api/BusinessCards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BusinessCards>> PostBusinessCards(BusinessCards businessCards)
        {
          if (_context.BusinessCards == null)
          {
              return Problem("Entity set 'BusinessCardDbContext.BusinessCards'  is null.");
          }
            _context.BusinessCards.Add(businessCards);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBusinessCards", new { id = businessCards.Id }, businessCards);
        }

        // DELETE: api/BusinessCards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusinessCards(int id)
        {
            if (_context.BusinessCards == null)
            {
                return NotFound();
            }
            var businessCards = await _context.BusinessCards.FindAsync(id);
            if (businessCards == null)
            {
                return NotFound();
            }

            _context.BusinessCards.Remove(businessCards);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BusinessCardsExists(int id)
        {
            return (_context.BusinessCards?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
