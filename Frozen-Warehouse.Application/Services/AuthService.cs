using Frozen_Warehouse.Application.DTOs.Auth;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;
using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<string> AuthenticateAsync(LoginRequest request)
        {
            var user = await _userRepository.FindByUserNameAsync(request.UserName);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            // plain text comparison for testing only
            if (user.Password != request.Password) throw new UnauthorizedAccessException("Invalid credentials");

            return _tokenService.CreateToken(user.Id, user.Username, user.Role);
        }
    }
}