using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GenArt.Classes;
using System.Drawing.Imaging;
using GenArt.AST;
using System.Drawing;
using GenArt.Core.AST;

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

        protected Pixel[] sourceColours;
        protected Thread thread;
        protected Object _evolverLock = new Object();

        protected volatile bool _readyToBreed = false;
        protected volatile DnaVectorDrawing _currentDrawing;
        protected volatile bool _isRunning = false;
        protected volatile int _selected;
        protected volatile int _selectedThisGeneration;
        protected volatile int _generation;
        protected double _errorLevel = double.MaxValue;

        private int seed;

        public int selected
        {
            get {return _selected; }
        }

        public int generation
        {
            get { return _generation; }
        }

        public bool isRunning
        {
            get { return _isRunning; }
        }

        public bool readyToBreed
        {
            get { return _readyToBreed; }
        }

        public double errorLevel
        {
            get { return _errorLevel; }
        }

        private bool metCompletionCritera
        {
            get { return _selectedThisGeneration >= SELECTED_MAX; }
        }

        public double progress
        {
            get { return _selectedThisGeneration / (double)SELECTED_MAX; }
        }

        private DnaVectorDrawing GetNewInitializedDrawing()
        {
            var drawing = new DnaVectorDrawing(seed);

            return drawing;
        }

        public AbstractDnaDrawing cloneDrawing()
        {
            lock (_evolverLock)
            {
                return _currentDrawing.Clone();
            }
        }

        public void breed(Evolver other)
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
            _isRunning = true;
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
            _isRunning = false;
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
            if (_currentDrawing == null)
                _currentDrawing = GetNewInitializedDrawing();

            NewFitnessCalculator calc = new NewFitnessCalculator();

            _readyToBreed = false;

            while (_isRunning)
            {
                DnaVectorDrawing newDrawing;
                lock (_currentDrawing)
                {
                    newDrawing = _currentDrawing.Clone();
                }
                newDrawing.Mutate();

                if (newDrawing.IsDirty)
                {
                    _generation++;

                    double newErrorLevel = calc.GetDrawingFitness(newDrawing, sourceColours);

                    if (newErrorLevel <= _errorLevel)
                    {
                        _selected++;
                        _selectedThisGeneration++;
                        lock (_currentDrawing)
                        {
                            _currentDrawing = newDrawing;
                        }
                        _errorLevel = newErrorLevel;
                    }
                }

                if (metCompletionCritera)
                {
                    _readyToBreed = true;
                }
            }
        }

        public int CompareTo(VectorEvolver other)
        {
            return _errorLevel.CompareTo(other._errorLevel);
        }
    }
}
