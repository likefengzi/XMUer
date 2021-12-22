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
    public class UsersController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;

        public UsersController(DATABASEContext context)
        {
            _context = context;
        }
        //用户Id
        [HttpGet("GetUserId")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetUserId()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            code = 200;
            result = true;
            msg = "获取成功";
            return APIResultHelper.Success(code, msg, result, new { id });
        }
        //用户登录
        [HttpGet("Login")]
        public async Task<ActionResult<APIResult>> Login(string id, string password)
        {
            String jwtstr;
            String token;
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                code = 404;
                result = false;
                msg = "用户不存在";
                return APIResultHelper.Error(code, msg, result);
            }
            password = MD5Helper.MD5Encryption(password);
            if (password == user.Password)
            {
                if (user.BeenAudit == 0)
                {
                    code = 405;
                    result = false;
                    msg = "未审核";
                    return APIResultHelper.Error(code, msg, result);
                }
                code = 200;
                result = true;
                msg = "登录成功";
                TokenModel tokenModel = new TokenModel { Uid = id, Role = "User" };
                jwtstr = JwtHelper.IssueJwt(tokenModel);
                token = jwtstr;
                return APIResultHelper.Success(code, msg, result, new { token });
            }
            else
            {
                code = 405;
                result = false;
                msg = "密码错误";
                return APIResultHelper.Error(code, msg, result);
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
                return APIResultHelper.Error(code, msg, result);
            }
            password = MD5Helper.MD5Encryption(password);
            user = new User();
            user.Id = id;
            user.Password = password;
            user.BeenAudit = 0;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "注册成功";
            return APIResultHelper.Success(code, msg, result);

        }

        //获取信息
        [HttpGet("UserInfo")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetUserInfo()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            var user = await _context.Users.FindAsync(id);


            code = 200;
            result = true;
            msg = "获取成功";
            User info = user;
            info.Password = null;
            return APIResultHelper.Success(code, msg, result, info);

        }
        //上传信息
        [HttpPut("UserInfo")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> SetUserInfo(string name, string email, string sexual, string birthday, string hometown, string college, string dept, string major, string grade, string music, string hobby, string book, string movie, string game, string anime, string sport)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            var user = await _context.Users.FindAsync(id);
            user.Name = name;
            user.Email = email;
            user.Sexual = Convert.ToByte(sexual);
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            user.Birthday = Convert.ToDateTime(birthday, dtFormat);
            user.Hometown = hometown;
            user.College = college;
            user.Dept = dept;
            user.Major = major;
            user.Grade = grade;
            user.Fmusic = music;
            user.Fhobby = hobby;
            user.Fbook = book;
            user.Fmovie = movie;
            user.Fgame = game;
            user.Fanime = anime;
            user.Fsport = sport;
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "修改成功";
            return APIResultHelper.Success(code, msg, result);

        }
        //获取用户动态
        [HttpGet("GetUserNews")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetUserNews()
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            List<News> news = await _context.News.Where(p => p.UserId == id).ToListAsync();
            code = 200;
            result = true;
            msg = "获取成功";
            return APIResultHelper.Success(code, msg, result, news,news.Count);

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
