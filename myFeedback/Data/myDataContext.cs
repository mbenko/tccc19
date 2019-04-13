using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace myFeedback.Models
{
    public class myDataContext : DbContext
    {
        public myDataContext (DbContextOptions<myDataContext> options)
            : base(options)
        {
        }

        public DbSet<myFeedback.Models.Feedback> Feedback { get; set; }
    }
}
