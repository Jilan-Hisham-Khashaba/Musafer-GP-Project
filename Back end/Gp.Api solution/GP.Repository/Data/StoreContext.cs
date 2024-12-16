using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GP.core.Entities.identity;
using GP.Core.Entities.Orders;

namespace GP.Repository.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<City> City { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Country> Country { get; set; }

        public DbSet<Shipment> shipments { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<Comments> Comments { get; set; }


        public DbSet<Request> Requests { get; set; }

     

        public DbSet<verficationFaccess> verficationFaccess { get; set; }

         public DbSet<Order> Orders { get; set; }


        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<NotifactionMessage> NotifactionMessage { get; set; }

    }
}
