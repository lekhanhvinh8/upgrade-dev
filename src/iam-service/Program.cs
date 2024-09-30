


using IAMService;
using identityserver4_ef_template;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddAppIdentityServer();


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
//app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages()
            .RequireAuthorization();

app.MapControllers(); // This will map all the attribute-routed controllers (typically API controllers)


if (args.Contains("/seed"))
{
    Log.Information("Seeding database...");
    SeedData.EnsureSeedData(app);
    Log.Information("Done seeding database. Exiting.");
    return;
}

app.Run();
