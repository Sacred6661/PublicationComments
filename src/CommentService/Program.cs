using CommentService.Consumers;
using CommentService.Data;
using CommentService.Data.Model;
using CommentService.Helpers;
using MassTransit;
using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var secret = builder.Configuration["Jwt:Key"] ?? "";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<ProfileEditedConsumer>();
    x.AddConsumer<CensorCheckedConsumer>();


    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("comment", false));

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ReceiveEndpoint("comment-user-registered", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });

        cfg.ReceiveEndpoint("comment-profile-edited", e =>
        {
            e.ConfigureConsumer<ProfileEditedConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });

        cfg.ReceiveEndpoint("comment-censor-checked", e =>
        {
            e.ConfigureConsumer<CensorCheckedConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });

        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<CommentAdded>(x =>
        {
            x.ExchangeType = "fanout";
            x.Durable = true;
            x.AutoDelete = false;
        });

        cfg.Publish<CommentAdded>(x => x.ExchangeType = "fanout");
    });
});


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2) // необов'язково
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasAnyRole", policy =>
            policy.RequireAssertion(context =>
                context.User.Identity?.IsAuthenticated == true &&
                context.User.Claims.Any(c => c.Type == ClaimTypes.Role)
            ));
});

builder.Services.AddControllers();

builder.Services.AddDbContext<CommentDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    await CommentStatusesInit.SeedStatusesAsync(dbContext);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
