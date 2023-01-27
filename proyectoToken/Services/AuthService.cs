using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using proyectoToken.Models;
using proyectoToken.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace proyectoToken.Services
{
    public class AuthService : IAuthService
    {

        private readonly JwtdbcursoContext _context;
        private readonly IConfiguration _configuration;

        public AuthService( JwtdbcursoContext context, IConfiguration configuration )
        {
            _context = context;
            _configuration = configuration;
        }

        private string GenerateToken( string UserID )
        {
            var key = _configuration.GetValue<string>("JwtSettings:key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, UserID));

            var TokenCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
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

        private string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            var refreshToken = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }

            return refreshToken;
        }

        private async Task<AuthResponse> SaveRefreshTokenHistory( int userId, string token, string refreshToken )
        {
            var refreshTokenHistory = new HistoryRefreshToken
            {
                UserId = userId,
                Token = token,
                RefreshToken = refreshToken,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(2) //<- Minutes just for example, it should be days 
            };

            await _context.HistoryRefreshTokens.AddAsync(refreshTokenHistory);
            await _context.SaveChangesAsync();

            return new AuthResponse { Token = token, RefreshToken = refreshToken, Result = true, Message = "Ok" };
        }

        public async Task<AuthResponse> ReturnToken( AuthRequest auth )
        {
            var foundUser = _context.Users.FirstOrDefault(x => x.Username == auth.Username && x.Pass == auth.Pass);

            if (foundUser == null)
            {
                return await Task.FromResult<AuthResponse>(null);
            }

            string createdToken = GenerateToken(foundUser.UserId.ToString());

            string createdRefreshToken = GenerateRefreshToken();

            //return new AuthResponse()
            //{
            //    Token = createdToken,
            //    Result = true,
            //    Message = "Ok"
            //}; 

            return await SaveRefreshTokenHistory(foundUser.UserId, createdToken, createdRefreshToken);
        }

        public async Task<AuthResponse> ReturnRefreshToken( RefreshTokenRequest refreshTokenRequest, int userID )
        {
            var foundRefreshToken = _context.HistoryRefreshTokens.FirstOrDefault(x =>
             x.Token == refreshTokenRequest.ExpiredToken &&
             x.RefreshToken == refreshTokenRequest.RefreshToken &&
             x.UserId == userID
            );

            if (foundRefreshToken == null)
            {
                return new AuthResponse { Result = false, Message = "Doesn't exist a valid Refresh Token" };
            }

            var createdRefreshToken = GenerateRefreshToken();
            var createdToken = GenerateToken(userID.ToString());

            return await SaveRefreshTokenHistory(userID, createdToken, createdRefreshToken);
        }
    }
}
