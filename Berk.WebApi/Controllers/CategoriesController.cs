using Berk.WebApi.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Berk.WebApi.Controllers
{

	[EnableCors]
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriesController: Controller
	{
		private readonly ProductContext _context;
		public CategoriesController(ProductContext context)
		{
			_context = context;

		}

		[HttpGet("{id}/products")]
		public IActionResult GetWithProducts(int id)
		{
			//eagerloading ile kategorilere productları ekleyeceğiz. Kategoriye onun içerisindeki productları ekle
			var data = _context.Categories.Include(x => x.Products).SingleOrDefault(x => x.Id == id);

			if (data == null)
			{
				return NotFound();
			}

			return Ok(data);

		}

	}
}
