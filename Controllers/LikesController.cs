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
    public class LikesController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;

        public LikesController(DATABASEContext context)
        {
            _context = context;
        }

        
        //点赞
        [HttpPost("PostThumb")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> PostThumb(string id)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            Like like = new Like();
            like.UserId = userid;
            like.NewsId = id;
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "点赞成功";
            return APIResultHelper.Success(code, msg, result);
        }
        //取消点赞
        [HttpDelete("DeleteThumb")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> DeleteThumb(string id)
        {
            var like = await _context.Likes.FindAsync(id);
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "取消点赞";
            return APIResultHelper.Success(code, msg, result);
        }
        /*
        // GET: api/Likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
        {
            return await _context.Likes.ToListAsync();
        }
        */
        /*
        // GET: api/Likes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Like>> GetLike(string id)
        {
            var like = await _context.Likes.FindAsync(id);

            if (like == null)
            {
                return NotFound();
            }

            return like;
        }
        */
        /*
        // PUT: api/Likes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLike(string id, Like like)
        {
            if (id != like.UserId)
            {
                return BadRequest();
            }

            _context.Entry(like).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikeExists(id))
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
        // POST: api/Likes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Like>> PostLike(Like like)
        {
            _context.Likes.Add(like);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LikeExists(like.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLike", new { id = like.UserId }, like);
        }
        */
        /*
        // DELETE: api/Likes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLike(string id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool LikeExists(string id)
        {
            return _context.Likes.Any(e => e.UserId == id);
        }
    }
}
