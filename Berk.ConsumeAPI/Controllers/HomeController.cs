using Berk.ConsumeAPI.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Berk.ConsumeAPI.Controllers
{
	public class HomeController: Controller
	{ //17643
		private readonly IHttpClientFactory _httpClientFactory;

		public HomeController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index() 
		{
			var client = _httpClientFactory.CreateClient();
			var responseMessage = await client.GetAsync("http://localhost:5000/api/products"); //api productsları çekmek istiyorum.

			if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK) //message'in statuscode'u apilerden dönen okeyse
			{

				var jsondata = await responseMessage.Content.ReadAsStringAsync(); //responseMessage'ın contentini okuyarak bua erişebiliyoruz.
				//elimde json data var ve bunu bir objeye .evirmem lazım. Bunun için managepacketeNewtonJson indirdik.
				var result = JsonConvert.DeserializeObject<List<ProductResponseModel>>(jsondata); //Json data içindekileri Productresponsemodele deser. ettik
				return View(result);
	
			}

			else
			{
				return View(null); 
			}
			
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(ProductResponseModel model)
		{
			var client = _httpClientFactory.CreateClient();
			var jsondata = JsonConvert.SerializeObject(model); //modeli json tipine çevirdik
			StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
			 var responseMessage = await client.PostAsync("http://localhost:5000/api/products", content ); //bunu ilk yazdığımda anladım ki parametreden gelen modelin httpcontente dönmesi lazım

			if (responseMessage.IsSuccessStatusCode) //200'le başlayan herhang bir kodsa
			{
				return RedirectToAction("Index");
			}
			else
			{
				TempData["errorMessage"] = $"Bir hata ile karşılaşıldı. Hata kodu:  {(int)responseMessage.StatusCode}"; 
				return View(model);
			}

			
		}
		public async Task<IActionResult> Update(int id)
		{
			var client = _httpClientFactory.CreateClient();
			var responseMessage = await client.GetAsync($"http://localhost:5000/api/products/{id}");

			if (responseMessage.IsSuccessStatusCode)
			{
				var jsonData = await responseMessage.Content.ReadAsStringAsync();
				var data = JsonConvert.DeserializeObject<ProductResponseModel>(jsonData);
				return View(data);

			}
			else
			{
				return View(null);
			}
				
		}
		[HttpPost]
		public async Task<IActionResult> Update(ProductResponseModel model)
		{
			var client = _httpClientFactory.CreateClient();
			var jsonData = JsonConvert.SerializeObject(model);
			var content = new StringContent(jsonData, Encoding.UTF8,"application/json");
			var responseMessage = await client.PutAsync("http://localhost:5000/api/products", content);

			if (responseMessage.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");

			}
			else
			{
				
				return View(model);
			}	
		}

		public async Task<IActionResult> Remove(int id)
		{
			var client = _httpClientFactory.CreateClient();
			 await client.DeleteAsync($"http://localhost:5000/api/products/{id}");
			return RedirectToAction("Index");

		}

		public IActionResult Upload()
		{
			return View();
		}

		[HttpPost]
		public async Task< IActionResult> Upload(IFormFile file)
		{
			var client = _httpClientFactory.CreateClient();

			var stream = new MemoryStream(); //
			await file.CopyToAsync(stream);
			var bytes = stream.ToArray();

			ByteArrayContent content = new ByteArrayContent(bytes); //file'ı bytea çevirmem lazım.
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType); 
			MultipartFormDataContent formdata = new MultipartFormDataContent();
			formdata.Add(content,"formFile", file.FileName);

			await client.PostAsync("http://localhost:5000/api/products/upload", formdata);
			return RedirectToAction("Index");
		}
	}
}
