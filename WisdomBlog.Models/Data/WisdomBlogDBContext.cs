using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WisdomBlog.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomBlog.Models.Data
{
    public class WisdomBlogDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public WisdomBlogDbContext(DbContextOptions<WisdomBlogDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("WisdomBlog_API_DBContext"));
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}
