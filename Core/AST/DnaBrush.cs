using GenArt.Classes;
using System;

namespace GenArt.AST
{
    [Serializable]
    public class DnaBrush
    {
        public static Pixel[] colours { get; set; }

        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }

        public Tools tool;

        public void Init(Tools tool)
        {
            this.tool = tool;
            // pick a colour from the image.
            Pixel pix = colours[tool.GetRandomNumber(0, colours.Length - 1)];
            Red = pix.R;
            Green = pix.G;
            Blue = pix.B;
            Alpha = tool.GetRandomNumber(Settings.ActiveAlphaRangeMin, Settings.ActiveAlphaRangeMax);
        }

        public DnaBrush Clone()
        {
            return new DnaBrush
                       {
                           Alpha = Alpha,
                           Blue = Blue,
                           Green = Green,
                           Red = Red,
                           tool = tool
                       };
        }

        public String getBrushString()
        {
            return "fill:#" + Red.ToString("X") + Green.ToString("X") + Blue.ToString("X") + ";fill-opacity:" + Alpha / 255.0;
        }

        public void Mutate(DnaDrawing drawing)
        {
            if (tool.WillMutate(Settings.ActiveRedMutationRate))
            {
                Red = tool.GetRandomNumber(Settings.ActiveRedRangeMin, Settings.ActiveRedRangeMax);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveGreenMutationRate))
            {
                Green = tool.GetRandomNumber(Settings.ActiveGreenRangeMin, Settings.ActiveGreenRangeMax);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveBlueMutationRate))
            {
                Blue = tool.GetRandomNumber(Settings.ActiveBlueRangeMin, Settings.ActiveBlueRangeMax);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveAlphaMutationRate))
            {
                Alpha = tool.GetRandomNumber(Settings.ActiveAlphaRangeMin, Settings.ActiveAlphaRangeMax);
                drawing.SetDirty();
            }
        }
    }
}