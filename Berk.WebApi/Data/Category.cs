using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Berk.WebApi.Data
{
	//api/categories/1/products //1 numaralı kategorilerin ürünleri
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public List<Product> Products { get; set; }
	}
}
