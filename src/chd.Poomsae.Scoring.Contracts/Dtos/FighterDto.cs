using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class FighterDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname => $"{this.Firstname} {this.Lastname.ToUpper()}";

        public FighterDto()
        {

        }
        public FighterDto(string firstName, string lastName)
        {
            this.Firstname = firstName;
            this.Lastname = lastName;
        }
    }
}
