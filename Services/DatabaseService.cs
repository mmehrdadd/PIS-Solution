using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using TF.CommonComponent;
using PIS_Web.Model.CRM;
using PIS_Web.Model.Web;
using System.IO;
using System.Reflection;
using TF.MessageManagement.WebReference.SMSirUsersWS;
using PIS_Web.Model.Personnel;
using TF.SecuritySystem;
using PIS_Web.Pages.CRM;


public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;

    }
    private SqlConnection GetConnection()
    {
       

            return new SqlConnection(_connectionString);
             
    }

    public async Task<IEnumerable<Ticket>> GetTicketsAsync()
    {
        using var connection = GetConnection();
        const string query = "SELECT TicketSN,TicketDs FROM CRM.Ticket";
        return await connection.QueryAsync<Ticket>(query);
    }

    public async Task<IEnumerable<TicketView>> GetTicketsAsync(int pageNumber, int pageSize)
    {
        using var connection = GetConnection();
        const string query = @"
        SELECT TicketSN,TicketNo,ProblemDs,
        TicketDs,DBO.SlashDate(TicketDate) AS TicketDate ,dbo.Int2Time(TicketTime) AS TicketTime,TicketRefTypeDS,
        IPAddress,TicketUserName,TelephoneNumber,             
        ApplicationDs,TicketTypeDS,PriorityDs,       
        StatusDs,IsClosed,
        COUNT(*) OVER() AS TotalCount
        FROM [CRM].[_VW_TicketFull]
        ORDER BY TicketDate DESC , TicketTime DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

        var parameters = new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        return await connection.QueryAsync<TicketView>(query, parameters);
    }

    public async Task<IEnumerable<GeneralStatus>> GetPriorityAsync()
    {
        using var connection = GetConnection();
        const string query = " SELECT GeneralStatusDtlDs,GeneralStatusDtlSN FROM dbo._baVW_GeneralStatus AS GeneralStatus WHERE GeneralStatusSN = 556";
        return await connection.QueryAsync<GeneralStatus>(query);
    }

    public async Task<IEnumerable<seApplication>> GetSeApplicationsAsync()
    {
        using var connection = GetConnection();
        const string query = "SELECT ApplicationSN, FarsiName FROM dbo.seApplication";
        return await connection.QueryAsync<seApplication>(query);
    }

    public async Task<IEnumerable<GeneralStatus>> GetTicketTypeAsync()
    {
        using var connection = GetConnection();
        const string query = "SELECT GeneralStatusDtlSN,GeneralStatusDtlDs  FROM PIS.dbo._baVW_GeneralStatus WHERE GeneralStatusSN = 555";
        return await connection.QueryAsync<GeneralStatus>(query);
    }

    public async Task<IEnumerable<GeneralStatus>> GetRecordStatusAsync()
    {
        using var connection = GetConnection();
        const string query = "SELECT GeneralStatusDtlSN, GeneralStatusDtlDs FROM dbo._baVW_GeneralStatus WHERE GeneralStatusSN = 3";
        return await connection.QueryAsync<GeneralStatus>(query);
    }

    public async Task<IEnumerable<seUser>> GetExpertsAsync()
    {
        using var connection = GetConnection();
        const string query = "SELECT UserSN, UserName FROM dbo.seUser WHERE IsExpert = 1";
        return await connection.QueryAsync<seUser>(query);
    }

    public async Task<TicketView> GetTicketBySNAsync(decimal? ticketSN)
    {
        using var connection = GetConnection();
        const string query = @"SELECT TicketSN,
                               TicketNo,
                               ProblemNo,
                               ProblemDs,
                               TicketDs,
                               TicketRefTypeDS,
                               IPAddress,
                               TicketUserName,
                               TelephoneNumber,
                               ApplicationID,
                               ApplicationDs,
                               TicketTypeDS,
                               PriorityDs,
                               StatusDs,
                               IsClosed,dbo.Int2Time(TicketTime) AS TicketTime,dbo.SlashDate(TicketDate) as TicketDate FROM  CRM._VW_TicketFull WHERE TicketSN = @Id";
        return await connection.QueryFirstOrDefaultAsync<TicketView>(query, new { Id = ticketSN });
    }

    public async Task<Ticket> GetTicketBySN_InsertAsync(decimal? ticketSN)
    {
        using var connection = GetConnection();
        const string query = @"SELECT TicketSN
                              ,FatherTicketSN
                              ,TicketNo
                              ,ProblemSN
                              ,ProblemNo
                              , ProblemDs
                              ,TicketDs
                               ,dbo.SlashDate(TicketDate) AS TicketDate
                              , dbo.Int2Time(TicketTime) as TicketTime
                              ,TicketRefTypeSN
                              ,TicketRefTypeDS
                              ,IPAddress
                              ,TicketUserSN
                              ,TicketUserName
                              ,TelephoneNumber
                              , ApplicationSN
                              ,ApplicationID
                              , ApplicationDs
                              ,Host_Name
                              ,TicketTypeSN
                              ,TicketTypeDS
                              ,PrioritySN
                              ,PriorityDs
                              ,StatusSN
                              , StatusDs
                              FROM CRM._VW_TicketFull WHERE TicketSN = @Id";
        return await connection.QueryFirstOrDefaultAsync<Ticket>(query, new { Id = ticketSN });
    }
    public async Task<IEnumerable<TicketView>> GetUserTicketAsync(decimal? userSN, int pageNumber, int pageSize)
    {
        using var connection = GetConnection();
        //const string query = @"SELECT *, COUNT(*) OVER() AS TotalCount  FROM CRM.Ticket WHERE TicketUserSN = @SN
        //                       ORDER BY TicketDate DESC
        //                       OFFSET @Offset ROWS
        //                       FETCH NEXT @PageSize ROWS ONLY";
        const string query2 = "SELECT TicketSN,FatherTicketSN,TicketNo,ProblemSN,ProblemNo, ProblemDs,TicketDs," +
                              " dbo.SlashDate(TicketDate) AS TicketDate, dbo.Int2Time(TicketTime) as TicketTime,TicketRefTypeSN,TicketRefTypeDS" +
                              ",IPAddress,TicketUserSN,TicketUserName,TelephoneNumber, ApplicationSN,ApplicationID, ApplicationDs,Host_Name,TicketTypeSN" +
                              ",TicketTypeDS,PrioritySN,PriorityDs,StatusSN, StatusDs, IsClosed,COUNT(TicketSN) OVER() AS TotalCount FROM CRM._VW_TicketFull WHERE TicketUserSN = @SN ORDER BY TicketDate DESC , TicketTime DESC" +
                              " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        var parameters = new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize,
            SN = userSN

        };
        return await connection.QueryAsync<TicketView>(query2, parameters);
    }
    public async Task<IEnumerable<TicketView>> GetUserTicketAsync(decimal? userSN)
    {
        using var connection = GetConnection();
        //const string query = @"SELECT *, COUNT(*) OVER() AS TotalCount  FROM CRM.Ticket WHERE TicketUserSN = @SN
        //                       ORDER BY TicketDate DESC
        //                       OFFSET @Offset ROWS
        //                       FETCH NEXT @PageSize ROWS ONLY";
        const string query2 = "SELECT TicketSN" +
                              ",FatherTicketSN" +
                              ",TicketNo" +
                              ",ProblemSN" +
                              ",ProblemNo" +
                              ", ProblemDs" +
                              ",TicketDs" +
                              ",dbo.SlashDate(TicketDate) AS TicketDate" +
                              ",dbo.Int2Time(TicketTime) as TicketTime" +
                              ",TicketRefTypeSN,TicketRefTypeDS" +
                              ",IPAddress" +
                              ",TicketUserSN" +
                              ",TicketUserName" +
                              ",TelephoneNumber" +
                              ", ApplicationSN" +
                              ",ApplicationID" +
                              ", ApplicationDs" +
                              ",Host_Name" +
                              ",TicketTypeSN" +
                              ",TicketTypeDS" +
                              ",PrioritySN" +
                              ",PriorityDs" +
                              ",StatusSN" +
                              ", StatusDs" +
                              ", IsClosed" +
                              ",COUNT(TicketSN) OVER() AS TotalCount" +
                              " FROM CRM._VW_TicketFull" +
                              " WHERE TicketUserSN = @SN ORDER BY TicketDate DESC , TicketTime DESC";

        return await connection.QueryAsync<TicketView>(query2, new { SN = userSN });
    }
    public async Task AddTicketAsync(Ticket ticket)
    {
        using var connection = GetConnection();

        const string query = "SELECT TicketSN FROM CRM.Ticket ORDER BY TicketSN DESC";
        var Tickets = await connection.QueryAsync<Ticket>(query);
        var lastTicket = Tickets.FirstOrDefault();
        if (lastTicket != null)
        {
            ticket.TicketSN = lastTicket.TicketSN + 1;
        }
        else
        {
            ticket.TicketSN = 1.101m;
        }
        const string query2 = "INSERT INTO CRM.Ticket (TicketSN, FatherTicketSN, TicketNo,TicketRefTypeSN,ProblemSN,ApplicationSN, TicketDs," +
                             "TicketUserSN,TicketUserName,TicketDate,TicketTime, TicketDateTime, IsSupervisorConfirm,IPAddress,TicketTypeSN, PrioritySN" +
                             ",StatusSN,IsClosed,DefaultLanguageSN,UserID_Name, Host_Name) " +
                             "VALUES (@TicketSN, @FatherTicketSN, @TicketNo, @TicketRefTypeSN, @ProblemSN, @ApplicationSN, @TicketDs, @TicketUserSN" +
                             ",@TicketUserName, @TicketDate, @TicketTime, @TicketDateTime, @IsSupervisorConfirm, @IPAddress, @TicketTypeSN" +
                             ", @PrioritySN, @StatusSN, @IsClosed, @DefaultLanguageSN, @UserID_Name, @Host_Name)";
        await connection.ExecuteAsync(query2, ticket);


        //var Parameters = new DynamicParameters();
        //Parameters.Add("@TicketSN", DbType.Decimal, direction: ParameterDirection.Output);
        //Parameters.Add("@FatherTicketSN", ticket.FatherTicketSN, DbType.Decimal);
        //Parameters.Add("@TicketNo", ticket.TicketNo, DbType.Int64);
        //Parameters.Add("@TicketRefTypeSN", ticket.TicketRefTypeSN, DbType.Decimal);
        //Parameters.Add("@ProblemSN", ticket.ProblemSN, DbType.Decimal);
        //Parameters.Add("@ApplicationSN", ticket.ApplicationSN, DbType.Decimal);
        //Parameters.Add("@TicketDs", ticket.TicketDs, DbType.String);
        //Parameters.Add("@TicketUserSN", ticket.TicketUserSN, DbType.Decimal);
        //Parameters.Add("@TicketUserName", ticket.TicketUserName, DbType.String);
        //Parameters.Add("@TicketDate", ticket.TicketDate, DbType.String);
        //Parameters.Add("@TicketTime", ticket.TicketTime, DbType.String);
        //Parameters.Add("@TicketDateTime", ticket.TicketDateTime, DbType.DateTime);
        //Parameters.Add("@IsSupervisorConfirm", ticket.IsSupervisorConfirm, DbType.Int16);
        //Parameters.Add("@IPAddress", ticket.IPAddress, DbType.String);
        //Parameters.Add("@TicketTypeSN", ticket.TicketTypeSN, DbType.Decimal);
        //Parameters.Add("@PrioritySN", ticket.PrioritySN, DbType.Decimal);
        //Parameters.Add("@StatusSN", ticket.StatusSN, DbType.Decimal);
        //Parameters.Add("@IsClosed", ticket.IsClosed, DbType.Int16);
        //Parameters.Add("@DefaultLanguageSN", ticket.DefaultLanguageSN, DbType.Int16);
        //Parameters.Add("@UserID_Name", ticket.UserID_Name, DbType.String);
        //Parameters.Add("@Host_Name", ticket.Host_Name, DbType.String);


        //await connection.ExecuteAsync("CRM.Ticket_Insert_X", Parameters, commandType: CommandType.StoredProcedure);
        //var ticketSN = Parameters.Get<decimal>("TicketSN");
    }

    public async Task<int> UpdateTicketAsync(Ticket ticket)
    {
        using var connection = GetConnection();
        const string query = "UPDATE CRM.Ticket" +
                             " SET FatherTicketSN = @FatherTicketSN" +
                             ",TicketRefTypeSN = @TicketRefTypeSN" +
                             ",ProblemSN = @ProblemSN" +
                             ",ApplicationSN = @ApplicationSN" +
                             ",TicketDs = @TicketDs" +
                             ",TicketUserSN = @TicketUserSN" +
                             ",TicketUserName = @TicketUserName" +
                             ",TicketDateTime = @TicketDateTime" +
                             ",IsSupervisorConfirm = @IsSupervisorConfirm" +
                             ",IPAddress = @IPAddress" +
                             ",TicketTypeSN = @TicketTypeSN" +
                             ", PrioritySN = @PrioritySN" +
                             ",StatusSN = @StatusSN" +
                             ",IsClosed = @IsClosed" +
                             ",DefaultLanguageSN = @DefaultLanguageSN,UserID_Name = @UserID_Name" +
                             ",Host_Name = @Host_Name" +
                             " WHERE  TicketSN = @TicketSN";
        return await connection.ExecuteAsync(query, ticket);
    }

    public async Task<int> DeleteTicketAsync(decimal ticketSN)
    {
        using var connection = GetConnection();
        const string query = "DELETE FROM CRM.Ticket" +
                             " WHERE TicketSN = @Id";
        return await connection.ExecuteAsync(query, new { Id = ticketSN });
    }
    public async Task<PIS_Web.Model.CRM.User> GetPassAsync(long userId)
    {
        using var connection = GetConnection();
        var parameters = new DynamicParameters();
        //var query = "declare @UserName SmallDes , @Password SmallDes EXECUTE [_seSP_GetUserInfoByUserID] @Id , @Password OUTPUT , @UserName output select @UserName AS UserName, @Password AS Password";
        const string query2 = "SELECT [UserSN]" +
                              ",[UserNo]" +
                              ",[UserName]" +
                              ",[UserPassword]" +
                              ",[IsProgrammer]" +
                              ",[IsAdmin]" +
                              " FROM [PIS].[dbo].[seUser]" +
                              " WHERE UserID = @userId";
        //parameters.Add("@UserID", userId);
        //parameters.Add("@Password", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        //parameters.Add("@UserName", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        //var newUser = await connection.QueryFirstOrDefaultAsync<USER>("[dbo].[_seSP_GetUserInfoByUserID]", parameters, commandType: CommandType.StoredProcedure);
        var user = await connection.QueryFirstOrDefaultAsync<PIS_Web.Model.CRM.User>(query2, new { userId = userId });
        return user;
    }
    public async Task SaveAttachmentForTicketAsync(TicketAttachment aTicketAttachment)
    {
        using var connection = GetConnection();
        var Parameters = new DynamicParameters();
        Parameters.Add("@TicketAttachmentSN", DbType.Int64, direction: ParameterDirection.Output);
        Parameters.Add("@TicketSN", aTicketAttachment.TicketSN, DbType.Decimal);
        Parameters.Add("@SortOrder", aTicketAttachment.SortOrder, DbType.Decimal);
        Parameters.Add("@AttachmentSubjectSN", aTicketAttachment.AttachmentSubjectSN, DbType.Int64);
        Parameters.Add("@AttachNo", aTicketAttachment.AttachNo, DbType.String);
        Parameters.Add("@AttachDes", aTicketAttachment.AttachDes, DbType.String);
        Parameters.Add("@AttachDate", aTicketAttachment.AttachDate, DbType.String);
        Parameters.Add("@FileName", aTicketAttachment.FileName, DbType.String);
        Parameters.Add("@FilePath", aTicketAttachment.FilePath, DbType.String);
        Parameters.Add("@FileSize", aTicketAttachment.FileSize, DbType.Int64);
        Parameters.Add("@FileExtention", aTicketAttachment.FileExtention, DbType.String);
        Parameters.Add("@UserID_Name", DBNull.Value, DbType.String);
        Parameters.Add("@Host_Name", CCommonFunction.GetHostName(), DbType.String);

        await connection.QueryAsync<int>("CRM.TicketAttachment_Insert_X", Parameters, commandType: CommandType.StoredProcedure);
        int AttachmentSN = 0;
        AttachmentSN = Parameters.Get<int>("TicketAttachmentSN");
        const string query = $"UPDATE CRM.TicketAttachment" +
                             $" SET FileSave = @File" +
                             $" WHERE TicketAttachmentSN = @AttachmentSN";
        await connection.ExecuteAsync(query, new { File = aTicketAttachment.FileSave, AttachmentSN = AttachmentSN });

    }
    public async Task<IEnumerable<AttachmentSubject>> GetAttachmentSubjectsAsync()
    {
        using var connection = GetConnection();
        const string query = "SELECT AttachmentSubjectSN" +
                             ",AttachmentSubjectDs" +
                             ",SortOrder " +
                             " FROM General.AttachmentSubject";
        return await connection.QueryAsync<AttachmentSubject>(query);
    }
    public async Task<IEnumerable<TicketAttachment>> GetTicketAttachmentsAsync(decimal TicketSN)
    {
        using var connection = GetConnection();
        const string query = "SELECT TicketAttachmentSN" +
                             ",TicketSN" +
                             ",TicketDtlSN" +
                             ",AttachDes" +
                             ",dbo.SlashDate(AttachDate) as AttachDate,FileName,FileSave " +
                             "FROM CRM.TicketAttachment WHERE TicketSN = @SN";
        return await connection.QueryAsync<TicketAttachment>(query, new { SN = TicketSN });
    }
    public async Task DeleteTicketAttachmentAsync(int AttachmentSN)
    {
        var connection = GetConnection();
        const string query = "DELETE FROM CRM.TicketAttachment" +
                             " WHERE TicketAttachmentSN = @AttachmentSN";
        await connection.ExecuteAsync(query, new { AttachmentSN = AttachmentSN });
    }
    public async Task<IEnumerable<Problem>> GetProblemListAsync()
    {
        var connection = GetConnection();
        const string query = "SELECT ProblemSN" +
                             ",FatherProblemSN" +
                             ", ProblemDs" +
                             " FROM General.Problem";
        return await connection.QueryAsync<Problem>(query);
    }
    public async Task<IEnumerable<TicketDtl>> GetTicketDtlListAsync()
    {
        var connection = GetConnection();
        const string query = @"SELECT TicketDtlSN
                              ,S.TicketDs
                              ,T.TicketSN
                              ,ExpertUserSN
                              ,MessageBody
                              ,OprationDateTime
                              ,U.UserName
                              FROM CRM.TicketDtl AS T INNER JOIN dbo.seUser AS U ON  ExpertUserSN = U.UserSN
                              INNER JOIN CRM.Ticket AS S ON T.TicketSN = S.TicketSN";
        return await connection.QueryAsync<TicketDtl>(query);
    }

    public async Task AddTicketDtlAsync(TicketDtl newTicketDtl)
    {
        var connection = GetConnection();
        const string query = "SELECT TicketDtlSN FROM CRM.TicketDtl" +
                             " ORDER BY TicketDtlSN DESC";
        var TicketDtl = await connection.QueryAsync<TicketDtl>(query);
        var lastTicket = TicketDtl.FirstOrDefault();
        if (lastTicket != null)
        {
            newTicketDtl.TicketDtlSN = lastTicket.TicketDtlSN + 1;
        }
        const string query2 = "INSERT INTO CRM.TicketDtl (TicketDtlSN, TicketSN, ExpertUserSN,MessageBody,OprationDateTime,DefaultLanguageSN,UserID_Name,Host_Name) \r\n" +
                              "VALUES (@TicketDtlSN,@TicketSN,@ExpertUserSN,@MessageBody,DEFAULT,@DefaultLanguageSN,@UserID_Name,DEFAULT)";
        await connection.QueryAsync<TicketDtl>(query2, newTicketDtl);
    }
    public async Task AddTicketDtlAttachment(TicketAttachment aTicketAttachment)
    {
        var connection = GetConnection();
        const string query = "SELECT TicketDtlSN FROM CRM.TicketDtl" +
                             " ORDER BY TicketDtlSN DESC";
        var TicketDtl = await connection.QueryAsync<TicketDtl>(query);
        var lastTicketDtlSN = TicketDtl.FirstOrDefault().TicketDtlSN;
        aTicketAttachment.TicketDtlSN = lastTicketDtlSN;
        var Parameters = new DynamicParameters();
        Parameters.Add("@TicketAttachmentSN", DbType.Int64, direction: ParameterDirection.Output);
        Parameters.Add("@TicketSN", aTicketAttachment.TicketSN, DbType.Decimal);
        Parameters.Add("@SortOrder", aTicketAttachment.SortOrder, DbType.Decimal);
        Parameters.Add("@AttachmentSubjectSN", aTicketAttachment.AttachmentSubjectSN, DbType.Int64);
        Parameters.Add("@AttachNo", aTicketAttachment.AttachNo, DbType.String);
        Parameters.Add("@AttachDes", aTicketAttachment.AttachDes, DbType.String);
        Parameters.Add("@AttachDate", aTicketAttachment.AttachDate, DbType.String);
        Parameters.Add("@FileName", aTicketAttachment.FileName, DbType.String);
        Parameters.Add("@FilePath", aTicketAttachment.FilePath, DbType.String);
        Parameters.Add("@FileSize", aTicketAttachment.FileSize, DbType.Int64);
        Parameters.Add("@FileExtention", aTicketAttachment.FileExtention, DbType.String);
        Parameters.Add("@UserID_Name", DBNull.Value, DbType.String);
        Parameters.Add("@Host_Name", CCommonFunction.GetHostName(), DbType.String);

        await connection.QueryAsync<int>("CRM.TicketAttachment_Insert_X", Parameters, commandType: CommandType.StoredProcedure);
        int AttachmentSN = 0;
        AttachmentSN = Parameters.Get<int>("TicketAttachmentSN");
        const string query2 = $"UPDATE CRM.TicketAttachment" +
                             $" SET FileSave = @File" +
                             $",TicketDtlSN = @TicketDtlSN" +
                             $" WHERE TicketAttachmentSN = @AttachmentSN";
        await connection.ExecuteAsync(query2, new { File = aTicketAttachment.FileSave, AttachmentSN = AttachmentSN, TicketDtlSN = aTicketAttachment.TicketDtlSN });
    }
    public async Task DeleteTicketDtlAsync(decimal ticketDtlSN)
    {
        var connection = GetConnection();
        const string qury = "DELETE FROM CRM.TicketDtl " +
                            "WHERE TicketDtlSN = @TicketDtlSN";
        await connection.ExecuteAsync(qury, new { TicketDtlSN = ticketDtlSN });
    }

    public async Task<TicketAttachment> GetAttachmentFileAsync(long attachmentSN)
    {
        var connection = GetConnection();
        const string query = "SELECT" +
                             " FileSave" +
                             ",FileName" +
                             ",FileExtention" +
                             " FROM CRM.TicketAttachment" +
                             " WHERE TicketAttachmentSN = @SN";
        return await connection.QueryFirstOrDefaultAsync<TicketAttachment>(query, new { SN = attachmentSN });
    }
    public async Task<IEnumerable<JobOpportunity>> GetJobOpportunitiesAsync(int pageNumber, int pageSize)
    {
        var connection = GetConnection();
        const string query = @"SELECT [JobOpportunitySN]
                              ,[NationalID]
                              ,[FirstName]
                              ,[LastName]
                              ,[CityDs]
                              ,[Mobile]
                              ,[Email]
                              ,[FileName]
                              ,[FileSave]
                              ,[DefaultLanguageSN],
                        	  COUNT(*) OVER() AS TotalCount
                              FROM [PIS].[Web].[JobOpportunity]
                              ORDER BY JobOpportunitySN DESC
                              OFFSET @Offset ROWS
                              FETCH NEXT @PageSize ROWS ONLY ";

        var parameters = new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };

        return await connection.QueryAsync<JobOpportunity>(query,parameters);
    }
    public async Task AddJobOpportunityAsync(JobOpportunity newJobOpportunity)
    {
        var connection = GetConnection();
        const string query = "SELECT TOP 1 JobOpportunitySN FROM Web.JobOpportunity ORDER BY JobOpportunitySN DESC";
        var jobOpportunities = await connection.QueryAsync<JobOpportunity>(query);
        var lastTicket = jobOpportunities.FirstOrDefault();
        if (lastTicket != null)
        {
            newJobOpportunity.JobOpportunitySN = lastTicket.JobOpportunitySN + 1;
        }
        const string query2 = "INSERT INTO [PIS].[Web].[JobOpportunity] (JobOpportunitySN, NationalID, FirstName,LastName,CityDs,Mobile,Email,FileSave,FileName,DefaultLanguageSN,UserID_Name,Host_Name) \r\n" +
                              "VALUES (@JobOpportunitySN,@NationalID,@FirstName,@LastName,@CityDs,@Mobile,@Email,@FileSave,@FileName,@DefaultLanguageSN,@UserID_Name,@Host_Name)";
        await connection.ExecuteAsync(query2, newJobOpportunity);

    }

    public async Task<IEnumerable<JobTitle>> GetJobTitleAsync(int pageNumber, int pageSize)
    {
        var connection = GetConnection();
        const string query = @"SELECT [JobTitleSN]
                              ,[FatherJobTitleSN]
                              ,[JobTitleNo]
                              ,[JobTitleDs]
                              ,[JobTitleTypeSN]
                              ,[Description]
                              ,[DefaultLanguageSN]
                              ,GeneralStatusDtlDs
                              ,[OldID]
                              ,[IsShowInWeb]
                              ,COUNT(*) OVER() AS TotalCount
                               FROM [PIS].[General].[JobTitle]
                               INNER JOIN dbo._baVW_GeneralStatus ON JobTitleTypeSN = GeneralStatusDtlSN
                               ORDER BY JobTitleSN
                               OFFSET @Offset ROWS
                               FETCH NEXT @PageSize ROWS ONLY";
        var parameters = new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        };
        return await connection.QueryAsync<JobTitle>(query, parameters);
    }
    public async Task<IEnumerable<TicketDtl>> GetTicketDtlAsync(decimal SN)
    {
            var connection = GetConnection();
        const string query = @"SELECT * FROM CRM.TicketDtl
                               WHERE TicketSN = @Sn
                               ORDER BY TicketDtlSN DESC";
        return await connection.QueryAsync<TicketDtl>(query, new { Sn = SN });
    }

    public async Task<Person> GetPersonInfoById(decimal SN)
    {
        var connection = GetConnection();
        var patameters = new DynamicParameters();
        patameters.Add("@UserSN", SN, DbType.Decimal);
        var person = await connection.QueryFirstAsync<Person>("[General].[_SP_GetPersonInfoByUserSN]", patameters, commandType:CommandType.StoredProcedure);
        const string query2 = @"SELECT [PersonSN]
                                FROM [PIS].[General].[_VW_PersonFull]
                                WHERE UserSN = @UserSN";
        var personSN = await connection.QueryFirstAsync<decimal>(query2, new { UserSN = SN });
        person.PersonSN = personSN;
        return person;
    }
    
    public async Task<PersonAttachment> GetPersonAvatar(decimal SN)
    {
        var connection = GetConnection();
        const string query = @"SELECT [FileSave] , [FileExtention]
                               FROM [PIS].[General].[PersonAttachment]
                               WHERE PersonSN = @PersonSN AND AttachmentSubjectSN = 37";

        return await connection.QueryFirstAsync<PersonAttachment>(query, new { PersonSN = SN });

    }
}
