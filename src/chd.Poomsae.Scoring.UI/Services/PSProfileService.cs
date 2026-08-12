using Blazored.Modal.Services;
using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Contracts.Dtos.Authentication;
using DocumentFormat.OpenXml.Office2010.Drawing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class PSProfileService : ProfileService<Guid, int>
    {
        private readonly IModalService _modalService;
        private UserDto<Guid, int> _userDto;

        public PSProfileService(IModalService modalService)
        {
            this._modalService = modalService;
        }

        protected override async Task<UserPermissionDto<int>> GetPermissions(UserDto<Guid, int> dto, CancellationToken cancellationToken = default)
        {
            var perm = new UserPermissionDto<int>();
            var lst = new List<UserRightDto<int>>();
            lst.Add(new UserRightDto<int>()
            {
                Id = RightConstants.IS_ADMIN,
                Name = "Admin"
            });

            lst.Add(new UserRightDto<int>()
            {
                Id = RightConstants.IS_ALLOWED,
                Name = "Allowed"
            });

            lst.Add(new UserRightDto<int>()
            {
                Id = RightConstants.HAS_FIGHTERS,
                Name = "Has Fighters"
            });
            perm.UserRightLst = lst;
            return perm;
        }

        protected override sealed async Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            var time = DateTime.Today;

            if (this._userDto is null)
            {
                this._userDto = new UserDto<Guid,int>()
                {
                    FirstName = "Test",
                    LastName = "User",
                };
            }
            return this._userDto;
        }

    }
}
