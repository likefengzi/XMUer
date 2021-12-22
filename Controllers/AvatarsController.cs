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
    public class AvatarsController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;

        public AvatarsController(DATABASEContext context)
        {
            _context = context;
        }
        //获取头像
        [HttpGet("Avatar")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetAvatar()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            Avatar avatar;
            try
            {
                avatar = await _context.Avatars.Where(p => p.UserId == id).FirstAsync();
            }
            catch
            {
                avatar = null;
            }

            if (avatar == null)
            {
                code = 403;
                result = false;
                msg = "头像不存在";
                return APIResultHelper.Error(code, msg, result);
            }
            code = 200;
            result = true;
            msg = "获取成功";
            return APIResultHelper.Success(code, msg, result, new { avatar.Path });

        }
        //上传头像
        [HttpPost("Avatar")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> PostAvatar([FromForm] FileUpload File)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            Avatar avatar;
            try
            {
                avatar = await _context.Avatars.Where(p => p.UserId == id).FirstAsync();
            }
            catch
            {
                avatar = null;
            }

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
                    if (avatar == null)
                    {
                        Avatar temp = new Avatar();
                        temp.Id = time;
                        temp.UserId = id;
                        temp.Path = url;
                        temp.GmtCreate = DateTime.Now;
                        temp.GmtModify = DateTime.Now;
                        _context.Avatars.Add(temp);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Avatars.Remove(avatar);
                        await _context.SaveChangesAsync();
                        Avatar temp = new Avatar();
                        temp.Id = time;
                        temp.UserId = id;
                        temp.Path = url;
                        temp.GmtCreate = DateTime.Now;
                        temp.GmtModify = DateTime.Now;
                        _context.Avatars.Add(temp);
                        await _context.SaveChangesAsync();
                    }
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
