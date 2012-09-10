using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using GenArt.AST;
using GenArt.Core.AST;
using System.Windows.Forms;

namespace GenArt.Classes
{
    public static class Renderer
    {

        //Convert a list of DnaPoint to a list of System.Drawing.Point's
        public static Point[] GetGdiPoints(IList<DnaPoint> points,int scale)
        {
            Point[] pts = new Point[points.Count];
            int i = 0;
            foreach (DnaPoint pt in points)
            {
                pts[i++] = new Point((int)pt.X * scale, (int)pt.Y * scale);
            }
            return pts;
        }

        //Convert a DnaBrush to a System.Drawing.Brush
        public static Brush GetGdiBrush(DnaBrush b)
        {
            return new SolidBrush(Color.FromArgb(b.Alpha, b.Red, b.Green, b.Blue));
        }


        
    }
}
