using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenArt.AST;
using GenArt.Classes;
using System.Drawing;

namespace GenArt.Core.AST
{
    [Serializable]
    class DnaEllipse : DnaShape
    {
        public DnaBrush Brush { get; set; }

        public DnaPoint origin { get; set; }
        public double rx { get; set; }
        public double ry { get; set; }

        public double rotation { get; set; }


        public int maxX()
        {
            return origin.X + (int)rx;
        }

        public int maxY()
        {
            return origin.Y + (int)ry;
        }

        public void Init()
        {
            origin = new DnaPoint();
            origin.Init();

            Brush = new DnaBrush();
            Brush.Init();

            rx = Tools.GetRandomNumber(0, Settings.ActiveMaxCircleRadius);
            ry = Tools.GetRandomNumber(0, Settings.ActiveMaxCircleRadius);
            rotation = Tools.GetRandomNumber(-99.0, 99.0);
        }

        public string toXML()
        {
            String xml = "";
            float rad2deg = 57.2957795f;

            xml += "<ellipse cx=\"" + (rx/4 + origin.X) + "\" cy=\"" + (ry/4 + origin.Y) + "\" rx=\"" + rx/2 + "\" ry=\"" + ry/2 + "\" style=\"" + Brush.getBrushString() + "\"/>";

            
            return xml;
        }

        public DnaShape Clone()
        {
            var newCirc = new DnaEllipse();
            newCirc.origin = origin.Clone();
            newCirc.Brush = Brush.Clone();
            newCirc.rx = rx;
            newCirc.ry = ry;

            return newCirc;
        }

        public double mutateScalar(double scalar, double min,  double max, DnaDrawing drawing)
        {
            if (Tools.WillMutate(Settings.ActiveCircleSizeMidMutationRate))
            {
                drawing.SetDirty();
                return Math.Min(Math.Max(min, scalar +
                                    Tools.GetRandomNumber(-Settings.ActiveCircleSizeRangeMid,
                                                           Settings.ActiveCircleSizeRangeMid)), max);
            }

            if (Tools.WillMutate(Settings.ActiveCircleSizeMinMutationRate))
            {
                drawing.SetDirty();
                return Math.Min(Math.Max(min, scalar +
                                 Tools.GetRandomNumber(-Settings.ActiveCircleSizeRangeMin,
                                                       Settings.ActiveCircleSizeRangeMin)), max);
            }

            return scalar;
        }

        public void Mutate(DnaDrawing drawing)
        {
            if (Tools.WillMutate(Settings.ActiveCircleWidthMutationRate))
            {
                rx = mutateScalar(rx, 0, Settings.ActiveMaxCircleRadius, drawing);
            }

            if (Tools.WillMutate(Settings.ActiveCircleHeightMutationRate))
            {
                ry = mutateScalar(ry, 0, Settings.ActiveMaxCircleRadius, drawing);
            }

            if (Tools.WillMutate(Settings.ActiveRotationMutationRate))
            {
                rotation = mutateScalar(rotation, -999.0, 999.0, drawing);
            }

            Brush.Mutate(drawing);

            MutateOrigin(drawing);
        }

        public void MutateOrigin(DnaDrawing drawing)
        {
            if (Tools.WillMutate(Settings.ActiveMovePointMaxMutationRate))
            {
                origin.X = Tools.GetRandomNumber(-Settings.ActiveMaxCircleRadius, Tools.MaxWidth + Settings.ActiveMaxCircleRadius);
                origin.Y = Tools.GetRandomNumber(-Settings.ActiveMaxCircleRadius, Tools.MaxHeight + Settings.ActiveMaxCircleRadius);
                drawing.SetDirty();
            }

            if (Tools.WillMutate(Settings.ActiveMovePointMidMutationRate))
            {
                origin.X =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.X +
                                 Tools.GetRandomNumber(-Settings.ActiveMovePointRangeMid,
                                                       Settings.ActiveMovePointRangeMid)), Tools.MaxWidth + Settings.ActiveMaxCircleRadius);
                origin.Y =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.Y +
                                 Tools.GetRandomNumber(-Settings.ActiveMovePointRangeMid,
                                                       Settings.ActiveMovePointRangeMid)), Tools.MaxHeight + Settings.ActiveMaxCircleRadius);
                drawing.SetDirty();
            }

            if (Tools.WillMutate(Settings.ActiveMovePointMinMutationRate))
            {
                origin.X =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.X +
                                 Tools.GetRandomNumber(-Settings.ActiveMovePointRangeMin,
                                                       Settings.ActiveMovePointRangeMin)), Tools.MaxWidth + Settings.ActiveMaxCircleRadius);
                origin.Y =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.Y +
                                 Tools.GetRandomNumber(-Settings.ActiveMovePointRangeMin,
                                                       Settings.ActiveMovePointRangeMin)), Tools.MaxHeight + Settings.ActiveMaxCircleRadius);
                drawing.SetDirty();
            }
        }


        public void Render(Graphics g, int scale)
        {
            using (Brush brush = Renderer.GetGdiBrush(Brush))
            {
                //if (rotation != 0)
                //{
                //    g.RotateTransform(0.1f);
                //}

                g.FillEllipse(brush, new RectangleF(origin.X * scale, origin.Y * scale, (float)rx * scale, (float)ry * scale));

                //if (rotation != 0)
                //{
                //    g.RotateTransform(0);
                //}
            }
        }
    }
}
