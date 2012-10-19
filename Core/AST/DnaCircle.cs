using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenArt.AST;
using GenArt.Classes;

namespace GenArt.Core.AST
{
    class DnaCircle : DnaEllipse
    {

        new public void Init()
        {
            origin = new DnaPoint();
            origin.Init();

            Brush = new DnaBrush();
            Brush.Init();

            rx = ry = Tools.GetRandomNumber(0, Settings.ActiveMaxCircleRadius);
            rotation = 0;
        }

        new public void Mutate(DnaDrawing drawing)
        {
            if (Tools.WillMutate(Settings.ActiveCircleWidthMutationRate))
            {
                rx = ry = mutateScalar(rx, 0, Settings.ActiveMaxCircleRadius, drawing);
            }

            Brush.Mutate(drawing);

            MutateOrigin(drawing);
        }
    }
}
