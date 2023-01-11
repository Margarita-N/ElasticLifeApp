using ElasticLifeApp.Services;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DataRepository>();

var connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200"));

connectionSettings.ServerCertificateValidationCallback((sender, cert, chain, errors) => true)
                  .EnableDebugMode();

builder.Services.AddSingleton<IElasticClient>(new ElasticClient(connectionSettings));
builder.Services.AddScoped<ElasticService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
