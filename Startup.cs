using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMUer.Models;
using XMUer.SetupService;
using XMUer.Utility;

namespace XMUer
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
            
            var connStr = ConfigHelper.GetConnectionString("ConnectionStrings");
            Console.WriteLine(connStr);
            Console.WriteLine("*****");
            var sqlConnection = Configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<DATABASEContext>(option => option.UseSqlServer(sqlConnection));
            services.AddMvc();
            services.AddAuthentication();
            

            //services.AddSingleton(new AppSettings(Configuration));
            /*
            #region ����jwt��֤
            var symmetricKeyAsBase64 = "!@#123";
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = "asd",//������
                    ValidateAudience = true,
                    ValidAudience = "fdf",//������
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,//����ǻ������ʱ�䣬Ҳ����˵����ʹ���������˹���ʱ�䣬����ҲҪ���ǽ�ȥ������ʱ��+����
                    RequireExpirationTime = true,
                };

            });
            #endregion
            */
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "XMUer", Version = "v1" });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "XMUer", Version = "v1" });

                #region ����xml
                // Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
                //��ȡӦ�ó�������Ŀ¼�����ԣ����ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·����
                var basePath = AppContext.BaseDirectory;
                var xmls = Directory.GetFiles(basePath, "*.xml");
                foreach (var aXml in xmls)
                {
                    c.IncludeXmlComments(aXml);
                }
                #endregion

                #region jwt��֤
                // ������ȨС��
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // ��header�����token�����ݵ���̨
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                // Jwt Bearer ��֤
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization:Bearer {your JWT token},ע������֮����һ���ո�",
                });
                #endregion
            });
            services.AddAuthorizationSetup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "XMUer v1"));
            }

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
