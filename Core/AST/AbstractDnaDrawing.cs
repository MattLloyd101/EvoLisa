using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GenArt.Core.AST
{
    public abstract class AbstractDnaDrawing
    {
        public void Render(Bitmap image, int scale)
        {
            using (Graphics backGraphics = Graphics.FromImage(image))
            {
                Render(backGraphics, scale);
            }
        }

        public abstract void Render(Graphics graphics, int scale);
    }
}
