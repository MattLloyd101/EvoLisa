using System;
using System.Collections.Generic;
using GenArt.Classes;
using GenArt.Core.AST;
using System.Drawing;

namespace GenArt.AST
{
    [Serializable]
    public class DnaPolygon : DnaShape
    {
        public List<DnaPoint> Points { get; set; }
        public DnaBrush Brush { get; set; }
        private Tools tool;

        public DnaPolygon(Tools tool)
        {
            this.tool = tool;
        }

        public void Init()
        {
            Points = new List<DnaPoint>();

            //int count = Tools.GetRandomNumber(3, 3);
            var origin = new DnaPoint();
            origin.Init(tool);

            for (int i = 0; i < Settings.ActivePointsPerPolygonMin; i++)
            {
                var point = new DnaPoint();
                point.X = Math.Min(Math.Max(0, origin.X + tool.GetRandomNumber(-3, 3)), Tools.MaxWidth);
                point.Y = Math.Min(Math.Max(0, origin.Y + tool.GetRandomNumber(-3, 3)), Tools.MaxHeight);

                Points.Add(point);
            }

            Brush = new DnaBrush();
            Brush.Init(tool);
        }

        public int maxX()
        {
            int maxX = 0;
            foreach (DnaPoint p in Points)
            {
                maxX = Math.Max(maxX, p.X);
            }
            return maxX;
        }

        public int maxY()
        {
            int maxY = 0;
            foreach (DnaPoint p in Points)
            {
                maxY = Math.Max(maxY, p.Y);
            }
            return maxY;
        }

        // can't be bothered deciphering C#'s XML.
        public String toXML()
        {
            String pointStr = "";
            foreach (DnaPoint p in Points)
                pointStr += p.X + ","+p.Y+" ";
            pointStr = pointStr.TrimEnd(new char[] { ' ' });

            String brushStr = Brush.getBrushString();

            return "<polygon points=\"" + pointStr + "\" style=\"" + brushStr + "\" />";
        }

        public DnaShape Clone()
        {
            var newPolygon = new DnaPolygon(tool);
            newPolygon.Points = new List<DnaPoint>();
            newPolygon.Brush = Brush.Clone();
            foreach (DnaPoint point in Points)
                newPolygon.Points.Add(point.Clone());

            return newPolygon;
        }

        public void Mutate(DnaDrawing drawing)
        {
            if (tool.WillMutate(Settings.ActiveAddPointMutationRate))
                AddPoint(drawing);

            if (tool.WillMutate(Settings.ActiveRemovePointMutationRate))
                RemovePoint(drawing);

            Brush.Mutate(drawing);
            Points.ForEach(p => p.Mutate(drawing));
        }

        private void RemovePoint(DnaDrawing drawing)
        {
            if (Points.Count > Settings.ActivePointsPerPolygonMin)
            {
                int index = tool.GetRandomNumber(0, Points.Count);
                Points.RemoveAt(index);

                drawing.SetDirty();
            }
        }

        private void AddPoint(DnaDrawing drawing)
        {
            if (Points.Count < Settings.ActivePointsPerPolygonMax)
            {
                var newPoint = new DnaPoint();

                int index = tool.GetRandomNumber(1, Points.Count - 1);

                DnaPoint prev = Points[index - 1];
                DnaPoint next = Points[index];

                newPoint.X = (prev.X + next.X)/2;
                newPoint.Y = (prev.Y + next.Y)/2;


                Points.Insert(index, newPoint);

                drawing.SetDirty();
            }
        }

        //Render a polygon
        public void Render(Graphics g, int scale)
        {
            using (Brush brush = Renderer.GetGdiBrush(Brush))
            {
                Point[] points = Renderer.GetGdiPoints(Points, scale);
                g.FillPolygon(brush, points);
            }
        }

    }
}