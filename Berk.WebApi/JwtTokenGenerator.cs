using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Berk.WebApi
{
	public class JwtTokenGenerator
	{//token oluşturmayı sağlayan methodumuz diyelim
		public string GenerateToken()
		{
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Berkberkberkbe1."));

			SigningCredentials credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256); //securitykey ve algoritma istiyor parametrelerde

			List<Claim> claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Role, "Member"));

			JwtSecurityToken token = new JwtSecurityToken(issuer:"http://localhost", audience: "http://localhost", claims:null,
				notBefore:DateTime.Now, expires:DateTime.Now.AddMinutes(1), signingCredentials: credentials); //issuer yayınlayan, kullanan audience, notbefore ne zamandan beri geçerli, şu andan, expires: ne kadar süre geçerli

			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			return handler.WriteToken(token);

			//signin
		}
	}
}
