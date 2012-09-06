using System.Collections.Generic;
using System.Xml.Serialization;
using GenArt.Classes;
using System;
using GenArt.Core.AST;

namespace GenArt.AST
{
    [Serializable]
    public class DnaDrawing
    {
        public List<DnaShape> Shapes { get; set; }

        [XmlIgnore]
        public bool IsDirty { get; private set; }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public void Init()
        {
            Shapes = new List<DnaShape>();


            SetDirty();
        }

        public void spawnLots(int n)
        {

            for (int i = 0; i < n; i++)
            {
                if (Settings.allowPolygons && Tools.WillMutate(4))
                    AddPolygon();

                if (Settings.allowCircles && Tools.WillMutate(4))
                    AddCircle();

                if (Settings.allowTinyCircles && Tools.WillMutate(4))
                    AddTinyCircle();

                if (Settings.allowEllipses && Tools.WillMutate(4))
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

        public DnaDrawing Clone()
        {
            var drawing = new DnaDrawing();
            drawing.Shapes = new List<DnaShape>();
            foreach (DnaShape shape in Shapes)
                drawing.Shapes.Add(shape.Clone());

            return drawing;
        }


        public void Mutate()
        {
            if (Settings.allowPolygons && Tools.WillMutate(Settings.ActiveAddPolygonMutationRate))
                AddPolygon();

            if (Settings.allowCircles && Tools.WillMutate(Settings.ActiveAddEllipseMutationRate))
                AddCircle();

            if (Settings.allowTinyCircles && Tools.WillMutate(Settings.ActiveAddTinyCircleMutationRate))
                AddTinyCircle();
            
            if (Settings.allowEllipses && Tools.WillMutate(Settings.ActiveAddEllipseMutationRate))
                AddEllipse();

            if (Tools.WillMutate(Settings.ActiveRemovePolygonMutationRate))
                RemoveShape();

            if (Tools.WillMutate(Settings.ActiveMovePolygonMutationRate))
                MoveShape();

            foreach (DnaShape s in Shapes)
                s.Mutate(this);
        }

        public void MoveShape()
        {
            if (Shapes.Count < 1)
                return;

            int index = Tools.GetRandomNumber(0, Shapes.Count);
            DnaShape poly = Shapes[index];
            Shapes.RemoveAt(index);
            index = Tools.GetRandomNumber(0, Shapes.Count);
            Shapes.Insert(index, poly);
            SetDirty();
        }

        public void RemoveShape()
        {
            if (Shapes.Count > Settings.ActivePolygonsMin)
            {
                int index = Tools.GetRandomNumber(0, Shapes.Count);
                Shapes.RemoveAt(index);
                SetDirty();
            }
        }

        public void AddEllipse()
        {
            var newEllipse = new DnaEllipse();
            newEllipse.Init();

            Shapes.Add(newEllipse);
            SetDirty();
        }

        public void AddTinyCircle()
        {
            var newCircle = new DnaTinyCircle();
            newCircle.Init();

            Shapes.Add(newCircle);
            SetDirty();
        }

        public void AddCircle()
        {
            var newCircle = new DnaCircle();
            newCircle.Init();

            Shapes.Add(newCircle);
            SetDirty();
        }

        public void AddPolygon()
        {
            var newPolygon = new DnaPolygon();
            newPolygon.Init();

            Shapes.Add(newPolygon);
            SetDirty();
        }
    }
}