using chd.UI.Base.Contracts.Interfaces.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface ILicenseTokenProfileService : IProfileService<Guid, int>
    {
        Task RenewLicense(CancellationToken cancellationToken = default);
    }
}
