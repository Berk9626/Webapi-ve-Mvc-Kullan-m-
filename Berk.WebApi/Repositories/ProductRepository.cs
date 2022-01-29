using Berk.WebApi.Data;
using Berk.WebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Berk.WebApi.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ProductContext _context;

		public ProductRepository(ProductContext context)
		{
			_context = context;
		}

		public async Task<Product> CreateAsync(Product product)
		{
			 await _context.Products.AddAsync(product);
			_context.SaveChanges();

			return product;
		}

		public async Task<List<Product>> GetAllAsync()
		{
			return await _context.Products.AsNoTracking().ToListAsync();
			
		}

		public async Task<Product> GetById(int id)
		{
			return await _context.Products.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
		}

		public async  Task RemoveAsync(int id)
		{
			var removedentity = await _context.Products.FindAsync(id);
			_context.Products.Remove(removedentity);
			await _context.SaveChangesAsync();

		}

		public async Task UpdateAsync(Product product)
		{
			var unchangedentity = await  _context.Products.FindAsync(product.Id);
			_context.Entry(unchangedentity).CurrentValues.SetValues(product);
			await _context.SaveChangesAsync();




		}
	}
}
