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
    public class TripConfigurations : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.Property(T => T.availableKg).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(T => T.UnitPricePerKg).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(T => T.FromCity)
                .WithMany()
                .HasForeignKey(s => s.FromCityID)
                .IsRequired().OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(T => T.ToCity)
                .WithMany()
                .HasForeignKey(s => s.ToCityId)
                .IsRequired().OnDelete(DeleteBehavior.NoAction);

            // تعريف الحقل في قاعدة البيانات لتخزين القيمة المحسوبة
            builder.Property(o => o.SubTotal).HasColumnName("SubTotal")
                .HasColumnType("decimal(18,2)").HasComputedColumnSql("availableKg * UnitPricePerKg");

            

            

        }
    }
}


