using System;
using System.Security.Cryptography;

namespace Rac.TestAutomation.Common
{
    public class Randomiser
    {
        private static RandomNumberGenerator _generator = RandomNumberGenerator.Create();
        private static Randomiser            _randomiser;

        public static Randomiser Get
        {
            get
            {
                if (_randomiser == null)
                {
                    _randomiser = new Randomiser();
                }
                return _randomiser;
            }
        }

        public int Next(int max)
        {
            return Next(0, max);
        }

        public int Next(int min, int max)
        {
            var calcMax = max - 1;

            var bytes = new byte[sizeof(int)]; // 4 bytes
            _generator.GetNonZeroBytes(bytes);
            var value = BitConverter.ToInt32(bytes, 0);

            var result = ((value - min) % (calcMax - min + 1) + (calcMax - min + 1)) % (calcMax - min + 1) + min;
            return result;
        }
    }
}