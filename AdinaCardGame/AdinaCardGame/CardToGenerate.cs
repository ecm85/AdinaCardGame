using System;
using Svg;

namespace AdinaCardGame
{
    public class CardToGenerate
    {
        public int Index { get; set; }
        public string Card { get; set; }
        public Func<string, SvgDocument> CreateImage { get; set; }
        public string FilePrefix { get; set; }
    }
}