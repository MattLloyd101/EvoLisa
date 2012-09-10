using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenArt.Core.Classes
{
    public interface Evolver
    {

        void breed(Evolver e2);

        bool isRunning { get; set; }

        object start();

        object stop();

        bool readyToBreed { get; set; }

        int selected { get; set; }
    }
}
