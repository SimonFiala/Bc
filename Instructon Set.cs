using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bc
{
    public class Instructon_Set
    {
        public Instructon_Set()
        {
        }

        //-------------REGISTRY-------------
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
        public Dictionary<string, byte> Registers    
        {
            get { return registers; }
            
        }

        //-------------FLAGS-------------
        private static Dictionary<string, bool> flags { get; } = new Dictionary<string, bool>
        {
            {"Z", false },
            {"S", false },
            {"P", false },
            {"C", false },
            {"AC",false }
        };

        public Dictionary<string, bool> Flags    
        {
            get { return flags; }

        }

        //-------------MEMORY-------------
        private static Dictionary<ushort, byte> memory { get; } = new Dictionary<ushort, byte> { };

        public Dictionary<ushort, byte> Memory
        {
            get { return memory; }
        }

        //-------------INSTRUCTION SETS-------------
        private static Dictionary<string, Action> setZeroParam { get; } = new Dictionary<string, Action>
        {
            {"XCHG", () =>{
                byte tmp = registers["H"];
                registers["H"] = registers["D"];
                registers["D"] = tmp;

                tmp = registers["L"];
                registers["L"] = registers["E"];
                registers["E"] = tmp;
            }},

            {"STC", () => {
                flags["C"] = true;
            }},

        };

        public Dictionary<string, Action> SetZeroParam
        {
            get { return setZeroParam; }
        }

        private static Dictionary<string, Action<string>> setOneParam { get; } = new Dictionary<string, Action<string>>
        {
            {"ADD", (RM) => {
                RM = RM.ToUpper();
                byte valueFromParametr = 0;


                if (registers.ContainsKey(RM))
                {
                    valueFromParametr = registers[RM];
                } else if (RM == "M")
                {
                    valueFromParametr = GetValueFromMemory("H");
                } else
                {
                    //error msg "RM neni registr nebo pamet"
                    return;
                }
                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '+', false);
                registers["A"] += valueFromParametr;
                CheckFlagsZeroParitySign();
                
            }},

            {"ADC", (RM) => {
                RM = RM.ToUpper();
                byte valueFromParametr = 0;
                int carry =  Convert.ToInt32(flags["C"]);

                if (registers.ContainsKey(RM))
                {
                    valueFromParametr = registers[RM];
                } else if (RM == "M")
                {
                    valueFromParametr = GetValueFromMemory("H");
                } else
                {
                    //error msg "RM neni registr nebo pamet"
                    return;
                }
                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '+', true);
                registers["A"] = (byte)(registers["A"] + valueFromParametr + carry);
                CheckFlagsZeroParitySign();
            }},

            {"ADI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                CheckFlagsCarryAuxCarry(registers["A"], DataVal, '+', false);
                registers["A"] += DataVal;
                CheckFlagsZeroParitySign();
            }},

            {"ACI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);
                int carry =  Convert.ToInt32(flags["C"]);

                CheckFlagsCarryAuxCarry(registers["A"], DataVal, '+', true);
                registers["A"] += (byte)(DataVal + carry);
                CheckFlagsZeroParitySign();
            }},

            {"DAD", (RegPair) => {
                RegPair = RegPair.ToUpper();
                ushort valueFromRegisterPair = 0;
                ushort HLpairValue = GetValueFromRegisterPair("H");


                switch (RegPair) {
                    case "B":
                        valueFromRegisterPair = GetValueFromRegisterPair("B");
                        break;
                    case "D":
                        valueFromRegisterPair = GetValueFromRegisterPair("D");
                        break;
                    default:
                        //error msg: Registr neni B ani D
                        return;
                }
                CheckCarryFor16bitNumbers(HLpairValue, valueFromRegisterPair);
                valueFromRegisterPair += HLpairValue;
                registers["H"] = (byte)((valueFromRegisterPair >> 8) & 0xff);
                registers["L"] = (byte)(valueFromRegisterPair & 0xff);
            }},

            {"SUB", (RM) => {
                RM = RM.ToUpper();
                byte valueFromParametr = 0;

                if (registers.ContainsKey(RM))
                {
                    valueFromParametr = registers[RM];
                } else if (RM == "M")
                {
                   valueFromParametr = GetValueFromMemory("H");
                } else
                {
                    //error msg "RM neni registr nebo pamet"
                    return;
                }
                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', false);
                registers["A"] += (byte)(~valueFromParametr + 1);
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
                        return;
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
                        address = GetValueFromRegisterPair("B");
                        break;
                    case "D":
                        address = GetValueFromRegisterPair("D");
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
                        memory[GetValueFromRegisterPair("H")] = registers[RMs];
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
                   memory[GetValueFromRegisterPair("H")] = DataVal;
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
                    byte highNibble = (byte)((DataVal >> 8) & 0xff);
                    byte lowNibble = (byte)((DataVal & 0xff));
                    
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

        private static void CheckFlagsCarryAuxCarry(int a, int b, char op, bool useCarry)
        {
            switch (op)
            {
                case '+':
                    flags["C"] = (useCarry == true) ? 
                        (a + b + Convert.ToInt32(flags["C"])) > 255 : 
                        (a + b) > 255;

                    flags["AC"] = (useCarry == true) ? 
                        ((a & 0xF) + (b & 0xF) + Convert.ToInt32(flags["C"])) > 0xF : 
                        ((a & 0xF) + (b & 0xF)) > 0xF;
                    break;

                case '-':
                    flags["C"] = (a - b) < 0;
                    flags["AC"] = ((a & 0xF) + (~b & 0xF) + 1) > 0xF;
                    break;
            }
        }

        private static void CheckCarryFor16bitNumbers(int a, int b)
        {
            flags["C"] = (a + b) > ushort.MaxValue;
        }


        private static ushort GetValueFromRegisterPair(string Reg)
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
            ushort address = GetValueFromRegisterPair(Reg);
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
                    return 0;
                }
                return DataVal;
            }
            else
            {
                //error msg "Nepsravna hodnota"
                return 0;
            }
                 
        }

        public void ReadLines(string path)      //DEMO JEN PRO TESTOVANI
        {
            StreamReader sr = File.OpenText(path);
            string s;
            string[] line;
            while((s = sr.ReadLine()) != null)
            {
                
                s = Regex.Replace(s, @"([abcdefhlmABCDEFHLM]{1})\s*,\s*", "$1, ").ToUpper();  //odstraneni mezery mezi prvnim parametrem a carkou

                line = s.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                
                switch (line.Length)
                {
                    case 1:
                        setZeroParam[line[0]]();
                        break;

                    case 2:
                        setOneParam[line[0]](line[1]);
                        break;

                    case 3:
                        if ((line[1] = line[1].Replace(" ", "")).EndsWith(","))
                        {
                            line[1] = line[1].Replace(",", "");
                            setTwoParam[line[0]](line[1], line[2]);
                        } else
                        {
                            //error: Ocekala se carka za prvnim parametrem
                        }
                        
                        break;

                    default:

                        //error: prilis mnoho argumentu
                        break;
                }
            }

        }

    }
}
