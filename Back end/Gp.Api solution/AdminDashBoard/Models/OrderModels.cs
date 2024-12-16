namespace AdminDashBoard.Models
{
    public class OrderModels
    {
        public int Id { get; set; }
        public DateTime? dateOfCreation { get; set; }

        public List<TransactionViewModel> Requests { get; set; }

        public string shipName { get; set; }

        public string fromCity { get; set; }

        public string tocity { get; set; }
        public decimal weight { get; set; }

        public decimal reward { get; set; }

        public DateTime date {  get; set; }

        public string TripName { get; set; }
        public string Status { get; set; }
        public string To { get; set; }

        public decimal Amount { get; set; }

        public string product { get; set; }


    }
}
