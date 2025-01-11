namespace PIS_Web.Model.Personnel
{
    public class Person
    {
        
            public decimal PersonSN { get; set; }
            public decimal? FatherPersonSN { get; set; }
            public string? PersonID { get; set; }
            public decimal? PersonTypeSN { get; set; }
            public string? PersonTypeDs { get; set; }
            public string? PersonCode { get; set; }
            public string? PersonName { get; set; }
            public string? FamilyCompanyName { get; set; }
            public string? PersonNameL { get; set; }
            public string? FamilyCompanyNameL { get; set; }
            public string? FatherName { get; set; }
            public string? RegID { get; set; }
            public string? RegDate { get; set; }
            public string? NationalID { get; set; }
            public string? EconomicalNo { get; set; }
            public string? CustomsNo { get; set; }
            public string? BusinessCardID { get; set; }
            public decimal? GeographicalLocationSN { get; set; }
            public string? GeographicalLocationNo { get; set; }
            public string? GeographicalLocationDS { get; set; }
            public decimal? FamilyStatusSN { get; set; }
            public string? FamilyStatusDs { get; set; }
            public decimal? SexTypeSN { get; set; }
            public string? SexTypeDs { get; set; }
            public decimal? MillitaryTypeSN { get; set; }
            public string? MilitaryTypeDs { get; set; }
            public string? BornIssueDate { get; set; }
            public string? ResidenceTypeDs { get; set; }
            public string? Address { get; set; }
            public string? ZipCode { get; set; }
            public string? Mobile { get; set; }
            public string? Email { get; set; }
            public string? BankBranchDs { get; set; }
            public string? BankCardNo { get; set; }
            public string? SHEBA { get; set; }
            public decimal? BornPlaceSN { get; set; }
            public string? BornPlaceNo { get; set; }
            public string? BornPlaceDs { get; set; }
            public decimal? CitizenshipSN { get; set; }
            public string? CitizenshipNo { get; set; }
            public string? CitizenshipDs { get; set; }
            public string? InsuranceNo { get; set; }
            public bool? IsInInsuranceList { get; set; }
            public string? InsuranceLastDate { get; set; }
            public string? InsurancePassword { get; set; }
            public string? Description { get; set; }
            public string? WorkExperience { get; set; }
            public decimal? AccDetailSN { get; set; }
            public decimal? JobStatusSN { get; set; }
            public string? JobStatusDs { get; set; }
            public decimal? InsuranceStatusSN { get; set; }
            public string? InsuranceStatusDs { get; set; }
            public decimal? IsUseRankingSN { get; set; }
            public string? IsUseRankingDs { get; set; }
            public decimal? RankingStatusSN { get; set; }
            public string? RankingStatusDs { get; set; }
            public decimal? UserSN { get; set; }
            public decimal? StatusSN { get; set; }
            public string? StatusDs { get; set; }
            public decimal? BusinessUnitSN { get; set; }
            public decimal? RegisteredUserSN { get; set; }
            public decimal? PersonAttachmentSN { get; set; }
            public string? FullName { get; set; }
            public string? FullNameL { get; set; }
            public string? DocDescription { get; set; }
            public string? PersonFullName { get; set; }
            public string? PersonFullNameL { get; set; }
            public bool? IsApplicant { get; set; }
            public string? UserID_Name { get; set; }
            public string? Host_Name { get; set; }
            public string? UserPassword { get; set; }    
               
            //public cPersonDtl PersonDtlItem { get; set; } = new cPersonDtl();
            //public cPersonBankInfo PersonBankInfoItem { get; set; } = new cPersonBankInfo();
            //public cPersonContract EmployeeContract { get; set; } = new cPersonContract();
        }
}
