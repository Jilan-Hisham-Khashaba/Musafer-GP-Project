using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Data.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.Property(c => c.NameOfCity).IsRequired();

            // builder.
            //.WithMany()
            //.OnDelete(DeleteBehavior.Cascade);
          builder 
      .HasOne(c => c.Country)
      .WithMany(co => co.cities)
  
      .OnDelete(DeleteBehavior.Cascade); ;

        }
    }
}
