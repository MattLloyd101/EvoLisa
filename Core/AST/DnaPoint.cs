using System;
using GenArt.Classes;

namespace GenArt.AST
{
    [Serializable]
    public class DnaPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        private Tools tool;
        public void Init(Tools tool)
        {
            this.tool = tool;
            X = tool.GetRandomNumber(0, Tools.MaxWidth);
            Y = tool.GetRandomNumber(0, Tools.MaxHeight);
        }

        public DnaPoint Clone()
        {
            return new DnaPoint
                       {
                           X = X,
                           Y = Y,
                       };
        }

        public void Mutate(DnaDrawing drawing)
        {
            if (tool.WillMutate(Settings.ActiveMovePointMaxMutationRate))
            {
                X = tool.GetRandomNumber(0, Tools.MaxWidth);
                Y = tool.GetRandomNumber(0, Tools.MaxHeight);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveMovePointMidMutationRate))
            {
                X =
                    Math.Min(
                        Math.Max(0,
                                 X +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMid,
                                                       Settings.ActiveMovePointRangeMid)), Tools.MaxWidth);
                Y =
                    Math.Min(
                        Math.Max(0,
                                 Y +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMid,
                                                       Settings.ActiveMovePointRangeMid)), Tools.MaxHeight);
                drawing.SetDirty();
            }

            if (tool.WillMutate(Settings.ActiveMovePointMinMutationRate))
            {
                X =
                    Math.Min(
                        Math.Max(0,
                                 X +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMin,
                                                       Settings.ActiveMovePointRangeMin)), Tools.MaxWidth);
                Y =
                    Math.Min(
                        Math.Max(0,
                                 Y +
                                 tool.GetRandomNumber(-Settings.ActiveMovePointRangeMin,
                                                       Settings.ActiveMovePointRangeMin)), Tools.MaxHeight);
                drawing.SetDirty();
            }
        }
    }
}