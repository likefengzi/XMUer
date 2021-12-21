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
    public class AlbumItemsController : ControllerBase
    {
        private readonly DATABASEContext _context;

        public AlbumItemsController(DATABASEContext context)
        {
            _context = context;
        }

        // GET: api/AlbumItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlbumItem>>> GetAlbumItems()
        {
            return await _context.AlbumItems.ToListAsync();
        }

        // GET: api/AlbumItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AlbumItem>> GetAlbumItem(string id)
        {
            var albumItem = await _context.AlbumItems.FindAsync(id);

            if (albumItem == null)
            {
                return NotFound();
            }

            return albumItem;
        }

        // PUT: api/AlbumItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbumItem(string id, AlbumItem albumItem)
        {
            if (id != albumItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(albumItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumItemExists(id))
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

        // POST: api/AlbumItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AlbumItem>> PostAlbumItem(AlbumItem albumItem)
        {
            _context.AlbumItems.Add(albumItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AlbumItemExists(albumItem.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAlbumItem", new { id = albumItem.Id }, albumItem);
        }

        // DELETE: api/AlbumItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbumItem(string id)
        {
            var albumItem = await _context.AlbumItems.FindAsync(id);
            if (albumItem == null)
            {
                return NotFound();
            }

            _context.AlbumItems.Remove(albumItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlbumItemExists(string id)
        {
            return _context.AlbumItems.Any(e => e.Id == id);
        }
    }
}
