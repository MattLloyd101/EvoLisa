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
        new public void Init()
        {
            origin = new DnaPoint();
            origin.Init();

            Brush = new DnaBrush();
            Brush.Init();

            rx = ry = Tools.GetRandomNumber(0.0, 5.0);
            rotation = 0;
        }

        new public void Mutate(DnaDrawing drawing)
        {
            Brush.Mutate(drawing);

            MutateOrigin(drawing);
        }
    }
}
