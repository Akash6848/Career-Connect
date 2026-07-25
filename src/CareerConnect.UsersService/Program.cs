using System.Text.Json.Serialization;
using CareerConnect.Shared.Auth;
using CareerConnect.Shared.Clients;
using CareerConnect.Shared.Exceptions;
using CareerConnect.UsersService.Data;
using CareerConnect.UsersService.Services;
using Microsoft.EntityFrameworkCore;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDb")));

builder.Services.AddCareerConnectJwtAuthentication(builder.Configuration);
builder.Services.AddCareerConnectExceptionHandling();

builder.Services.AddRefitClient<IFileServiceClient>()
    .ConfigureHttpClient(client =>
        client.BaseAddress = new Uri(builder.Configuration["Services:FileService"]!));

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();

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
