using System;
using System.Drawing;

namespace AdinaCardGame
{
    public class CardToGenerate
    {
        public int Index { get; set; }
        public string Card { get; set; }
        public Func<string, Image> CreateImage { get; set; }
        public string FilePrefix { get; set; }
    }
}