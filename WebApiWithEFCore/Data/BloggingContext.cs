﻿using Microsoft.EntityFrameworkCore;

namespace TestDatabase.Sample.WebApiWithEFCore.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options) { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
