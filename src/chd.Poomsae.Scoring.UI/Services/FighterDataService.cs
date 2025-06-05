using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class FighterDataService : IFighterDataService
    {
        private readonly List<FighterDto> _fighterLst;

        public List<FighterDto> Fighters => this._fighterLst;

        public FighterDataService()
        {
            this._fighterLst = new List<FighterDto>();
        }

        public void AddFighter(FighterDto fighter) => this._fighterLst.Add(fighter);
        public void RemoveFighter(FighterDto fighter) => this._fighterLst.Remove(fighter);
    }
}
