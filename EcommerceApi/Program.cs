using Microsoft.EntityFrameworkCore;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Core.IRepositories;
using Ecommerce.Infrastructure.Repositories;
using Ecommerce.Api.Mapping_Profiles;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Ecommerce.Core.IRepositories.IServices;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
namespace EcommerceApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            options.CacheProfiles.Add("DefaultCache" ,
            new Microsoft.AspNetCore.Mvc.CacheProfile()
            {
                Duration = 30 , 
                Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any
            }));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //connecting on the database via connection strings
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")).UseLazyLoadingProxies();
                };
            });

            //implementing the repo when calling the repo interface
            builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            builder.Services.AddScoped(typeof(ICategoriesRepository), typeof(CategoriesRepository));
            builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
            builder.Services.AddScoped(typeof(IGenericRepository<>) , typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddScoped(typeof(IUsersRepository), typeof(UsersRepository));
            builder.Services.AddScoped(typeof(IRolesRepository) , typeof(RolesRepository));
            builder.Services.AddScoped(typeof(IOrderDetailsRepository), typeof(OrderDetailsRepository));
            builder.Services.AddScoped(typeof(ITokenService) , typeof(TokenService));
            builder.Services.AddScoped(typeof(IFilesService), typeof(FilesService));
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAuthentication();
            builder.Services.AddTransient(typeof(IEmailService), typeof(EmailService));
            builder.Services.AddIdentity<LocalUser , IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(x => x.Value.Errors.Count() > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                    var ApiValidtionResponse = new ApiValidationResponse(StatusCode: 400, Errors: errors);

                    return new BadRequestObjectResult(ApiValidtionResponse);
                };
            });


            var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
                };
            });

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory() , "UploadedFiles")),
                RequestPath = "/files"
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
