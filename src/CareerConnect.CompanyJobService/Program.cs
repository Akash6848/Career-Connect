using System.Text.Json.Serialization;
using CareerConnect.CompanyJobService.Data;
using CareerConnect.CompanyJobService.Services;
using CareerConnect.Shared.Auth;
using CareerConnect.Shared.Clients;
using CareerConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddDbContext<CompanyJobDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CompanyJobDb")));

builder.Services.AddCareerConnectJwtAuthentication(builder.Configuration);
builder.Services.AddCareerConnectExceptionHandling();

builder.Services.AddRefitClient<IFileServiceClient>()
    .ConfigureHttpClient(client =>
        client.BaseAddress = new Uri(builder.Configuration["Services:FileService"]!));

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
