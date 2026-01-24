using Frozen_Warehouse.Application.DTOs.Auth;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginRequest request);
    }
}