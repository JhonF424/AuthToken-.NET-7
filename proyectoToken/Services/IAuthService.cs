using proyectoToken.Models.Custom;

namespace proyectoToken.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> ReturnToken( AuthRequest auth );
        Task<AuthResponse> ReturnRefreshToken( RefreshTokenRequest refreshTokenRequest, int userID);
    }
}
