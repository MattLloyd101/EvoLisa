using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenArt.AST;
using GenArt.Classes;

namespace GenArt.Core.AST
{
    public class DnaCircle : DnaEllipse
    {

        public DnaCircle(Tools tool) : base(tool)
        {
        }

        override public void Init()
        {
            origin = new DnaPoint();
            origin.Init(tool);

            Brush = new DnaBrush();
            Brush.Init(tool);

            rx = ry = tool.GetRandomNumber(0, Settings.ActiveMaxCircleRadius);
            rotation = 0;
        }

        override public void Mutate(DnaVectorDrawing drawing)
        {
            if (tool.WillMutate(Settings.ActiveCircleWidthMutationRate))
            {
                rx = ry = mutateScalar(rx, 0, Settings.ActiveMaxCircleRadius, drawing);
            }

            Brush.Mutate(drawing);

            MutateOrigin(drawing);
        }

        override public DnaShape Clone()
        {
            var newCirc = new DnaCircle(tool);
            newCirc.origin = origin.Clone();
            newCirc.Brush = Brush.Clone();
            newCirc.rx = rx;
            newCirc.ry = ry;

            return newCirc;
        }
    }
}
