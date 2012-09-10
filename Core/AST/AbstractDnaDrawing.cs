using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GenArt.Core.AST
{
    public interface AbstractDnaDrawing
    {
        void Render(Bitmap image, PaintEventArgs e, int scale);
    }
}
