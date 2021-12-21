using System;
using System.Collections.Generic;
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
    public class UsersController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        Boolean result;
        String msg;

        public UsersController(DATABASEContext context)
        {
            _context = context;
        }
        //用户登录
        [HttpGet("Login")]
        public async Task<ActionResult<APIResult>> Login(string id, string password)
        {
            String jwtstr;
            String token;
            var admin = await _context.Users.FindAsync(id);
            if (admin == null)
            {
                code = 404;
                result = false;
                msg = "用户不存在";
                return APIResultHelper.Success(code, new { result }, msg);
            }
            password = MD5Helper.MD5Encryption(password);
            if (password == admin.Password)
            {
                code = 200;
                result = true;
                msg = "登录成功";
                TokenModel tokenModel = new TokenModel { Uid = id, Role = "User" };
                jwtstr = JwtHelper.IssueJwt(tokenModel);
                token = jwtstr;
                return APIResultHelper.Success(code, new { token, result }, msg);
            }
            else
            {
                code = 405;
                result = false;
                msg = "密码错误";
                return APIResultHelper.Success(code, new { result }, msg);
            }
        }
        //用户注册
        [HttpPost("Register")]
        public async Task<ActionResult<APIResult>> Register(string id, string password)
        {
            User user;
            try
            {
                user = await _context.Users.FindAsync(id);
            }
            catch
            {
                user = null;
            }


            if (user != null)
            {
                code = 403;
                result = false;
                msg = "用户已存在";
                return APIResultHelper.Success(code, new { result }, msg);
            }
            password = MD5Helper.MD5Encryption(password);
            user = new User();
            user.Id = id;
            user.Password = password;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "注册成功";
            return APIResultHelper.Success(code, new { result }, msg);

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
                return APIResultHelper.Success(code, new { result }, msg);
            }
            code = 200;
            result = true;
            msg = "获取成功";
            return APIResultHelper.Success(code, new { result, avatar.Path }, msg);

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
                    return APIResultHelper.Success(code, new { result, url }, msg);
                }
                else
                {
                    code = 404;
                    result = false;
                    msg = "上传失败";
                    return APIResultHelper.Success(code, new { result }, msg);
                }
            }
            catch (Exception ex)
            {
                code = 404;
                result = false;
                msg = "上传失败";
                return APIResultHelper.Success(code, new { result, ex.Message }, msg);
            }

        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
