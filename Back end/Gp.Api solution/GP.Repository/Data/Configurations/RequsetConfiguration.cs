using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Data.Configurations
{
    public class RequsetConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {

            builder.HasOne(s => s.Shipment)
               .WithMany()
               .HasForeignKey(s => s.ShipmentId)
               .IsRequired().OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(s => s.Trip)
               .WithMany()
               .HasForeignKey(s => s.TripId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
