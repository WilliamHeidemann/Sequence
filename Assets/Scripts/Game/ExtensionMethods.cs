using System;
using UtilityToolkit.Runtime;

namespace Game
{
    public static class ExtensionMethods
    {
        private static readonly Random Random = new();
        public static T[] Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }

            return array;
        }

        public static void Try<T>(this Option<T> option, Action<T> action)
        {
            if (option.IsSome(out T value)) action(value);
        }
    }
}