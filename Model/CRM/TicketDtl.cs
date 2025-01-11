namespace PIS_Web.Model.CRM
{
    public class TicketDtl
    {
        public int TicketDtlSN { get; set; }
        public decimal TicketSN { get; set; }
        public decimal? ExpertUserSN { get; set; }
        public string MessageBody { get; set; }
        public string TicketDs { get; set; }
        public string UserName { get; set; }
        public DateTime OprationDateTime { get; set; }
        public int? DefaultLanguageSN { get; set; }
        public string? UserID_Name { get; set; }
        public string? Host_Name { get; set; }
    }
}
