using chd.Poomsae.Scoring.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Persistence
{
    public class ScoringContext : DbContext
    {
        public const string DB_FILE = "chdPoomsaeScoring.db";

        public DbSet<FighterDto> Fighters { get; set; }

        public ScoringContext() : base()
        {

        }

        public ScoringContext(DbContextOptions<ScoringContext> options) : base(options)
        {
            //Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Filename={DB_FILE}");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
