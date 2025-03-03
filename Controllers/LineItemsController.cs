using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using prsdbNetWeb.Models;

namespace prsdbNetWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineItemsController : ControllerBase
    {
        private readonly prsdbContext _context;

        public LineItemsController(prsdbContext context)
        {
            _context = context;
        }

        // GET: api/LineItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItems()
        {
            var lineItems = _context.LineItems.Include(c => c.Product)
                                            .Include(c => c.Request);

            return await _context.LineItems.ToListAsync();
        }

        // GET: api/LineItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineItem>> GetLineItemById(int id)
        {
            var lineItems = await _context.LineItems.Include(c => c.Product)
                                          .Include(c => c.Request)
                                          .FirstOrDefaultAsync(c => c.Id == id);

            if (lineItems == null)
            {
                return NotFound();
            }
            return lineItems;
        }


        // PUT: api/LineItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineItem(int id, LineItem lineItem)
        {
            if (id != lineItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(lineItem).State = EntityState.Modified;

            try
            {
                
                await _context.SaveChangesAsync();
                await RecalculateTotal(lineItem.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemExists(id))
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

        // POST: api/LineItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineItem>> PostLineItem(LineItem lineItem)
        {
            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();
            await RecalculateTotal(lineItem.RequestId);

            return CreatedAtAction("GetLineItemById", new { id = lineItem.Id }, lineItem);
        }

        // DELETE: api/LineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLineItem(int id)
        {
            var lineItem = await _context.LineItems.FindAsync(id);
            if (lineItem == null)
            {
                return NotFound();
            }

            _context.LineItems.Remove(lineItem);
            await _context.SaveChangesAsync();
            await RecalculateTotal(lineItem.RequestId);

            return NoContent();
        }

        [HttpGet("lines-for-req/{reqId}")]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItemsByReqId(int reqId)
        {
            var lineItems = await _context.LineItems.Include(c => c.Product)
                                                    .Include(c => c.Request)
                                                     .Where(l => l.RequestId == reqId).ToListAsync();


            if (lineItems == null)
            {
                return NotFound();
            }
            return lineItems;
        }

        public async Task RecalculateTotal(int reqId)
        {
            var request = await _context.Requests.Where(r => r.Id == reqId).FirstOrDefaultAsync();

            var lineItems = await _context.LineItems.Include(l => l.Product)
                .Where(l => l.RequestId == reqId).ToListAsync();

            decimal sum = 0;
            foreach (var lineItem in lineItems)
            {
                if (lineItem.Product != null)
                {
                    sum += (lineItem.Product.Price * lineItem.Quantity);
                }

            }

            request.Total = sum;

            await _context.SaveChangesAsync();


        }




        private bool LineItemExists(int id)
        {
            return _context.LineItems.Any(e => e.Id == id);
        }
    }
}
