using Berk.WebApi.Data;
using Berk.WebApi.Interfaces;
using Berk.WebApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berk.WebApi
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

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt=> 
			{//token Htpps ile beraber �al���r.
				opt.RequireHttpsMetadata = false; //defalt g�steriliyor ama �nerilen true olmas�.
				opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidIssuer = "http://localhost",
					ValidAudience = "http://localhost",
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Berkberkberkbe1.")),
					//bizden security key istiyor. 3 tip seckey var.(simetrik,asimetrik,Hash)
					//Hash = datay� bir kere �ifreledi�imiz zaman onu art�k kaybederiz. Bir daha ��zmemiz m�mk�n de�ildir.
					//simetril = bir key vas�tas�yla. �ifreliyende �ifreyi ��zen de ayn� keye sahip olmas� gerekiyor. Bu key ile �ifreyi ��zebiliyoruz.
					//asimetrik = �ifreyi ��zen de �ifreleyenler de farkl� key kullan�yor. Biz tokene simetrik kullan�yoruz.
					ValidateIssuerSigningKey = true, //�ifre yani keyi validate et
					ValidateLifetime = true, //token� kontrol etsin zaman� ge�mi� mi ge�memi� mi diye. 
					ClockSkew = TimeSpan.Zero, //sunucu ile client aras�ndaki zaman farklar�n� d���nerek bir gecikme s�resi default olarak atan�yor.�n�ne ge�mek i�in s�f�rlad�k





				};
				 

			});
			

			services.AddDbContext<ProductContext>(opt =>
			{
				opt.UseSqlServer(Configuration.GetConnectionString("Local"));

			}); //bunu normalde businessta yapmal�y�z ama sorun yok

			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IDummyRepository, DummyRep>();


			services.AddCors(cors =>  //1- bunu ekledik Cors i�in
			{
				cors.AddPolicy("BerkCorsPolicy", opt =>
				 {
					 opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); //her t�rl� xyz.com'dan yani orign'den gelen de�eri kabul edecek.
					 //opt.WithOrigins("") dersem belirli sitedekilere Api a��lacak.

				 });

			});

			services.AddControllers().AddNewtonsoftJson(opt=> { opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });//Newtonofjson kurulumu i�in gerekliydi. Tek i�lem
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Berk.WebApi", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Berk.WebApi v1"));
			}

			app.UseStaticFiles();//wwwrootu d��ar� a�t�k :)

			app.UseRouting();

			app.UseCors("BerkCorsPolicy"); // bu da 2. hamle Cors i�in. En son hamle de Controller'�n n �st k�sm�na [EnableCors] dememiz. Bitti !

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
