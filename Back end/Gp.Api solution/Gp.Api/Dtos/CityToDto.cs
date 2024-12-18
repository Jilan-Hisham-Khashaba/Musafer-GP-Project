namespace Gp.Api.Dtos
{
    public class CityToDto
    {
        public int Id { get; set; }
        public string CityName { get; set; }

        public int CountryId { get; set; }

        public string CountryName {  get; set; }
    }
}
