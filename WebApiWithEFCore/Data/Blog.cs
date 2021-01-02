using System.Collections.Generic;

namespace TestDatabase.Sample.WebApiWithEFCore.Data
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }
}