using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Blog.API.Model;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Reflection.PortableExecutable;
using Blog.API.ExceptionHandle;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Blog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;

        public BlogsController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        // GET: api/<BlogsController>
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Blogs> blogs = GetBlogs();

            return Ok(blogs);
        }

        // GET api/<BlogsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                throw new ValidationException("Id can't be empty");
            }

            Blogs blog = new Blogs();
            IEnumerable<Blogs> blogs = GetBlogs();
            try
            {
                blog = blogs.Where(b => b.Id == id).FirstOrDefault();
                if (blog == null)
                {
                    throw new NotFoundException("Blog not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok(blog);
        }

        // POST api/<BlogsController>
        [HttpPost]
        public IActionResult Post([FromBody] Blogs blog)
        {
            if (string.IsNullOrEmpty(blog.UserName) || string.IsNullOrEmpty(blog.Text))
            {
                throw new ValidationException("User Name & Text can not be empty");
            }

            try
            {
                List<Blogs> blogs = GetBlogs()?.ToList();
                int Id = blogs?.Count() > 0 ? blogs.OrderByDescending(blog => blog.Id).FirstOrDefault().Id : 0;
                Id++;
                if (blogs == null)
                {
                    blogs = new List<Blogs>();
                }
                var createdDate = DateTime.Now.Date;
                blogs.Add(new Blogs { Id = Id, UserName = blog.UserName, DateCreated = createdDate, Text = blog.Text });
                SaveJson(blogs);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok();
        }

        // PUT api/<BlogsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Blogs blog)
        {
            if (string.IsNullOrEmpty(blog.UserName) || string.IsNullOrEmpty(blog.Text))
            {
                throw new ValidationException("User Name & Text can not be empty");
            }

            try
            {
                List<Blogs> blogs = GetBlogs().ToList();
                Blogs Blog = blogs.Where(b => b.Id == id).FirstOrDefault();

                Blog.UserName = blog.UserName;
                Blog.Text = blog.Text;

                blogs.Remove(blogs.Where(blog => blog.Id == id).FirstOrDefault());
                blogs.Add(Blog);
                SaveJson(blogs);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok();
        }

        // DELETE api/<BlogsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                throw new ValidationException("Id can't be empty");
            }

            try
            {
                List<Blogs> blogs = GetBlogs().ToList();
                blogs.Remove(blogs.Where(blog => blog.Id == id).FirstOrDefault());
                SaveJson(blogs);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Ok();
        }
        private IEnumerable<Blogs> GetBlogs()
        {

            var path = Path.Combine(_hostingEnvironment.WebRootPath, "data.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Blogs>>(json);
            }
        }
        private void SaveJson(List<Blogs> blogs)
        {
            var convertedJson = JsonConvert.SerializeObject(blogs, Formatting.Indented);
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "data.json");
            System.IO.File.WriteAllText(path, convertedJson);
        }
    }
}
