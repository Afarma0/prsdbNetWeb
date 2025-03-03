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
        public async Task<ActionResult<Request>> PostRequest(RequestCreate r)
        {
            Request request = new Request();
            request.UserId = r.UserId;
            request.Description = r.Description;
            request.Justification = r.Justification;
            request.DateNeeded = r.DateNeeded;
            request.DeliveryMode = r.DeliveryMode;

            request.RequestNumber = getNextRequestNumber();


            request.Status = "NEW";
            request.SubmittedDate = DateTime.Now;
            request.Total = 0;
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
            if (request.Total > 50)
            {
                request.Status = "REVIEW";
            }
            else
            {
                request.Status = "APPROVED";
            }

            //set submitted date to current date
            request.SubmittedDate = DateTime.Now;
            _context.SaveChanges();
            //return updated request
            return request;

        }

        [HttpGet("list-review/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsOfOtherUsers(int userId)
        {
            var requests = await _context.Requests.Where(c => c.UserId != userId && c.Status == "REVIEW")
                                                   .ToListAsync();

            return requests;

        }
        [HttpPut("approve-request/{id}")]
        public Request ApproveRequest(int id)
        {
            //get the request for that id
            var request = _context.Requests.FirstOrDefault(c => c.Id == id);
            //update the status column to "APPROVED"
            if (request.Status == "REVIEW")
            {
                request.Status = "APPROVED";
            }
            
            
            _context.SaveChanges();
            //return updated request
            return request;

        }
        [HttpPut("reject-request/{id}")]
        public async Task<IActionResult> RejectRequest(int id, Request request)
        {
            
                if (id != request.Id)
                {
                    return BadRequest();
            }
            //set status to "REJECTED"
            //if (reasonforrejection != null)

            if (request.ReasonForRejection != null)
            {
                request.Status = "REJECTED";
                _context.Entry(request).State = EntityState.Modified;
            }  
            else
            {
                return BadRequest();
            }
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

        private string getNextRequestNumber()
        {
            // requestNumber format: R2409230011
            // 11 chars, 'R' + YYMMDD + 4 digit # w/ leading zeros
            string requestNbr = "R";
            // add YYMMDD string
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            requestNbr += today.ToString("yyMMdd");
            // get maximum request number from db
            string maxReqNbr = _context.Requests.Max(r => r.RequestNumber);
            String reqNbr = "";
            if (maxReqNbr != null)
            {
                // get last 4 characters, convert to number
                String tempNbr = maxReqNbr.Substring(7);
                int nbr = Int32.Parse(tempNbr);
                nbr++;
                // pad w/ leading zeros
                reqNbr += nbr;
                reqNbr = reqNbr.PadLeft(4, '0');
            }
            else
            {
                reqNbr = "0001";
            }
            requestNbr += reqNbr;
            return requestNbr;

        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
