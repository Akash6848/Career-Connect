using System.Text.Json.Serialization;
using CareerConnect.PostService.Data;
using CareerConnect.PostService.Services;
using CareerConnect.Shared.Auth;
using CareerConnect.Shared.Clients;
using CareerConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PostsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostsDb")));

builder.Services.AddCareerConnectJwtAuthentication(builder.Configuration);
builder.Services.AddCareerConnectExceptionHandling();

builder.Services.AddRefitClient<IFileServiceClient>()
    .ConfigureHttpClient(client =>
        client.BaseAddress = new Uri(builder.Configuration["Services:FileService"]!));

builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPostLikeService, PostLikeService>();

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
