using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface ITokenService
    {
        string GenerateLicenseToken(PSUserDto user, DateTime expiryDate);
        (PSUserDto User, DateTime valid) ValidateLicenseToken(string token);
    }
}
