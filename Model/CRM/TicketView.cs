namespace PIS_Web.Model.CRM
{
    public class TicketView
    {
        public decimal TicketSN { get; set; }
        public decimal? TicketNo { get; set; }
        public string? ProblemDs { get; set; }
        public string TicketDs { get; set; }
        public string TicketDate { get; set; }
        public string TicketTime { get; set; }
        public string? TicketRefTypeDS { get; set; }
        public string? TelephoneNumber { get; set; }
        public string TicketTypeDs { get; set; }
        public string PriorityDs { get; set; }
        public string StatusDs { get; set; }
        public string TicketUserName { get; set; }
        public string? IPAddress { get; set; }
        public bool IsClosed { get; set; }
        public string? ApplicationDs { get; set; }
        public int TotalCount { get; set; }

    }
}
