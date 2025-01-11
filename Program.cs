using PIS_Web.Services;
using PIS_Web.Extensions;

var builder = WebApplication.CreateBuilder(args);


var mvcBuilder = builder.Services.AddRazorPages();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddSingleton<BuildConnectionStringService>();
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
try
{
    builder.Services.AddSingleton<DatabaseService>(sp =>
    {
        //var configuration = sp.GetRequiredService<IConfiguration>();
        //var connectionString = configuration.GetConnectionString("DefaultConnection");
        var iniFileService = sp.GetRequiredService<BuildConnectionStringService>();
        var connectionString = iniFileService.GetConnectionStringService("PIS");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Connection string is missing or null.");

        return new DatabaseService(connectionString);
    });

}
catch (Exception er)
{
    //context.Response.Redirect("/DataBaseError");
    Console.WriteLine($"Error: {er.Message}");
}

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Index";
        options.LogoutPath = "/Index";
        options.AccessDeniedPath = "/Privacy";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(options =>
    {

        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
        options.AddPolicy("ExpertOnly", policy => policy.RequireRole("Expert"));
    });

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch
    {
        context.Response.Redirect("/DataBaseError");
    }
});
//app.UseHttpsRedirection();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
