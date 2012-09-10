using System;
using System.Collections.Generic;

namespace GenArt.Classes
{
    public class Tools
    {
        public readonly Random random = new Random();

        public int GetRandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        public double GetRandomNumber(double min, double max)
        {
            return min + random.NextDouble() * max;
        }



        public static int MaxWidth = 200;
        public static int MaxHeight = 200;

        public bool WillMutate(int mutationRate)
        {
            if (GetRandomNumber(0, mutationRate) == 1)
                return true;
            return false;
        }

        public void shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = GetRandomNumber(0, i);
                T tmp = list[j];
                list[j] = list[i];
                list[i] = list[j];
            }
        }
    }
}