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
			{//token Htpps ile beraber çalýþýr.
				opt.RequireHttpsMetadata = false; //defalt gösteriliyor ama çnerilen true olmasý.
				opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidIssuer = "http://localhost",
					ValidAudience = "http://localhost",
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Berkberkberkbe1.")),
					//bizden security key istiyor. 3 tip seckey var.(simetrik,asimetrik,Hash)
					//Hash = datayý bir kere þifrelediðimiz zaman onu artýk kaybederiz. Bir daha çözmemiz mümkün deðildir.
					//simetril = bir key vasýtasýyla. Þifreliyende þifreyi çözen de ayný keye sahip olmasý gerekiyor. Bu key ile þifreyi çözebiliyoruz.
					//asimetrik = þifreyi çözen de þifreleyenler de farklý key kullanýyor. Biz tokene simetrik kullanýyoruz.
					ValidateIssuerSigningKey = true, //þifre yani keyi validate et
					ValidateLifetime = true, //tokený kontrol etsin zamaný geçmiþ mi geçmemiþ mi diye. 
					ClockSkew = TimeSpan.Zero, //sunucu ile client arasýndaki zaman farklarýný düþünerek bir gecikme süresi default olarak atanýyor.Önüne geçmek için sýfýrladýk





				};
				 

			});
			

			services.AddDbContext<ProductContext>(opt =>
			{
				opt.UseSqlServer(Configuration.GetConnectionString("Local"));

			}); //bunu normalde businessta yapmalýyýz ama sorun yok

			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IDummyRepository, DummyRep>();


			services.AddCors(cors =>  //1- bunu ekledik Cors için
			{
				cors.AddPolicy("BerkCorsPolicy", opt =>
				 {
					 opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); //her türlü xyz.com'dan yani orign'den gelen deðeri kabul edecek.
					 //opt.WithOrigins("") dersem belirli sitedekilere Api açýlacak.

				 });

			});

			services.AddControllers().AddNewtonsoftJson(opt=> { opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });//Newtonofjson kurulumu için gerekliydi. Tek iþlem
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

			app.UseStaticFiles();//wwwrootu dýþarý açtýk :)

			app.UseRouting();

			app.UseCors("BerkCorsPolicy"); // bu da 2. hamle Cors için. En son hamle de Controller'ýn n üst kýsmýna [EnableCors] dememiz. Bitti !

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
