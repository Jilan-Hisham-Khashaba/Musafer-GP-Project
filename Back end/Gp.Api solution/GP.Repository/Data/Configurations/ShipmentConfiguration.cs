using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Data.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
        //    builder.Property(sh => sh.Reward)
        //.IsRequired()
        //.HasColumnType("decimal(18,2)");
        
            builder.Property(sh => sh.Weight).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(sh => sh.Address).IsRequired().HasMaxLength(100);

            builder.HasOne(s => s.FromCity)
       .WithMany()
       .HasForeignKey(s => s.FromCityID)
       .IsRequired().OnDelete(DeleteBehavior.NoAction);

           
            builder.HasOne(s => s.ToCity)
                .WithMany()
                .HasForeignKey(s => s.ToCityId)
                .IsRequired().OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Products)
                 .WithMany(p => p.shipments);


            //builder.HasOne(s => s.Category)
            //   .WithMany()
            //   .HasForeignKey(s => s.CategoryId)
            //   .IsRequired().OnDelete(DeleteBehavior.NoAction);

            
  


        }
    }
}
