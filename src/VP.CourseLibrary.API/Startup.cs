using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using VP.CourseLibrary.API;
using VP.CourseLibrary.API.Services;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction =>
            {
                //TODO: ReturnHttpNotAcceptable Not working - fix. (Content Negotiation)
                setupAction.ReturnHttpNotAcceptable = true; //if unsupported response type is requested (false by default)
                //setupAction.Filters.Add(new AuthorizeFilter()); //uncomment this if we want [Authorize] attribute by default on all controllers
            })
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver
                    = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            //making the API comply with https://tools.ietf.org/html/rfc7807
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    //create a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                        context.HttpContext,
                        context.ModelState);

                    //add additional info not added by default
                    problemDetails.Detail = "See the errors field for details.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    //find out which status code to use
                    var actionExecutingContext = context as ActionExecutingContext;

                    //if there are modelstate errors & all arguments were correctly 
                    //found/parsed we're dealing with validataion errors
                    if (context.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count)
                    {
                        problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors occured.";
                        
                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    //if one of the arguments wasn't correctly found / couldn't be parsed
                    //we're dealing with null/unparseable input
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occured.";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });

            services
                .AddAuthentication(x => 
                {
                    x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //x.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie() //"account/login" is the default path
                .AddCookie(ExternalAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle(x => 
                {
                    //using "Manage Client Secrets" in VisualStudio, so we would not be checking them in source controll
                    x.SignInScheme = ExternalAuthenticationDefaults.AuthenticationScheme;
                    x.ClientId = Configuration["Google:ClientId"];
                    x.ClientSecret = Configuration["Google:ClientSecret"];
                });
            //.AddCookie(o => o.LoginPath = "account/signin"); if we don't like the default path


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler(appBuilder => 
                {
                    appBuilder.Run(async context => 
                    {
                        //TODO: Log Exception
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
