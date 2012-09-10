using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GenArt.Classes;
using System.Drawing.Imaging;
using GenArt.AST;
using System.Drawing;

namespace GenArt.Core.Classes
{
    // a single threaded evolver.
    public class VectorEvolver : IComparable<VectorEvolver>, Evolver
    {
        private const int SELECTED_MAX = 250;

        public VectorEvolver(int seed, Pixel[] sourceColours)
        {
            this.seed = seed;
            this.sourceColours = sourceColours;
        }

        private Pixel[] sourceColours;
        private Thread thread;
        public Object evolverLock = new Object();

        public volatile bool readyToBreed = false;
        public volatile DnaVectorDrawing currentDrawing;
        public volatile bool isRunning = false;
        public volatile int lastSelected;
        public volatile int selected;
        public volatile int selectedThisGeneration;
        public volatile int generation;
        public double _errorLevel = double.MaxValue;
        private int seed;

        public double errorLevel
        {
            get { return _errorLevel; }
        }

        private bool metCompletionCritera
        {
            get { return selectedThisGeneration >= SELECTED_MAX; }
        }

        public double progress
        {
            get { return selectedThisGeneration / (double)SELECTED_MAX; }
        }

        private DnaVectorDrawing GetNewInitializedDrawing()
        {
            var drawing = new DnaVectorDrawing(seed);

            return drawing;
        }

        public void breed(VectorEvolver other)
        {
            //Console.WriteLine("Breeding");
            //currentDrawing.breed(other.currentDrawing);
            //readyToBreed = false;
            //other.readyToBreed = false;
            //selectedThisGeneration = 0;
            //other.selectedThisGeneration = 0;
        }

        public void start()
        {
            isRunning = true;
            if (thread != null)
                KillThread();

            Console.WriteLine("starting");
            thread = new Thread(StartEvolution)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            thread.Start();
        }

        public void stop()
        {
            isRunning = false;
            KillThread();
        }

        private void KillThread()
        {
            if (thread != null)
            {
                thread.Abort();
            }
            thread = null;
        }

        private void StartEvolution()
        {
            if (currentDrawing == null)
                currentDrawing = GetNewInitializedDrawing();

            lastSelected = 0;
            NewFitnessCalculator calc = new NewFitnessCalculator();

            readyToBreed = false;

            while (isRunning)
            {
                DnaVectorDrawing newDrawing;
                lock (currentDrawing)
                {
                    newDrawing = currentDrawing.Clone();
                }
                newDrawing.Mutate();

                if (newDrawing.IsDirty)
                {
                    generation++;

                    double newErrorLevel = calc.GetDrawingFitness(newDrawing, sourceColours);

                    if (newErrorLevel <= _errorLevel)
                    {
                        selected++;
                        selectedThisGeneration++;
                        lock (currentDrawing)
                        {
                            currentDrawing = newDrawing;
                        }
                        _errorLevel = newErrorLevel;
                    }
                }

                if (metCompletionCritera)
                {
                    readyToBreed = true;
                }
            }
        }

        public int CompareTo(VectorEvolver other)
        {
            return _errorLevel.CompareTo(other._errorLevel);
        }
    }
}
