using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToDisplayString(this decimal value) => value >= 10 ? value.ToString("n0",CultureInfo.InvariantCulture) : value.ToString("n1", CultureInfo.InvariantCulture);
        public static string ToDisplayString(this decimal? value) => !value.HasValue ? "-" : value.Value.ToDisplayString();
    }
}
