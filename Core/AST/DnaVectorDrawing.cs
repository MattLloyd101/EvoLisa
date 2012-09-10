using System.Collections.Generic;
using System.Xml.Serialization;
using GenArt.Classes;
using System;
using GenArt.Core.AST;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GenArt.AST
{
    [Serializable]
    public class DnaVectorDrawing : AbstractDnaDrawing
    {
        private Tools tool;
        private int seed;
        public List<DnaShape> Shapes { get; set; }

        [XmlIgnore]
        public bool IsDirty { get; private set; }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public DnaVectorDrawing(int seed)
        {
            this.seed = seed;
            this.tool = new Tools(seed);
            Shapes = new List<DnaShape>();
            SetDirty();
        }

        public void Render(System.Drawing.Bitmap image, PaintEventArgs e, int scale)
        {
            using (Graphics backGraphics = Graphics.FromImage(image))
            {
                backGraphics.SmoothingMode = SmoothingMode.HighQuality;
                foreach (DnaShape s in Shapes)
                    s.Render(backGraphics, scale);

                e.Graphics.DrawImage(image, 0, 0);
            }

        }

        public void spawnLots(int n)
        {

            for (int i = 0; i < n; i++)
            {
                if (Settings.allowPolygons && tool.WillMutate(4))
                    AddPolygon();

                if (Settings.allowCircles && tool.WillMutate(4))
                    AddCircle();

                if (Settings.allowTinyCircles && tool.WillMutate(4))
                    AddTinyCircle();

                if (Settings.allowEllipses && tool.WillMutate(4))
                    AddEllipse();
            }
        }

        public String toXML()
        {
            double maxX = 0, maxY = 0;
            foreach (DnaShape s in Shapes) {
                maxX = Math.Max(s.maxX(), maxX);
                maxY = Math.Max(s.maxY(), maxY);
            }


            String svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"" + Tools.MaxWidth + "\" height=\"" + Tools.MaxHeight + "\" version=\"1.1\">";
            svg += "<rect width=\"" + Tools.MaxWidth + "\" height=\"" + Tools.MaxHeight + "\" style=\"fill:black\" />";
            foreach(DnaShape s in Shapes) {
                svg += s.toXML();
            }
            svg += "</svg>";

            return svg;
        }

        public DnaVectorDrawing Clone()
        {
            lock (this)
            {
                var drawing = new DnaVectorDrawing(seed);
                drawing.tool = tool;
                drawing.Shapes = new List<DnaShape>();
                foreach (DnaShape shape in Shapes)
                    drawing.Shapes.Add(shape.Clone());

                return drawing;
            }
        }


        public virtual void Mutate()
        {
            if (Settings.allowPolygons && tool.WillMutate(Settings.ActiveAddPolygonMutationRate))
                AddPolygon();

            if (Settings.allowCircles && tool.WillMutate(Settings.ActiveAddEllipseMutationRate))
                AddCircle();

            if (Settings.allowTinyCircles && tool.WillMutate(Settings.ActiveAddTinyCircleMutationRate))
                AddTinyCircle();

            if (Settings.allowEllipses && tool.WillMutate(Settings.ActiveAddEllipseMutationRate))
                AddEllipse();

            if (tool.WillMutate(Settings.ActiveRemoveShapeMutationRate))
                RemoveShape();

            if (tool.WillMutate(Settings.ActiveMovePolygonMutationRate))
                MoveShape();

            foreach (DnaShape s in Shapes)
                s.Mutate(this);
        }

        public void MoveShape()
        {
            if (Shapes.Count < 1)
                return;

            int index = tool.GetRandomNumber(0, Shapes.Count);
            DnaShape poly = Shapes[index];
            Shapes.RemoveAt(index);
            index = tool.GetRandomNumber(0, Shapes.Count);
            Shapes.Insert(index, poly);
            SetDirty();
        }

        public void RemoveShape()
        {
            if (Shapes.Count > Settings.ActivePolygonsMin)
            {
                int index = tool.GetRandomNumber(0, Shapes.Count);
                Shapes.RemoveAt(index);
                SetDirty();
            }
        }

        public void AddEllipse()
        {
            var newEllipse = new DnaEllipse(tool);
            newEllipse.Init();

            Shapes.Add(newEllipse);
            SetDirty();
        }

        public void AddTinyCircle()
        {
            var newCircle = new DnaTinyCircle(tool);
            newCircle.Init();

            Shapes.Add(newCircle);
            SetDirty();
        }

        public void AddCircle()
        {
            var newCircle = new DnaCircle(tool);
            newCircle.Init();

            Shapes.Add(newCircle);
            SetDirty();
        }

        public void AddPolygon()
        {
            var newPolygon = new DnaPolygon(tool);
            newPolygon.Init();

            Shapes.Add(newPolygon);
            SetDirty();
        }

        public void breed(DnaVectorDrawing other)
        {
            lock (this)
            {
                lock (other)
                {
                    shuffleShapes();
                    other.shuffleShapes();
                    List<DnaShape> otherFirst = other.firstHalf;
                    List<DnaShape> otherSnd = other.secondHalf;
                    List<DnaShape> thisFirst = firstHalf;
                    List<DnaShape> thisSnd = secondHalf;
                    otherFirst.AddRange(thisSnd);
                    thisFirst.AddRange(otherSnd);
                    Shapes = thisFirst;
                    other.Shapes = otherFirst;
                }
            }
        }

        private void shuffleShapes()
        {
            tool.shuffle(Shapes);
        }

        private List<DnaShape> firstHalf
        {
            get
            {
                return Shapes.GetRange(0, Shapes.Count / 2);
            }
        }

        private List<DnaShape> secondHalf
        {
            get
            {
                int n = Shapes.Count / 2;
                return Shapes.GetRange(n, Shapes.Count - n);
            }
        }
    }
}