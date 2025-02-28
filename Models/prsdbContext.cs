using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prsdbNetWeb.Models
{
    public partial class prsdbContext : DbContext
    {
        public prsdbContext()
        {
        }

        public prsdbContext(DbContextOptions<prsdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LineItem> LineItems { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Request> Requests { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Vendor> Vendors { get; set; }

    }
}
