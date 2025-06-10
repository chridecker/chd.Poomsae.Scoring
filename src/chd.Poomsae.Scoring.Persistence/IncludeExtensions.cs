using chd.Poomsae.Scoring.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Persistence
{
    public static class IncludeExtensions
    {
        public static IQueryable<FighterDto> LoadFighters(this IQueryable<FighterDto> fighters)
            => fighters.Include(i => i.Rounds).ThenInclude(i => i.Scores);
    }
}
