
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SimpleEmailV2;
using Amazon.EmailService.Infrastructure;
using Amazon.EmailService.Infrastructure.Appsettings;
using Amazon.EmailService.Services;


namespace Amazon.EmailService
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

            services.AddMediatR(typeof(Startup).Assembly, typeof(IIdentity).Assembly);

            services.Configure<AWSConfigs>(Configuration.GetSection("AwsConfig"));
            services.Configure<AWSEmailServiceOptions>(Configuration.GetSection("AWSEmailServiceOptions"));

            AddAWSSimpleEmailService(services, Configuration.GetAWSConfigs("AwsConfig"));
            services.Configure<CommonSettings>(Configuration.GetSection("CommonSettings"));

            services.AddScoped<IAWSEmailService, AWSEmailService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IEmailService, Amazon.EmailService.Services.EmailService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Email Service API", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {
                        "Bearer",
                        new List<string>()
                    },
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.RegisterNamedHttpClientFactories(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseApiExceptionHandler();
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                loggerFactory.AddConsole();
            }
            else
            {
                loggerFactory.AddAWSProvider(this.Configuration.GetAWSLoggingConfigSection("AWSLogging"));
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email Service API V1");
            });
            app.UseMvc();
        }        

        public void AddAWSSimpleEmailService(IServiceCollection services, AWSConfigs options)
        {
            var config = new AmazonSimpleEmailServiceV2Config();

            if (options.Region != null)
            {
                config.RegionEndpoint = options.Region;
            }
            else
            {
                config.ServiceURL = options.DefaultClientConfig.ServiceURL;
            }

            if (!string.IsNullOrEmpty(options.Profile))
            {
                var chain = new CredentialProfileStoreChain(options.ProfilesLocation);
                chain.TryGetAWSCredentials(options.Profile, out AWSCredentials awsCredential);

                var client = new AmazonSimpleEmailServiceV2Client(awsCredential, config);

                services.AddSingleton<IAmazonSimpleEmailServiceV2>(se =>
                {
                    return client;
                });
            }
            else
            {
                var client = new AmazonSimpleEmailServiceV2Client(config.AccessKey,
                                                                  config.SerectKey,
                                                                  config);

                services.AddSingleton<IAmazonSimpleEmailServiceV2>(se =>
                {
                    return client;
                });
            }
        }
    }
}
