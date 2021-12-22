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
    public class AlbumItemsController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;

        public AlbumItemsController(DATABASEContext context)
        {
            _context = context;
        }
        //获取图片
        [HttpGet("AlbumItem")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetAlbumItem(string id)
        {
            List<AlbumItem> albumItems;
            try
            {
                albumItems = await _context.AlbumItems.Where(p => p.AlbumId == id).ToListAsync();
            }
            catch
            {
                albumItems = null;
            }
            code = 200;
            result = true;
            msg = "获取成功";
            return APIResultHelper.Success(code, msg, result, albumItems, albumItems.Count);

        }
        //上传图片
        [HttpPost("AlbumItem")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> PostAlbumItem([FromForm] FileUpload File, string id, string name)
        {
            AlbumItem albumItem = new AlbumItem();

            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string path = "http://122.9.8.178/";
            string extensionname = Path.GetExtension(File.files.Name);
            string url = path + time + extensionname;
            try
            {
                if (File.files.Length > 0)
                {
                    FileUploadHelper.FileUploadToLinux(url, File);

                    code = 200;
                    result = true;
                    msg = "上传成功";
                    albumItem.Id = time;
                    albumItem.Name = name;
                    albumItem.AlbumId = id;
                    albumItem.Path = url;
                    albumItem.GmtCreate = DateTime.Now;
                    albumItem.GmtModify = DateTime.Now;
                    _context.AlbumItems.Add(albumItem);
                    await _context.SaveChangesAsync();
                    var album = await _context.Albums.FindAsync(id);
                    album.Path = url;
                    await _context.SaveChangesAsync();
                    return APIResultHelper.Success(code, msg, result, new { url });
                }
                else
                {
                    code = 404;
                    result = false;
                    msg = "上传失败";
                    return APIResultHelper.Error(code, msg, result);
                }
            }
            catch (Exception ex)
            {
                code = 404;
                result = false;
                msg = "上传失败";
                return APIResultHelper.Error(code, msg, result, new { ex.Message });
            }

        }
        //删除图片
        [HttpDelete("AlbumItem")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> DeleteAlbumItem(string id)
        {
            var albumItem = await _context.AlbumItems.FindAsync(id);
            if (albumItem == null)
            {
                code = 404;
                result = false;
                msg = "图片不存在";
                return APIResultHelper.Error(code, msg, result);
            }
            _context.AlbumItems.Remove(albumItem);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "删除成功";
            return APIResultHelper.Success(code, msg, result);
        }
        /*
        // GET: api/AlbumItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlbumItem>>> GetAlbumItems()
        {
            return await _context.AlbumItems.ToListAsync();
        }
        */
        /*
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
        */
        /*
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
        */
        /*
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
        */
        /*
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
        */
        private bool AlbumItemExists(string id)
        {
            return _context.AlbumItems.Any(e => e.Id == id);
        }
    }
}
