using Berk.WebApi.Data;
using Berk.WebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Berk.WebApi.Controllers
{//ok(200), not found(404), no content(204), created(201), badrequest(400)

	[EnableCors]
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly IProductRepository _productRepository;
		public ProductsController(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		[Authorize] //bunu dediğim an token alması gerekiyor ki listeleyebilmeli.
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _productRepository.GetAllAsync();
			return Ok(result);
		}

		//api/products/1, bu routetan gelen değer, ama şöyle yapsakyık /api/products?id = 1 deseydik bu FromQuery'den alınan olacaktı
		[Authorize(Roles ="Admin")]
		[HttpGet("{id}")]
		//[HttpGet("{getById}")] ,  [FromQuery]'den bekliyorum
		public async Task<IActionResult> GetProduct(int id) //routtean alıyor aslında
		{
			var data =  await _productRepository.GetById(id);
			if (data == null)
			{
				return NotFound(id);
			}
			return Ok(data);
		}

		[HttpPost] //[FromBody] vardır aslında Product'ın yanında, yanına [FromQuery] yazarsak üstteki gibi
		//api/products?id=1&name=telefon&
		public async  Task<IActionResult> CreateProduct([FromBody] Product product) //bodyden alıyor
		{
			var addedproduct = await _productRepository.CreateAsync(product);
			return  Created(string.Empty, addedproduct);
		}

		[HttpPut]
		public async Task< IActionResult> UpdateProduct(Product product) //bodyden alıyor
		{
			var checkproduct = _productRepository.GetById(product.Id);
			 
			if (checkproduct == null )
			{
				return NotFound(product.Id);
			}

			await _productRepository.UpdateAsync(product);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id) //from routedan alıyor
		{
			var deletedproduct = _productRepository.GetById(id);
			if (deletedproduct == null)
			{
				return NotFound(id);
			}

			await _productRepository.RemoveAsync(id);
			return NoContent();
		}

		//uploadFile  //api/products/upload
		[HttpPost("upload")]
		public async Task<IActionResult> Upload(IFormFile formfile)
		{
			var newName = Guid.NewGuid()+"." + Path.GetExtension(formfile.Name); //new name oluşturduk aslında
			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newName);
			var stream = new FileStream(path, FileMode.Create);
			await formfile.CopyToAsync(stream);

			return Created(string.Empty, formfile);

		}

		[HttpGet("[action]")]
		public IActionResult Test(/*[FromForm] string name, [FromHeader] string auth,*/ [FromServices]IDummyRepository dummyrepository) //bu attribute kavramına örnek için. Name datasını çekmek istiyorum formdan gelen mesela
		{
			//http mimarisine request ve response var. Requestimizin header'ı var bodysi var. Bunlara erişebiliriz aşağıdaki örnek gibi
			//[FromServices] ise mesela Iproductrep gibi dep. injectioanların geçtiğini düşünelim çok fazla şekilde o zaman bunu kullanabiliriz

			//var authetication = HttpContext.Request.Headers["auth"]; //bunnu yerine yukardaki şekliyle de ele alabilirim.

			//var name2 = HttpContext.Request.Form["name"];
			return Ok(dummyrepository.GetName());
		}



		

		





















		

		////api/products dediğim zaman bütün ürünler
		////api/products/1 dediğim zaman ya da ne olursa id'si 1 olan ürün gelmesini istiyorum.
		//[HttpGet]
		//public IActionResult GetProducts()
		//{
		//	return Ok(new[] { new { Name = "Bilgisayar", Price = 15000 }, new { Name = "Telefon", Price = 5000 } });
		//}

		//[HttpGet("{id}")] 
		//public IActionResult GetProduct(int id)
		//{
		//	return Ok(new[] { new { Name = "Bilgisayar", Price = 15000 }}) ;

		//}

	}
}
