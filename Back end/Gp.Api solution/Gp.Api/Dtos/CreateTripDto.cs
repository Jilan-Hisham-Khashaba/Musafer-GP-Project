using System.ComponentModel.DataAnnotations.Schema;

namespace Gp.Api.Dtos
{
    public class CreateTripDto
    {
    


        public decimal availableKg { get; set; }

        public DateTime arrivalTime { get; set; }

        public DateTime dateofCreation { get; set; }

     
        public string FromCityName { get; set; }

      

        public string CountryNameFrom { get; set; }


        
        public string ToCityName { get; set; }

       

        public string CountryNameTo { get; set; }
    }
}
