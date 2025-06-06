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
        const string DB_FILE = "chdPoomsaeScoring.db";

        public DbSet<FighterDto> Fighters { get; set; }

        public ScoringContext(DbContextOptions<ScoringContext> options) : base(options)
        {
            Database.EnsureCreated(); 
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Filename={DB_FILE}");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FighterDto>().Ignore(i => i.Scores);

            base.OnModelCreating(modelBuilder);
        }
    }
}
