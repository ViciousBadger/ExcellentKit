using System.Linq;
using UnityEngine;

namespace ExcellentGame
{
    public static class StringScrambler
    {
        public static string Scramble(string input)
        {
            return new string(input.Select(c => (char)(c + 7)).ToArray());
        }

        public static string Unscramble(string input)
        {
            return new string(input.Select(c => (char)(c - 7)).ToArray());
        }
    }
}
