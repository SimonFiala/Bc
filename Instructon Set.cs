using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

        private static Dictionary<string, Action> setZeroParam { get; } = new Dictionary<string, Action>
        {
            {"XCHG", () =>{
                byte tmp = registers["H"];
                registers["H"] = registers["D"];
                registers["D"] = tmp;

                tmp = registers["L"];
                registers["L"] = registers["E"];
                registers["E"] = tmp;
            }}
        };

        public Dictionary<string, Action> SetZeroParam
        {
            get { return setZeroParam; }
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
                    byte valueFromMemory = GetValueFromMemory("H");
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
                ushort parsedAddress = (ushort)ParseAndCheckNumber(Address, 16);
                memory[parsedAddress] = registers["A"];
            
            }},

            {"LDAX", (RegPair) => {
                RegPair = RegPair.ToUpper();
                byte valueFromMemory = 0;
                switch (RegPair) {
                    case "B":
                        valueFromMemory = GetValueFromMemory("B");
                        break;
                    case "D":
                        valueFromMemory = GetValueFromMemory("D");
                        break;
                    default:
                        //error msg: Registr neni B ani D
                        break;
                }
                registers["A"] = valueFromMemory;
            }},

            {"LHLD", (Address) => {
                ushort parsedAddress = (ushort)ParseAndCheckNumber(Address,16);
                registers["L"] =  GetValueFromMemory(parsedAddress.ToString());
                registers["H"] =  GetValueFromMemory((parsedAddress + 1).ToString());
            }},

            {"STA", (Address) => { 
                ushort parsedAddress = (ushort)ParseAndCheckNumber(Address,16);
                memory[parsedAddress] = registers["A"];
            }},

            {"STAX", (RegPair) => {
                RegPair = RegPair.ToUpper();
                ushort address = 0;
                switch (RegPair) {
                    case "B":
                        address = GetAddresFromRegPair("B");
                        break;
                    case "D":
                        address = GetAddresFromRegPair("D");
                        break;
                    default:
                        //error msg: Registr neni B ani D
                        break;
                }
                memory[address] = registers["A"];
            }},

            {"SHLD", (Address) => {
                ushort parsedAddress = (ushort)ParseAndCheckNumber(Address,16);
                memory[parsedAddress] = registers["L"];
                memory[++parsedAddress] = registers["H"];
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
                        byte valueFromMemory = GetValueFromMemory("H");
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
                        memory[GetAddresFromRegPair("H")] = registers[RMs];
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
                   memory[GetAddresFromRegPair("H")] = DataVal;
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

      
        private static ushort GetAddresFromRegPair(string Reg)
        {
            ushort address = Reg switch
            {
                "B" => (ushort)((registers["B"] << 8) + registers["C"]),
                "D" => (ushort)((registers["D"] << 8) + registers["E"]),
                "H" => (ushort)((registers["H"] << 8) + registers["L"]),
                _ => Convert.ToUInt16(Reg)
            };

            return address;
        }

        private static byte GetValueFromMemory(string Reg)
        {
            ushort address = GetAddresFromRegPair(Reg);
            if (memory.ContainsKey(address))
            {
                return memory[address];
            }
            return 0;
        }

        private static bool CheckIfNumberIsInRange(int number, int numberOfBits)
        {
            return 0 <= number && number < Math.Pow(2,numberOfBits);
        }

        private static int ParseAndCheckNumber(string number, int numberOfBits)
        {
            Regex r = new Regex("^[A-F0-9]{1,5}H{0,1}$");
            number = number.ToUpper();
            if (r.IsMatch(number))
            {
                
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
                    return -1;
                }
                return DataVal;
            }
            else
            {
                //error msg "Nepsravna hodnota"
                return -1;
            }
                 
        }
    }
}
