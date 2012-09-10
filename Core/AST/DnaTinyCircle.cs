using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenArt.AST;
using GenArt.Classes;

namespace GenArt.Core.AST
{
    class DnaTinyCircle : DnaCircle
    {
        public DnaTinyCircle(Tools tool) : base(tool)
        {
        }

        new public void Init()
        {
            origin = new DnaPoint();
            origin.Init(tool);

            Brush = new DnaBrush();
            Brush.Init(tool);

            rx = ry = tool.GetRandomNumber(0.0, 5.0);
            rotation = 0;
        }

        new public void Mutate(DnaVectorDrawing drawing)
        {
            Brush.Mutate(drawing);

            MutateOrigin(drawing);
        }
    }
}
