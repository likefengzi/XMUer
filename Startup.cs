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
            #region 开启jwt认证
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
                    ValidIssuer = "asd",//发行人
                    ValidateAudience = true,
                    ValidAudience = "fdf",//订阅人
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,//这个是缓冲过期时间，也就是说，即使我们配置了过期时间，这里也要考虑进去，过期时间+缓冲
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

                #region 加载xml
                // 为 Swagger JSON and UI设置xml文档注释路径
                //获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var basePath = AppContext.BaseDirectory;
                var xmls = Directory.GetFiles(basePath, "*.xml");
                foreach (var aXml in xmls)
                {
                    c.IncludeXmlComments(aXml);
                }
                #endregion

                #region jwt认证
                // 开启加权小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // 在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                // Jwt Bearer 认证
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization:Bearer {your JWT token},注意两者之间是一个空格",
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
