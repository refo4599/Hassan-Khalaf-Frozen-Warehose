using System;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        string CreateToken(int userId, string userName, string role);
    }
}