using System.Linq;
using MemoryPack;

namespace ExcellentKit
{
    [MemoryPackable]
    public readonly partial struct SecretString
    {
        public string ScrambledData { get; init; }

        /// <summary>
        /// Scrambles a string to make it 'secret' in memory.
        /// </summary>
        public static SecretString Create(string _unscrambledData)
        {
            return new SecretString
            {
                ScrambledData = new string(_unscrambledData.Select(c => (char)(c + 1337)).ToArray())
            };
        }

        /// <summary>
        /// Unscrambles a secret string.
        /// </summary>
        public readonly string Reveal()
        {
            return new string(ScrambledData.Select(c => (char)(c - 1337)).ToArray());
        }

        public override bool Equals(object obj)
        {
            return obj is SecretString secretString
                && secretString.ScrambledData.Equals(ScrambledData);
        }

        public override int GetHashCode()
        {
            return ScrambledData.GetHashCode();
        }
    }
}
