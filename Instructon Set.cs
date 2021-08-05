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
                { "L", 0 },
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

        private static Dictionary<ushort, byte> memory { get; } = new Dictionary<ushort, byte> { };

        public Dictionary<ushort, byte> Memory
        {
            get { return memory; }
        }

        private static Dictionary<string, Action<string>> setOneParam { get; } = new Dictionary<string, Action<string>>
        {
            {"ADD", (RM) => {
                RM = RM.ToUpper();
                if (registers.ContainsKey(RM))
                { 
                    CheckFlagsCarryAuxCarry(registers["A"], registers[RM], '+');
                    registers["A"] = (byte)(registers["A"] + registers[RM]);
                } else if (RM == "M")
                {
                    byte valueFromMemory = GetValueFromMemory();
                    CheckFlagsCarryAuxCarry(registers["A"], valueFromMemory, '+');
                    registers["A"] = (byte)(registers["A"] + valueFromMemory);
                } else
                {
                    //error msg "RM neni registr nebo pamet"
                    return;
                }
                CheckFlagsZeroParitySign();
                
                }},

            {"LDA", (Address) => {
                ushort AddressVal = (ushort)ParseAndCheckNumber(Address, 16);
                memory[AddressVal] = registers["A"];
            
            }}
        };
        public Dictionary<string, Action<string>> SetOneParam
        {
            get { return setOneParam; }
        }

        private static Dictionary<string, Action<string, string>> setTwoParam { get; } = new Dictionary<string, Action<string, string>>
        {
            {"MOV", (RMd, RMs) => {
                RMd = RMd.ToUpper();
                RMs = RMs.ToUpper();

                if (registers.ContainsKey(RMd))
                {
                    if (registers.ContainsKey(RMs))
                    {
                        registers[RMd] = registers[RMs];
                    } else if(RMs == "M")
                    {
                        byte valueFromMemory = GetValueFromMemory();
                        registers[RMd] = valueFromMemory;
                    } else
                    {
                        //error msg "RMs neni registr nebo pamet"
                        return;
                    }
                } else if (RMd == "M")
                {
                    if (registers.ContainsKey(RMs))
                    {
                        memory[GetAddresFromHL()] = registers[RMs];
                    } else if(RMs == "M")
                    {
                        //error msg "8085 nepovoluje MOV M, M"
                        return;
                    } else
                    {
                        //error msg "RMs neni registr"
                        return;
                    }
                } else
                {
                    //error msg "RMd neni registr nebo pamet"
                    return;
                }

            }},

            {"MVI", (RMd, Data) => {
                RMd = RMd.ToUpper();
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                if (registers.ContainsKey(RMd))
                {
                   registers[RMd] = DataVal;
                } else if (RMd == "M")
                {
                   memory[GetAddresFromHL()] = DataVal;
                } else
                {
                    //error msg "RMd neni registr nebo pamet"
                    return;
                }
            }},

            {"LXI", (RegPair, Data) => {
                RegPair = RegPair.ToUpper();
                ushort DataVal = (ushort)ParseAndCheckNumber(Data,16);
                if (registers.ContainsKey(RegPair))
                {
                    byte lowNibble = (byte)((DataVal & 0xff));
                    byte highNibble = (byte)((DataVal >> 8) & 0xff);
                    switch (RegPair)
                    {
                        case "B":
                            registers["B"] = highNibble;
                            registers["C"] = lowNibble;
                        break;

                        case "D":
                            registers["D"] = highNibble;
                            registers["E"] = lowNibble;
                        break;

                        case "H":
                            registers["H"] = highNibble;
                            registers["L"] = lowNibble;
                        break;

                        default:
                            if(RegPair == "C" || RegPair == "E" || RegPair == "L")
                            {
                                //error msg "Pary se oznacuji B pro BC, D pro DE etc..."
                                return;
                            }

                        break;
                    }
                } else
                {
                    //error msg "RegPair neni nazev Reg paru"
                    return;
                }
            }}



        };
        public Dictionary<string, Action<string, string>> SetTwoParam
        {
            get { return setTwoParam; }
        }









        // POMOCNE FUNKCE

        private static void CheckFlagsZeroParitySign()
        {
            flags["Z"] = registers["A"] == 0;

            int numberOfOnesInAcc = Convert.ToString(registers["A"], 2).Replace("0", "").Length;
            flags["P"] = numberOfOnesInAcc % 2 == 0;

            flags["S"] = (sbyte)registers["A"] < 0;
        }


        private static void CheckFlagsCarryAuxCarry(byte a, byte b, char op)
        {
            switch (op)
            {
                case '+':
                    flags["C"] = (a + b) > 255;
                    flags["AC"] = ((a & 0xF) + (b & 0xF)) > 0xF;
                    break;

                case '-':
                    flags["C"] = (a - b) < 0;
                    flags["AC"] = ((a & 0xF) + (~b & 0xF) + 1) > 0xF;
                    break;
            }
        }

      
        private static ushort GetAddresFromHL()
        {
            return (ushort)((registers["H"] << 8) + registers["L"]);
        }

        private static byte GetValueFromMemory()
        {
            ushort address = GetAddresFromHL();
            return memory[address];
        }

        private static bool CheckIfNumberIsInRange(int number, int numberOfBits)
        {
            return 0 <= number && number < Math.Pow(2,numberOfBits);
        }

        private static int ParseAndCheckNumber(string number, int numberOfBits)
        {
            //VYŘEŠIT PÍSMENA V ČISLU!!!!!!

            number = number.ToUpper();
            int DataVal;
            if (number.EndsWith("H"))
            {
                number = number.Replace("H", String.Empty);
                DataVal = Convert.ToInt32(number, 16);
            }
            else
            {
                DataVal = Int32.Parse(number);
            }

            if (!CheckIfNumberIsInRange(DataVal, numberOfBits))
            {
                //error msg "Nepsravna hodnota"
                return 0;
            }
            return DataVal;
        }
    }
}
