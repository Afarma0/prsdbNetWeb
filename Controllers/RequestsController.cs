using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prsdbNetWeb.Models;

namespace prsdbNetWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly prsdbContext _context;

        public RequestsController(prsdbContext context)
        {
            _context = context;
        }

        // *GET: api/Requests


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequest()
        {
            var products = _context.Requests.Include(c => c.User);

            return await _context.Requests.ToListAsync();
        }

        // **GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestById(int id)
        {
            var request = await _context.Requests.Include(c => c.User)
                                          .FirstOrDefaultAsync(c => c.Id == id);

            if (request == null)
            {
                return NotFound();
            }
            return request;
        }
            // PUT: api/Requests/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("submit-review/{id}")]
        public Request SubmitRequestForReview(int id)
        {
            //get the request for that id
            var request = _context.Requests.FirstOrDefault(c => c.Id == id);
            //update the status column to "REVIEW"
            request.Status = "REVIEW";
            //set submitted date to current date
            request.SubmittedDate = DateTime.Now;
            _context.SaveChanges();
            //return updated request
            return request;

        }






        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
