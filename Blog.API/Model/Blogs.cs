using System.ComponentModel.DataAnnotations;

namespace Blog.API.Model
{
    public class Blogs
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateCreated { get; set; }
        public string Text { get; set; }
    }
}
