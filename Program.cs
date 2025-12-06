using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VolunteerApp1.Data;
using VolunteerApp1.Mapper;
using Microsoft.AspNetCore.Hosting; // Added this using directive for IWebHostEnvironment

namespace VolunteerApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var a = "Server=localhost;uid=xyzxyz;pwd=2020;Database=VolunteerDatabase;TrustServerCertificate=True";

            builder.Services.AddControllers();
            builder.Services.AddDbContext<VolunteerDbContext>(option => option.UseSqlServer(a));
            builder.Services.AddAutoMapper(typeof(VolunteerProfile));
            builder.Services.AddScoped<Repository.IVolunteer, Repository.VolunteerRepository>();

            // Add IWebHostEnvironment service (already included by default in the builder, but good practice to note its usage)
            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Security definition - must use Scheme = "bearer" lowercase
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter 'Bearer' [space] and your JWT token.\n\nExample: \"Bearer eyJhb...\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // Security requirement - tells swagger to send the Authorization header
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        },
        Array.Empty<string>()
    }
});
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // The correct place for UseStaticFiles()
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}

//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;
//using VolunteerApp1.Data;
//using VolunteerApp1.Mapper;
//namespace VolunteerApp1
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            var a= "Server=localhost;uid=xyzxyz;pwd=2020;Database=VolunteerDatabase;TrustServerCertificate=True";

//            builder.Services.AddControllers();
//            builder.Services.AddDbContext<VolunteerDbContext>(option => option.UseSqlServer(a));
//            builder.Services.AddAutoMapper(typeof(VolunteerProfile));
//            builder.Services.AddScoped<Repository.IVolunteer, Repository.VolunteerRepository>();
//            var jwtSettings = builder.Configuration.GetSection("Jwt");
//            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

//            builder.Services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//            .AddJwtBearer(options =>
//            {
//                options.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidateLifetime = true,
//                    ValidateIssuerSigningKey = true,
//                    ValidIssuer = jwtSettings["Issuer"],
//                    ValidAudience = jwtSettings["Audience"],
//                    IssuerSigningKey = new SymmetricSecurityKey(key)
//                };
//            });


//            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();
//            builder.Services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

//                // Security definition - must use Scheme = "bearer" lowercase
//                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Description = "Enter 'Bearer' [space] and your JWT token.\n\nExample: \"Bearer eyJhb...\"",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header,
//                    Type = SecuritySchemeType.Http,
//                    Scheme = "bearer",
//                    BearerFormat = "JWT"
//                });

//                // Security requirement - tells swagger to send the Authorization header
//                c.AddSecurityRequirement(new OpenApiSecurityRequirement
//{
//    {
//        new OpenApiSecurityScheme
//        {
//            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
//        },
//        Array.Empty<string>()
//    }
//});
//            });
//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("AllowAll", policy =>
//                    policy.AllowAnyOrigin()



//                    .AllowAnyMethod()
//                          .AllowAnyHeader());
//            });

//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();

//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.UseCors("AllowAll");

//            app.MapControllers();

//            app.Run();
//        }
//    }
//}
