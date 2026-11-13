using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Services;

namespace NovaScale.Platform.API.IdentityAccess.Infrastructure.Security;

public class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public string Generate(User user)
    {
        var key = configuration["Authentication:SigningKey"]!;
        var token = new JwtSecurityToken(configuration["Authentication:Issuer"], configuration["Authentication:Audience"],
            [new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim("organization_id", user.OrganizationId.ToString()),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())],
            expires: DateTime.UtcNow.AddMinutes(configuration.GetValue("Authentication:ExpirationMinutes", 60)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
