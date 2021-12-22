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
    public class NewsController : ControllerBase
    {
        private readonly DATABASEContext _context;
        int code;
        String msg;
        Boolean result;
        int total;

        public NewsController(DATABASEContext context)
        {
            _context = context;
        }
        //创建动态
        [HttpPost("CreateNews")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> PostNews([FromForm] FileUpload File, string content)
        {
            var tokenHeader = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var id = JwtHelper.SerializeJwt(tokenHeader).Uid;
            News news = new News();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string path = "http://122.9.8.178/";

            news.Id = time;
            news.UserId = id;
            news.Body = content;

            news.GmtCreate = DateTime.Now;
            news.GmtModify = DateTime.Now;
            try
            {
                if (File.files != null)
                {
                    string extensionname = Path.GetExtension(File.files.Name);
                    string url = path + time + extensionname;
                    FileUploadHelper.FileUploadToLinux(url, File);
                    news.Path = url;

                }
                else
                {
                    news.Path = null;
                }
                _context.News.Add(news);
                await _context.SaveChangesAsync();
                code = 200;
                result = true;
                msg = "发布成功";
                return APIResultHelper.Success(code, msg, result);
            }
            catch (Exception ex)
            {
                code = 404;
                result = false;
                msg = "上传失败";
                return APIResultHelper.Error(code, msg, result, new { ex.Message });
            }


        }
        //删除动态
        [HttpDelete("DeleteNews")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> DeleteNews(string id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            code = 200;
            result = true;
            msg = "发布成功";
            return APIResultHelper.Success(code, msg, result);
        }
        //获取动态点赞
        [HttpGet("GetThumb")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetThumb(string id)
        {
            List<Like> likes = await _context.Likes.Where(p => p.NewsId == id).ToListAsync();
            code = 200;
            result = true;
            msg = "点赞成功";
            return APIResultHelper.Success(code, msg, result,likes, likes.Count);
        }
        //获取动态评论
        [HttpGet("GetCommentsList")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<APIResult>> GetCommentsList(string id)
        {
            List<Comment> comments = await _context.Comments.Where(p => p.NewsId == id).ToListAsync();
            total = comments.Count;
            code = 200;
            result = true;
            msg = "点赞成功";
            return APIResultHelper.Success(code, msg, result, comments, total);
        }
        // GET: api/News
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNews()
        {
            return await _context.News.ToListAsync();
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNews(string id)
        {
            var news = await _context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            return news;
        }

        // PUT: api/News/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNews(string id, News news)
        {
            if (id != news.Id)
            {
                return BadRequest();
            }

            _context.Entry(news).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
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

        // POST: api/News
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<News>> PostNews(News news)
        {
            _context.News.Add(news);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NewsExists(news.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNews", new { id = news.Id }, news);
        }
        /*
        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(string id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool NewsExists(string id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
