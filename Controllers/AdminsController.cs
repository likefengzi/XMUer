using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XMUer.Authorization;
using XMUer.Models;
using XMUer.Utility;

namespace XMUer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;
        int total;

        public AdminsController(DATABASEContext context)
        {
            _context = context;
        }
        //管理员登录
        [HttpGet("Login")]
        public async Task<ActionResult<APIResult>> Login(string id, string password)
        {
            String jwtstr;
            String token;
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                code = 404;
                result = false;
                msg = "用户不存在";
                return APIResultHelper.Error(code, msg, result);
            }
            password = MD5Helper.MD5Encryption(password);
            if (password == admin.Password)
            {
                code = 200;
                result = true;
                msg = "登录成功";
                TokenModel tokenModel = new TokenModel { Uid = id, Role = "Admin" };
                jwtstr = JwtHelper.IssueJwt(tokenModel);
                token = jwtstr;
                return APIResultHelper.Success(code, msg, result, new { token});
            }
            else
            {
                code = 405;
                result = false;
                msg = "密码错误";
                return APIResultHelper.Error(code, msg, result);
            }
        }
        //通过审核
        [HttpPut("Verify")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResult>> PassVerify(string id)
        {

            var user = await _context.Users.FindAsync(id);
            user.BeenAudit = 1;
            code = 200;
            result = true;
            msg = "审核成功";
            return APIResultHelper.Success(code, msg, result);

        }
        //通过审核
        [HttpDelete("Verify")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResult>> DeleteVerify(string id)
        {

            var user = await _context.Users.FindAsync(id);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "删除成功";
            return APIResultHelper.Success(code, msg, result);

        }
        // GET: api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(string id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(string id, Admin admin)
        {
            if (id != admin.Id)
            {
                return BadRequest();
            }

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(id))
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

        // POST: api/Admins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            _context.Admins.Add(admin);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdminExists(admin.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAdmin", new { id = admin.Id }, admin);
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminExists(string id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
