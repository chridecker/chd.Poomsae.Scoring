using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Extensions
{
    public static class BLEExtensions
    {
        public static Guid ToGuidId(this string text) => Guid.Parse($"0000{text}-0000-1000-8000-00805F9B34FB");
        public static string ToGuidString(this string text) => text.ToGuidId().ToString();
    }
}
