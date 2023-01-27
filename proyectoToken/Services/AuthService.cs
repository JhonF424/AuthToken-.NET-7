using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using proyectoToken.Models;
using proyectoToken.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace proyectoToken.Services {
    public class AuthService : IAuthService {

        private readonly JwtdbcursoContext _context;
        private readonly IConfiguration _configuration;

        public AuthService( JwtdbcursoContext context, IConfiguration configuration ) {
            _context = context;
            _configuration = configuration;
        }

        private string GenerateToken( string UserID ) {
            var key = _configuration.GetValue<string>("JwtSettings:key");
            var keyBytes = Encoding.ASCII.GetBytes( key );

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserID));

            var TokenCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            var tokenDescriptor = new SecurityTokenDescriptor {
                // Remember Claims contains the UserID info
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = TokenCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var configToken = tokenHandler.CreateToken(tokenDescriptor);

            string createdToken = tokenHandler.WriteToken(configToken);

            return createdToken;
        }

        public async Task<AuthResponse> ReturnToken( AuthRequest auth ) {
            var foundUser = _context.Users.FirstOrDefault(x => x.Username == auth.Username && x.Pass == auth.Pass);

            if (foundUser == null) {
                return await Task.FromResult<AuthResponse>(null);
            }

            string createdToken = GenerateToken(foundUser.UserId.ToString());

            return new AuthResponse() {
                Token = createdToken, Result = true, Message = "Ok"
            };
        }
    }
}
