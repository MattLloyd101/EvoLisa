using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenArt.Core.AST;

namespace GenArt.Core.Classes
{
    public interface Evolver
    {

        void breed(Evolver e2);

        void start();

        void stop();

        bool readyToBreed { get; }

        int selected { get; }

        int generation { get; }

        double errorLevel { get; }

        bool isRunning { get; }

        AbstractDnaDrawing cloneDrawing();
    }
}
