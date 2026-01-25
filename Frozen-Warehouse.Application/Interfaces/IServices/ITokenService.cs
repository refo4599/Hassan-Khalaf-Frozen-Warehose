using System;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        string CreateToken(Guid userId, string userName, string role);
    }
}