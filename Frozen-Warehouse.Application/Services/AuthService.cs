using Frozen_Warehouse.Application.DTOs.Auth;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;
using Frozen_Warehouse.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

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

            // verify password using SHA256 matching seed
            string Hash(string plain)
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));
                return Convert.ToBase64String(bytes);
            }

            if (user.PasswordHash != Hash(request.Password)) throw new UnauthorizedAccessException("Invalid credentials");

            return _tokenService.CreateToken(user.Id, user.UserName, user.Role);
        }
    }
}