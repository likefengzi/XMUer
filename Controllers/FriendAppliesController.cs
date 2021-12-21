using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XMUer.Models;

namespace XMUer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendAppliesController : ControllerBase
    {
        private readonly DATABASEContext _context;

        public FriendAppliesController(DATABASEContext context)
        {
            _context = context;
        }

        // GET: api/FriendApplies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendApply>>> GetFriendApplies()
        {
            return await _context.FriendApplies.ToListAsync();
        }

        // GET: api/FriendApplies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FriendApply>> GetFriendApply(string id)
        {
            var friendApply = await _context.FriendApplies.FindAsync(id);

            if (friendApply == null)
            {
                return NotFound();
            }

            return friendApply;
        }

        // PUT: api/FriendApplies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriendApply(string id, FriendApply friendApply)
        {
            if (id != friendApply.FromId)
            {
                return BadRequest();
            }

            _context.Entry(friendApply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendApplyExists(id))
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

        // POST: api/FriendApplies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FriendApply>> PostFriendApply(FriendApply friendApply)
        {
            _context.FriendApplies.Add(friendApply);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FriendApplyExists(friendApply.FromId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFriendApply", new { id = friendApply.FromId }, friendApply);
        }

        // DELETE: api/FriendApplies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendApply(string id)
        {
            var friendApply = await _context.FriendApplies.FindAsync(id);
            if (friendApply == null)
            {
                return NotFound();
            }

            _context.FriendApplies.Remove(friendApply);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FriendApplyExists(string id)
        {
            return _context.FriendApplies.Any(e => e.FromId == id);
        }
    }
}
