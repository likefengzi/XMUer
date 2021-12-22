using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XMUer.Authorization;
using XMUer.Models;
using XMUer.Utility;

namespace XMUer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;

        public AlbumsController(DATABASEContext context)
        {
            _context = context;
        }
        //查看相册
        [HttpGet("Album")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetAlbum()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            List<Album> albums;
            try
            {
                albums = await _context.Albums.Where(p => p.UserId == id).ToListAsync();
            }
            catch
            {
                albums = null;
            }
            if (albums == null)
            {
                code = 404;
                result = false;
                msg = "没有相册";
                return APIResultHelper.Error(code, msg, result);
            }
            else
            {
                code = 200;
                result = true;
                msg = "获取成功";
                return APIResultHelper.Success(code, msg, result, albums, albums.Count());
            }
        }
        //创建相册
        [HttpPost("Album")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> PostAlbum(string name)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            Album album=new Album();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            album.Id = time;
            album.Name = name;
            album.UserId = id;
            album.GmtCreate = DateTime.Now;
            album.GmtModify = DateTime.Now;
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "创建成功";
            return APIResultHelper.Success(code, msg, result);
            
        }
        //删除相册
        [HttpDelete("Album")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> DeleteAlbum(string id)
        {
            Album album;
            try
            {
                album = await _context.Albums.FindAsync(id);
            }
            catch
            {
                album = null;
            }
            if (album == null)
            {
                code = 404;
                result = false;
                msg = "相册不存在";
                return APIResultHelper.Error(code, msg, result);
            }
            else
            {
                _context.Remove(album);
                await _context.SaveChangesAsync();
                code = 200;
                result = true;
                msg = "删除成功";
                return APIResultHelper.Success(code, msg, result);
            }
        }
        // GET: api/Albums
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Album>>> GetAlbums()
        {
            return await _context.Albums.ToListAsync();
        }

        // GET: api/Albums/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Album>> GetAlbum(string id)
        {
            var album = await _context.Albums.FindAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            return album;
        }

        // PUT: api/Albums/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbum(string id, Album album)
        {
            if (id != album.Id)
            {
                return BadRequest();
            }

            _context.Entry(album).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(id))
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

        // POST: api/Albums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum(Album album)
        {
            _context.Albums.Add(album);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AlbumExists(album.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAlbum", new { id = album.Id }, album);
        }

        /*
        // DELETE: api/Albums/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(string id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool AlbumExists(string id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
