using CareerConnect.ChatService.Data;
using CareerConnect.ChatService.Hubs;
using CareerConnect.ChatService.Services;
using CareerConnect.Shared.Auth;
using CareerConnect.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatDb")));

builder.Services.AddCareerConnectJwtAuthentication(builder.Configuration);
builder.Services.AddCareerConnectExceptionHandling();

// The SignalR JS client can't set an Authorization header on the websocket handshake, so it sends
// the JWT as an "access_token" query string param instead - this reads it for the hub path only.
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var originalOnMessageReceived = options.Events?.OnMessageReceived;

    options.Events ??= new JwtBearerEvents();
    options.Events.OnMessageReceived = async context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;

        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
        {
            context.Token = accessToken;
        }

        if (originalOnMessageReceived is not null)
        {
            await originalOnMessageReceived(context);
        }
    };
});

builder.Services.AddScoped<IChatsService, ChatsService>();

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
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
