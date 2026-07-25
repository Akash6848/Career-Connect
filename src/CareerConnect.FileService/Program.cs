using CareerConnect.FileService.Configuration;
using CareerConnect.FileService.Services;
using CareerConnect.Shared.Exceptions;
using CloudinaryDotNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCareerConnectExceptionHandling();

var cloudinaryOptions = builder.Configuration
    .GetSection(CloudinaryOptions.SectionName)
    .Get<CloudinaryOptions>()
    ?? throw new InvalidOperationException("Missing required 'Cloudinary' configuration section");

builder.Services.AddSingleton(new Cloudinary(cloudinaryOptions.Url));
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
