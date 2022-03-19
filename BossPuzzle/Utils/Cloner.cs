using System;
using System.Collections.Generic;

namespace BossPuzzle.Utils
{
    public static class Cloner
    {
        public static T[] SingleArr<T>(T[] arr) where T : IConvertible
        {
            int xLength = arr.Length;

            T[] ret = new T[xLength];

            for (var i = 0; i < xLength; i++)
            {
                ret[i] = arr[i];
            }

            return ret;
        }

        public static T[][] DoubleArr<T>(T[][] arr) where T : IConvertible
        {
            int xLength = arr.Length;

            T[][] ret = new T[xLength][];

            for (var i = 0; i < xLength; i++)
            {
                int yLength = arr[i].Length;
                ret[i] = new T[yLength];

                for (var j = 0; j < yLength; j++)
                {
                    ret[i][j] = arr[i][j];
                }
            }

            return ret;
        }
    }
}
