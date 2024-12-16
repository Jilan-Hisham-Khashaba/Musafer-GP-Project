using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GP.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync( StoreContext dbComtext)
        {
            if (!dbComtext.Country.Any()) //one element inside collection
            {


                var CountryData = File.ReadAllText("../GP.Repository/Data/DataSeed/Country.Json");

                var countries = JsonSerializer.Deserialize<List<Country>>(CountryData);

                if (countries?.Count > 0)
                {
                    foreach (var country in countries)
                        await dbComtext.Country.AddAsync(country);


                    await dbComtext.SaveChangesAsync();

                }

            }
            if (!dbComtext.City.Any()) //one element inside collection
            {


                var CityData = File.ReadAllText("../GP.Repository/Data/DataSeed/City.Json");

                var cities = JsonSerializer.Deserialize<List<City>>(CityData);

                if (cities?.Count > 0)
                {
                    foreach (var city in cities)
                        await dbComtext.City.AddAsync(city);


                    await dbComtext.SaveChangesAsync();

                }

            }
            if (!dbComtext.Categories.Any()) //one element inside collection
            {


                var CategoriesData = File.ReadAllText("../GP.Repository/Data/DataSeed/Categories.Json");

                var categories = JsonSerializer.Deserialize<List<Category>>(CategoriesData);

                if (categories?.Count > 0)
                {
                    foreach (var category in categories)
                        await dbComtext.Categories.AddAsync(category);


                    await dbComtext.SaveChangesAsync();

                }

            }

            //if (!dbComtext.Trips.Any()) //one element inside collection
            //{


            //    var TripsData = File.ReadAllText("../GP.Repository/Data/DataSeed/Trips.Json");

            //    var Trips = JsonSerializer.Deserialize<List<Trip>>(TripsData);

            //    if (Trips?.Count > 0)
            //    {
            //        foreach (var trip in Trips)
            //            await dbComtext.Trips.AddAsync(trip);



            //        await dbComtext.SaveChangesAsync();

            //    }

               

                

                
            //}
            //if (!dbComtext.Products.Any()) //one element inside collection
            //{


            //    var productData = File.ReadAllText("../GP.Repository/Data/DataSeed/Products.Json");

            //    var products = JsonSerializer.Deserialize<List<Product>>(productData);

            //    if (products?.Count > 0)
            //    {
            //        foreach (var product in products)
            //            await dbComtext.Products.AddAsync(product);


            //        await dbComtext.SaveChangesAsync();

            //    }

            //}
            //if (!dbComtext.shipments.Any()) //one element inside collection
            //{


            //    var ShipementData = File.ReadAllText("../GP.Repository/Data/DataSeed/Shipments.Json");

            //    var shipementes = JsonSerializer.Deserialize<List<Shipment>>(ShipementData);

            //    if (shipementes?.Count > 0)
            //    {
            //        foreach (var shipement in shipementes)
            //            await dbComtext.shipments.AddAsync(shipement);


            //        await dbComtext.SaveChangesAsync();

            //    }

            //}


        }
    }
}
