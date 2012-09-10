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
    public class DnaEllipse : DnaShape
    {
        public Tools tool;
        public DnaBrush Brush { get; set; }

        public DnaPoint origin { get; set; }
        public double rx { get; set; }
        public double ry { get; set; }

        public double rotation { get; set; }

        public DnaEllipse(Tools tool)
        {
            this.tool = tool;
        }

        public int maxX()
        {
            return origin.X + (int)rx;
        }

        public int maxY()
        {
            return origin.Y + (int)ry;
        }

        public virtual void Init()
        {
            
            origin = new DnaPoint();
            origin.Init(tool);

            Brush = new DnaBrush();
            Brush.Init(tool);

            rx = tool.GetRandomNumber(0, Settings.ActiveMaxCircleRadius);
            ry = tool.GetRandomNumber(0, Settings.ActiveMaxCircleRadius);
            rotation = tool.GetRandomNumber(-99.0, 99.0);
        }

        public string toXML()
        {
            String xml = "";
            //float rad2deg = 57.2957795f;

            xml += "<ellipse cx=\"" + (rx/4 + origin.X) + "\" cy=\"" + (ry/4 + origin.Y) + "\" rx=\"" + rx/2 + "\" ry=\"" + ry/2 + "\" style=\"" + Brush.getBrushString() + "\"/>";
            
            return xml;
        }

        public virtual DnaShape Clone()
        {
            var newCirc = new DnaEllipse(tool);
            newCirc.origin = origin.Clone();
            newCirc.Brush = Brush.Clone();
            newCirc.rx = rx;
            newCirc.ry = ry;

            return newCirc;
        }

        public double mutateScalar(double scalar, double min,  double max, DnaDrawing drawing)
        {
            if (tool.WillMutate(Settings.ActiveCircleSizeMidMutationRate))
            {
                drawing.SetDirty();
                return Math.Min(Math.Max(min, scalar +
                                    tool.GetRandomNumber(-Settings.ActiveCircleSizeRangeMid,
                                                           Settings.ActiveCircleSizeRangeMid)), max);
            }

            if (tool.WillMutate(Settings.ActiveCircleSizeMinMutationRate))
            {
                drawing.SetDirty();
                return Math.Min(Math.Max(min, scalar +
                                 tool.GetRandomNumber(-Settings.ActiveCircleSizeRangeMin,
                                                       Settings.ActiveCircleSizeRangeMin)), max);
            }

            return scalar;
        }

        virtual public void Mutate(DnaDrawing drawing)
        {
            Console.WriteLine("Mutating Ellipse");
            if (tool.WillMutate(Settings.ActiveCircleWidthMutationRate))
            {
                rx = mutateScalar(rx, 0, Settings.ActiveMaxCircleRadius, drawing);
            }

            if (tool.WillMutate(Settings.ActiveCircleHeightMutationRate))
            {
                ry = mutateScalar(ry, 0, Settings.ActiveMaxCircleRadius, drawing);
            }

            if (tool.WillMutate(Settings.ActiveRotationMutationRate))
            {
                rotation = mutateScalar(rotation, -999.0, 999.0, drawing);
            }

            Brush.Mutate(drawing);

            MutateOrigin(drawing);
        }

        public void MutateOrigin(DnaDrawing drawing)
        {
            if (tool.WillMutate(Settings.ActiveMovePointMaxMutationRate))
            {
                origin.X = tool.GetRandomNumber(-Settings.ActiveMaxCircleRadius, Tools.MaxWidth + Settings.ActiveMaxCircleRadius);
                origin.Y = tool.GetRandomNumber(-Settings.ActiveMaxCircleRadius, Tools.MaxHeight + Settings.ActiveMaxCircleRadius);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveMovePointMidMutationRate))
            {
                origin.X =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.X +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMid,
                                                       Settings.ActiveMovePointRangeMid)), Tools.MaxWidth + Settings.ActiveMaxCircleRadius);
                origin.Y =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.Y +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMid,
                                                       Settings.ActiveMovePointRangeMid)), Tools.MaxHeight + Settings.ActiveMaxCircleRadius);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveMovePointMinMutationRate))
            {
                origin.X =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.X +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMin,
                                                       Settings.ActiveMovePointRangeMin)), Tools.MaxWidth + Settings.ActiveMaxCircleRadius);
                origin.Y =
                    Math.Min(
                        Math.Max(-Settings.ActiveMaxCircleRadius,
                                 origin.Y +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMin,
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
