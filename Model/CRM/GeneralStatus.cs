namespace PIS_Web.Model.CRM
{
    public class GeneralStatus
    {
        public string GeneralStatusDtlDs { get; set; } // Nullable (VARCHAR 150)
        public decimal GeneralStatusDtlSN { get; set; } // Non-nullable

        //public decimal? SortOrder { get; set; } // Nullable decimal
        //public string UnqStr { get; set; } // Nullable (VARCHAR 50)
        //public string UserID_Name { get; set; } // Nullable (VARCHAR 50)
        //public string Host_Name { get; set; } // Nullable (VARCHAR 50)
        //public byte[] TimeStamp { get; set; } // Nullable Timestamp (RowVersion)
        //public long? OldID { get; set; } // Nullable BigInt
    }
}
