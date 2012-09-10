using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenArt.AST;
using System.Drawing;
using GenArt.Classes;

namespace GenArt.Core.AST
{
    public interface DnaShape
    {
        int maxX();
        int maxY();

        void Init();

        string toXML();

        DnaShape Clone();

        void Mutate(DnaVectorDrawing drawing);

        void Render(Graphics g, int scale);
    }
}
