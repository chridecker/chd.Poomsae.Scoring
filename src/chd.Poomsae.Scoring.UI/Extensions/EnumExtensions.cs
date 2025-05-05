using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Extensions
{
    public static class EnumExtensions
    {
        public static string GetCssClass(this EScoringButtonColor color) => color switch
        {
            EScoringButtonColor.Blue => "button-blue",
            EScoringButtonColor.Red => "button-red",
            _ => string.Empty
        };

        public static string GetCssClass(this EScoringButtonDirection direction) => direction switch
        {
            EScoringButtonDirection.Left => "button-left",
            EScoringButtonDirection.Right => "button-right",
            _ => string.Empty
        };
    }
}
