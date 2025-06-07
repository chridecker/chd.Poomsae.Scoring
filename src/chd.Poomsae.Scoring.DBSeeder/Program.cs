using chd.Poomsae.Scoring.Persistence;
using System;

using var db = new ScoringContext(new Microsoft.EntityFrameworkCore.DbContextOptions<ScoringContext>());
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
db.Fighters.Add(new chd.Poomsae.Scoring.Contracts.Dtos.FighterDto
{
    Firstname = "Max",
    Lastname = "Mustermann"
});
db.SaveChanges();
