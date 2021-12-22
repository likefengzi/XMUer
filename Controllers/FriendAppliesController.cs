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
    public class FriendAppliesController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;

        public FriendAppliesController(DATABASEContext context)
        {
            _context = context;
        }
        //申请好友
        [HttpPost("ApplyFriend")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> ApplyFriend(string id)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            FriendApply friendapply = new FriendApply();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            friendapply.FromId = userid;
            friendapply.ToId = id;
            friendapply.GmtCreate = DateTime.Now;
            _context.FriendApplies.Add(friendapply);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "申请成功";
            return APIResultHelper.Success(code, msg, result);
        }
        //通过申请
        [HttpPut("PassApply")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> PassApply(string id)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            var apply = await _context.FriendApplies.Where(p => p.FromId == id).Where(p => p.ToId == userid).FirstAsync();
            _context.Remove(apply);
            await _context.SaveChangesAsync();
            Friend friend = new Friend();
            friend.MyId = userid;
            friend.OtherId = id;
            friend.GmtCreate = DateTime.Now;
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
            friend = new Friend();
            friend.MyId = id;
            friend.OtherId = userid;
            friend.GmtCreate = DateTime.Now;
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "添加成功";
            return APIResultHelper.Success(code, msg, result);
        }
        //拒绝申请
        [HttpPut("DeleteApply")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> DeleteApply(string id)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userid = JwtHelper.SerializeJwt(tokenHeader).Uid;
            var apply = await _context.FriendApplies.Where(p => p.FromId == id).Where(p => p.ToId == userid).FirstAsync();
            _context.Remove(apply);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "添加成功";
            return APIResultHelper.Success(code, msg, result);
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
