using MassTransit;
using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfileService.Consumers;
using ProfileService.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
            ClockSkew = TimeSpan.FromMinutes(2)
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

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("comment", false));

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ReceiveEndpoint("profile-user-registered", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });

        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });


        cfg.Publish<ProfileEdited>(x =>
        {
            x.ExchangeType = "fanout";
            x.Durable = true;
            x.AutoDelete = false;
        });
        cfg.Publish<UserRegistered>(x => x.ExchangeType = "fanout");
    });
});


builder.Services.AddDbContext<ProfileDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
