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
    public class FriendsController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;
        int total;

        public FriendsController(DATABASEContext context)
        {
            _context = context;
        }
        //获取好友
        [HttpGet("Friend")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> Friend()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            List<Friend> friends = await _context.Friends.Where(p => p.MyId==userid)
                .ToListAsync();
            code = 200;
            result = true;
            msg = "获取成功";
            return APIResultHelper.Success(code, msg, result, friends, friends.Count);
        }
        //删除好友
        [HttpDelete("Friend")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> DeleteFriend(string id)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            Friend friend = await _context.Friends.Where(p => p.MyId == userid).Where(p => p.OtherId == id).FirstAsync();
            _context.Remove(friend);
            await _context.SaveChangesAsync();
            friend = await _context.Friends.Where(p => p.MyId == id).Where(p => p.OtherId == userid).FirstAsync();
            _context.Remove(friend);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "删除成功";
            return APIResultHelper.Success(code, msg, result);
        }
        //搜索同学
        [HttpGet("SearchFriend")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> SearchFriend(string id, string name, string college, string dept, string major)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            List<User> users = await _context.Users.Where(p => EF.Functions.Like(p.Id, $"{id}%"))
                .Where(p => EF.Functions.Like(p.Name, $"{name}%"))
                .Where(p => EF.Functions.Like(p.College, $"{college}%"))
                .Where(p => EF.Functions.Like(p.Dept, $"{dept}%"))
                .Where(p => EF.Functions.Like(p.Major, $"{major}%"))
                .ToListAsync();
            code = 200;
            result = true;
            msg = "搜索成功";
            return APIResultHelper.Success(code, msg, result, users, users.Count);
        }
        /*
        // GET: api/Friends
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends()
        {
            return await _context.Friends.ToListAsync();
        }
        */
        /*
        // GET: api/Friends/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Friend>> GetFriend(string id)
        {
            var friend = await _context.Friends.FindAsync(id);

            if (friend == null)
            {
                return NotFound();
            }

            return friend;
        }
        */
        /*
        // PUT: api/Friends/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriend(string id, Friend friend)
        {
            if (id != friend.MyId)
            {
                return BadRequest();
            }

            _context.Entry(friend).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendExists(id))
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
        // POST: api/Friends
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Friend>> PostFriend(Friend friend)
        {
            _context.Friends.Add(friend);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FriendExists(friend.MyId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFriend", new { id = friend.MyId }, friend);
        }
        */
        /*
        // DELETE: api/Friends/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var friend = await _context.Friends.FindAsync(id);
            if (friend == null)
            {
                return NotFound();
            }

            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool FriendExists(string id)
        {
            return _context.Friends.Any(e => e.MyId == id);
        }
    }
}
