using System;
using System.Collections.Generic;
using System.Text;

namespace Bc
{
    class Instructon_Set
    {
        static Dictionary<string, int> registers = new Dictionary<string, int>
            {
                { "A", 0 },
                { "B", 0 },
                { "C", 0 },
                { "D", 0 },
                { "E", 0 },
                { "H", 0 },
                { "L", 0 }
            };

        static Dictionary<string, int> flags = new Dictionary<string, int>
        {
            {"Z", 0 },
            {"S", 0 },
            {"P", 0 },
            {"C", 0 },
            {"AC", 0 }
        };
        Dictionary<string, Action<string>> set1 = new Dictionary<string, Action<string>>
        {
            {"ADD", (a) => {
                if (registers.ContainsKey(a))
                {
                    registers["A"] += registers[a];
                    if(registers["A"] > 255)
                    {
                        registers["A"] -= 255;
                        flags["C"] = 1;
                    }
                }
                registers["A"] += int.Parse(a); }}
        };
    }
}
