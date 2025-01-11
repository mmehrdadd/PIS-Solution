using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PIS_Web.Model.CRM
{
    public class TicketAttachment
    {

        public long TicketAttachmentSN { get; set; }

        public decimal TicketSN { get; set; }

        public long? TicketDtlSN { get; set; }

        public decimal SortOrder { get; set; }

        public long AttachmentSubjectSN { get; set; }

        public long? AttachNo { get; set; }

        public string? AttachDes { get; set; }

        public string? AttachDate { get; set; }

        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public string? FileExtention { get; set; }

        public long? FileSize { get; set; }

        public long? FileExtSN { get; set; }

        public byte[]? FileSave { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string? UserIDName { get; set; }

        public string? HostName { get; set; }
        public byte[]? TimeStamp { get; set; }
    }
}
