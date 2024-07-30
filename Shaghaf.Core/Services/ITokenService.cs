using Shaghaf.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shaghaf.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}
