using CensorshipService.Services;
using MassTransit;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Messaging;
using CensorshipService.Consumers;


var builder = WebApplication.CreateBuilder(args);

// JWT
var secret = builder.Configuration["Jwt:Key"] ?? "";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

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

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CommentAddedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("censor", false));

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ReceiveEndpoint("censor-comment-added", e =>
        {
            e.ConfigureConsumer<CommentAddedConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });

        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });


        cfg.Publish<CensorChecked>(x =>
        {
            x.Durable = true;
            x.AutoDelete = false;
        });

    });
});

builder.Services.AddSingleton<ITextCensorshipService, TextCensorshipService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
