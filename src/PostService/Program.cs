using MassTransit;
using Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PostService.Consumers;
using PostService.Data;
using PostService.Helpers;
using PostService.Services;
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

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<ProfileEditedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("post", false));

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ReceiveEndpoint("post-user-registered", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });

        cfg.ReceiveEndpoint("post-profile-edited", e =>
        {
            e.ConfigureConsumer<ProfileEditedConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, 500));
        });


        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});


builder.Services.AddDbContext<PostDbContexts>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPublication, PublicationService>();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
