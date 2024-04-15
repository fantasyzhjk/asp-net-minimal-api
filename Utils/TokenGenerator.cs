using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

public class TokenGenerator(IConfiguration config)
{
	private readonly byte[] _accessTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:AccessTokenSecret")!);
	private readonly string _issuer = config.GetValue<string>("Jwt:Issuer")!;
	private readonly string _audience = config.GetValue<string>("Jwt:Audience")!;

    public string GenerateAccessToken(Models.User user)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
				new[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(ClaimTypes.Role, user.Role),
				}
			),
			Expires = DateTime.UtcNow.AddMinutes(120),
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(_accessTokenSecret),
				SecurityAlgorithms.HmacSha256Signature
			),
			Issuer = _issuer,
			Audience = _audience,
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}