using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class RoundDto
    {
        public Guid Id { get; set; }

        public Guid FighterId { get; set; }
        public DateTime Created { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<SavedScoreDto> Scores { get; set; } = [];
    }
}
