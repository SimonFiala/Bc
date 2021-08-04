using System;
using System.Collections.Generic;
using System.Text;

namespace Bc
{
    public class Instructon_Set
    {
        public Instructon_Set()
        {
        }

         private static Dictionary<string, byte> registers { get; } = new Dictionary<string, byte>
            {
                { "A", 0 },
                { "B", 0 },
                { "C", 0 },
                { "D", 0 },
                { "E", 0 },
                { "H", 0 },
                { "L", 0 }
            };
        public Dictionary<string, byte> Registers    // property
        {
            get { return registers; }
            
        }
        private static Dictionary<string, bool> flags { get; } = new Dictionary<string, bool>
        {
            {"Z", false },
            {"S", false },
            {"P", false },
            {"C", false },
            {"AC",false }
        };

        public Dictionary<string, bool> Flags    // property
        {
            get { return flags; }

        }

        private static Dictionary<short, byte> memory { get; } = new Dictionary<short, byte> { };

        public Dictionary<short, byte> Memory
        {
            get { return memory; }
        }



        private static Dictionary<string, Action<string>> set1 = new Dictionary<string, Action<string>>
        {
            {"ADD", (a) => {
                if (registers.ContainsKey(a))
                {
                    flags["C"] = registers["A"] + registers[a] > 255;
                    registers["A"] = (byte)(registers["A"] + registers[a]);

                    // DODĚLAT!!!! CHYBI SIGN FLAG ATD 
                    
                }
                }}
        };


        private void CheckFlagsZeroAndParity()
        {

            flags["Z"] = registers["A"] == 0;

            int numberOfOnesInAcc = Convert.ToString(registers["A"], 2).Replace("0", "").Length;
            flags["P"] = numberOfOnesInAcc % 2 == 0;
        }

    }
}
