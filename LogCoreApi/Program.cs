using LogCoreApi.Data; 
using LogCoreApi.Entities; 
using LogCoreApi.Filters; 
using FluentValidation.AspNetCore; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; 
using Serilog; 
using System.Text; 


var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


builder.Host.UseSerilog();


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelStateValidationFilter>(); 
})
.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<Program>(); 
});


builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequiredLength = 8; 
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false; 
        options.Password.RequireLowercase = false; 
        options.Password.RequireDigit = true; 
    })
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<AppDbContext>() 
    .AddSignInManager(); 



var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, 
            ValidateAudience = true, 
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true, 

            ValidIssuer = jwtIssuer, 
            ValidAudience = jwtAudience, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)) 
        };
    });

builder.Services.AddAuthorization(); 


builder.Services.AddScoped<LogCoreApi.Services.Auth.TokenService>();


builder.Services.AddAutoMapper(typeof(LogCoreApi.Mapping.NoteMappingProfile));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSerilogRequestLogging();


app.UseMiddleware<LogCoreApi.Middlewares.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();
app.Run();
