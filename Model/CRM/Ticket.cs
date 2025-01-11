using System;
using System.ComponentModel.DataAnnotations;

public class Ticket
{
    
    public decimal TicketSN { get; set; } 
    public decimal? FatherTicketSN { get; set; } 
    public int? TicketNo { get; set; } 
    public decimal? TicketRefTypeSN { get; set; } 
    public decimal? ProblemSN { get; set; }
    public decimal? ApplicationSN { get; set; } 
    public string TicketDs { get; set; } 
    public decimal? TicketUserSN { get; set; } 
    public string TicketUserName { get; set; } 
    public string TicketDate { get; set; } 
    public string TicketTime { get; set; } 
    public DateTime? TicketDateTime { get; set; } 
    public bool? IsSupervisorConfirm { get; set; } 
    public string IPAddress { get; set; } 
    public decimal? TicketTypeSN { get; set; } 
    public decimal? PrioritySN { get; set; } 
    public decimal? StatusSN { get; set; } 
    public bool? IsClosed { get; set; } 
    public int? DefaultLanguageSN { get; set; }
    public string UserID_Name { get; set; } 
    public string Host_Name { get; set; } 
    public int TotalCount { get; set; } 

}
