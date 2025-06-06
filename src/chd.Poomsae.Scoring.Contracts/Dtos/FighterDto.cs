using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class FighterDto
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string DisplayName => $"{(this.Firstname.Length == 0 ? "" : this.Firstname.Substring(0, 1))}. {(this.Lastname ?? "").ToUpper()}";
        public Dictionary<DeviceDto, ScoreDto> Scores { get; set; } =[];
        public FighterDto()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
