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
    public class AvatarsController : ControllerBase
    {
        private readonly DATABASEContext _context;

        public AvatarsController(DATABASEContext context)
        {
            _context = context;
        }

        // GET: api/Avatars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avatar>>> GetAvatars()
        {
            return await _context.Avatars.ToListAsync();
        }

        // GET: api/Avatars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Avatar>> GetAvatar(string id)
        {
            var avatar = await _context.Avatars.FindAsync(id);

            if (avatar == null)
            {
                return NotFound();
            }

            return avatar;
        }

        // PUT: api/Avatars/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvatar(string id, Avatar avatar)
        {
            if (id != avatar.Id)
            {
                return BadRequest();
            }

            _context.Entry(avatar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AvatarExists(id))
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

        // POST: api/Avatars
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Avatar>> PostAvatar(Avatar avatar)
        {
            _context.Avatars.Add(avatar);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AvatarExists(avatar.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAvatar", new { id = avatar.Id }, avatar);
        }

        // DELETE: api/Avatars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvatar(string id)
        {
            var avatar = await _context.Avatars.FindAsync(id);
            if (avatar == null)
            {
                return NotFound();
            }

            _context.Avatars.Remove(avatar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AvatarExists(string id)
        {
            return _context.Avatars.Any(e => e.Id == id);
        }
    }
}
