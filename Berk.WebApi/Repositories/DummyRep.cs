using Berk.WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Berk.WebApi.Repositories
{
	public class DummyRep : IDummyRepository
	{
		public string GetName()
		{
			return "Berk";
		}
	}
}
