﻿using GraphX.Common.Models;
using System.Drawing;

namespace GraphX.Controls.DesignerExampleData
{
    internal sealed class EdgeDataExample<TVertex> : EdgeBase<TVertex>
    {
        public EdgeDataExample(TVertex source, TVertex target)
            : base(source, target)
        {

        }
        public EdgeDataExample(TVertex source, TVertex target, double weight)
            : base(source, target, weight)
        {

        }

        public string Text { get; set; }

        public override Color Color { get; } = Color.Black;
    }
}
