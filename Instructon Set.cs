using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                { "Temp", 0}
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

            {"RLC", () => {
                if ((registers["A"] & 128) > 0) 
                {
                    registers["A"] = (byte)(registers["A"] << 1);
                    registers["A"] += 1;
                    flags["C"] = true;
                } else {
                    registers["A"] = (byte)(registers["A"] << 1);
                    flags["C"] = false;
                }
            }},

            {"RRC", () => {
                if ((registers["A"] & 1) > 0)
                {
                    registers["A"] = (byte)(registers["A"] >> 1);
                    registers["A"] += 128;
                    flags["C"] = true;
                } else {
                    registers["A"] = (byte)(registers["A"] >> 1);
                    flags["C"] = false;
                }
            }},

            {"RAL", () => {
                if ((registers["A"] & 128) > 0)
                {
                    registers["A"] = (byte)(registers["A"] << 1);
                    registers["A"] += Convert.ToByte(flags["C"]);
                    flags["C"] = true;
                } else {
                    registers["A"] = (byte)(registers["A"] << 1);
                    registers["A"] += Convert.ToByte(flags["C"]);
                    flags["C"] = false;
                }
            }},

            {"RAR", () => {
                if ((registers["A"] & 1) > 0)
                {
                    registers["A"] = (byte)(registers["A"] >> 1);
                    registers["A"] += (byte)(Convert.ToByte(flags["C"])*128);
                    flags["C"] = true;
                } else {
                    registers["A"] = (byte)(registers["A"] >> 1);
                    registers["A"] += (byte)(Convert.ToByte(flags["C"])*128);
                    flags["C"] = false;
                }
            }},
        
            {"CMA", () => {
                registers["A"] = (byte)~registers["A"];
            }},

            {"CMC", () => {
                flags["C"] = !flags["C"];
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
                    Console.WriteLine( "RM neni registr nebo pamet");
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '+', true);
                registers["A"] += (byte)(valueFromParametr + carry);
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
                        Console.WriteLine( "Registr neni B ani D");
                        return;
                }
                CheckCarryFor16bitNumbers(HLpairValue, valueFromRegisterPair);
                valueFromRegisterPair += HLpairValue;
                saveValueToRegisterPair("H", valueFromRegisterPair);
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', false);
                registers["A"] += (byte)(~valueFromParametr + 1);
                CheckFlagsZeroParitySign();
            }},

            {"SBB", (RM) => {
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', true);
                registers["A"] += (byte)(~(valueFromParametr + carry) + 1);
                CheckFlagsZeroParitySign();
            }},

            {"SUI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                CheckFlagsCarryAuxCarry(registers["A"], DataVal, '-', false);
                registers["A"] -= DataVal;
                CheckFlagsZeroParitySign();
            }},

            {"SBI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);
                int carry =  Convert.ToInt32(flags["C"]);

                CheckFlagsCarryAuxCarry(registers["A"], DataVal, '-', true);
                registers["A"] -= (byte)(DataVal + carry);
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
                        Console.WriteLine( "Registr neni B ani D");
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
                        Console.WriteLine( "Registr neni B ani D");
                        break;
                }
                memory[address] = registers["A"];
            }},

            {"SHLD", (Address) => {
                ushort parsedAddress = (ushort)ParseAndCheckNumber(Address,16);
                memory[parsedAddress] = registers["L"];
                memory[++parsedAddress] = registers["H"];
            }},

            {"INR", (RM) => {
                RM = RM.ToUpper();
                bool carry = flags["C"];
                if (registers.ContainsKey(RM))
                {
                    CheckFlagsCarryAuxCarry(registers[RM], 1, '+', false);
                    registers[RM] += 1;
                    CheckFlagsZeroParitySign();
                } else if (RM == "M")
                {
                    ushort addresInHL = GetValueFromRegisterPair("H");
                    CheckFlagsCarryAuxCarry(memory[addresInHL], 1, '+', false);
                    memory[addresInHL] += 1;
                    CheckFlagsZeroParitySign();
                } else
                {
                    //error msg "RM neni registr nebo pamet"
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                flags["C"] = carry;
            }},

            {"INX", (RegPair) => {
                RegPair = RegPair.ToUpper();
                if (RegPair == "B" || RegPair == "D" || RegPair == "H"){
                    ushort val = (ushort)(GetValueFromRegisterPair(RegPair) + 1);
                    saveValueToRegisterPair(RegPair,val);
                } else {
                    // error "RegPair neukazuje na registrovy par"
                    System.Console.WriteLine("RegPair neukazuje na registrovy par");
                }
            }},

            {"DCR", (RM) => {
                RM = RM.ToUpper();
                bool carry = flags["C"];
                if (registers.ContainsKey(RM))
                {
                    CheckFlagsCarryAuxCarry(registers[RM], 1, '-', false);
                    registers[RM] -= 1;
                    CheckFlagsZeroParitySign();
                } else if (RM == "M")
                {
                    ushort addresInHL = GetValueFromRegisterPair("H");
                    CheckFlagsCarryAuxCarry(memory[addresInHL], 1, '-', false);
                    memory[addresInHL] -= 1;
                    CheckFlagsZeroParitySign();
                } else
                {
                    //error msg "RM neni registr nebo pamet"
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                flags["C"] = carry;
            }},

            {"DCX", (RegPair) => {
                RegPair = RegPair.ToUpper();
                if (RegPair == "B" || RegPair == "D" || RegPair == "H"){
                    ushort val = (ushort)(GetValueFromRegisterPair(RegPair) - 1);
                    saveValueToRegisterPair(RegPair,val);
                } else {
                    // error "RegPair neukazuje na registrovy par"
                    System.Console.WriteLine("RegPair neukazuje na registrovy par");
                }
            }},

            {"CMP", (RM) => {
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }

                CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', false);
                registers["Temp"] = (byte)(registers["A"] + (byte)(~valueFromParametr + 1));
                CheckFlagsZeroParitySign("Temp");
            }},

            {"CPI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                CheckFlagsCarryAuxCarry(registers["A"], DataVal, '-', false);
                registers["Temp"] = (byte)(registers["A"] + (byte)(~DataVal + 1));
                CheckFlagsZeroParitySign("Temp");
            }},

            {"ANA", (RM) => {
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                registers["A"] &= valueFromParametr;
                CheckFlagsZeroParitySign();
                flags["C"] = false;
                flags["AC"] = true;
            }},

            {"ANI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                registers["A"] &= DataVal;
                CheckFlagsZeroParitySign();
                flags["C"] = false;
                flags["AC"] = true;
            }},

            {"XRA", (RM) => {
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                registers["A"] ^= valueFromParametr;
                CheckFlagsZeroParitySign();
                flags["C"] = false;
                flags["AC"] = false;
            }},

            {"XRI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                registers["A"] ^= DataVal;
                CheckFlagsZeroParitySign();
                flags["C"] = false;
                flags["AC"] = false;
            }},

            {"ORA", (RM) => {
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
                    Console.WriteLine( "RM neni registr nebo pamet");
                    return;
                }
                registers["A"] |= valueFromParametr;
                CheckFlagsZeroParitySign();
                flags["C"] = false;
                flags["AC"] = false;
            }},

            {"ORI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                registers["A"] |= DataVal;
                CheckFlagsZeroParitySign();
                flags["C"] = false;
                flags["AC"] = false;
            }},
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
                        Console.WriteLine( "RMs neni registr nebo pamet");
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
                        Console.WriteLine( "8085 nepovoluje MOV M, M");
                        return;
                    } else
                    {
                        //error msg "RMs neni registr"
                        Console.WriteLine( "RMs neni registr");
                        return;
                    }
                } else
                {
                    //error msg "RMd neni registr nebo pamet"
                    Console.WriteLine( "RMd neni registr nebo pamet");
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
                      Console.WriteLine( "RMd neni registr nebo pamet");
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
                                  Console.WriteLine("Pary se oznacuji B pro BC, D pro DE etc...");
                                return;
                            }

                        break;
                    }
                } else
                {
                    //error msg "RegPair neni nazev Reg paru"
                    Console.WriteLine("RegPair neni nazev Reg paru");
                    return;
                }
            }}


        };
        public Dictionary<string, Action<string, string>> SetTwoParam
        {
            get { return setTwoParam; }
        }


        // POMOCNE FUNKCE

        private static void CheckFlagsZeroParitySign(string register = "A")
        {
            flags["Z"] = registers[register] == 0;

            int numberOfOnesInAcc = Convert.ToString(registers[register], 2).Replace("0", "").Length;
            flags["P"] = numberOfOnesInAcc % 2 == 0;

            flags["S"] = (sbyte)registers[register] < 0;
        }

        private static void CheckFlagsCarryAuxCarry(int a, int b, char op, bool useCarry)
        {
            int carry = Convert.ToInt32(flags["C"]);
            switch (op)
            {
                case '+':
                    flags["C"] = (useCarry == true) ?
                        (a + b + carry) > 255 :
                        (a + b) > 255;

                    flags["AC"] = (useCarry == true) ?
                        ((a & 0xF) + (b & 0xF) + carry) > 0xF :
                        ((a & 0xF) + (b & 0xF)) > 0xF;
                    break;

                case '-':
                    flags["C"] = (useCarry == true) ?
                        (a - b - carry) < 0 :
                        (a - b) < 0;

                    flags["AC"] = (useCarry == true) ?
                        ((a & 0xF) + (~(b + carry) & 0xF) + 1) > 0xF :
                        ((a & 0xF) + (~b & 0xF) + 1) > 0xF;
                    break;
            }
        }

        private static void saveValueToRegisterPair(string regPair, int value)
        {
            ushort val = (ushort)value;
            byte highNibble = (byte)((val >> 8) & 0xff);
            byte lowNibble = (byte)((val & 0xff));
            switch (regPair)
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
                    if (regPair == "C" || regPair == "E" || regPair == "L")
                    {
                        //error msg "Pary se oznacuji B pro BC, D pro DE etc..."
                        Console.WriteLine("Pary se oznacuji B pro BC, D pro DE, H pro HL.");
                        return;
                    }
                    else
                    {
                        //error msg "Argument RegPair neoznacuje registrovt par"
                        Console.WriteLine("Argument RegPair neoznacuje registrovt par");
                        return;
                    }
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
            double range = Math.Pow(2, numberOfBits);
            return -range <= number && number < range;
        }

        private static int ParseAndCheckNumber(string number, int numberOfBits)
        {
            Regex r = new Regex("^[-]{0,1}[A-F0-9]{1,5}H{0,1}$");
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
                    Console.WriteLine("Nepsravna hodnota");
                    return 0;
                }
                return DataVal;
            }
            else
            {
                //error msg "Nepsravna hodnota"
                Console.WriteLine("Nepsravna hodnota");
                return 0;
            }

        }

        public void ReadLines(string path)      //DEMO JEN PRO TESTOVANI
        {
            StreamReader sr = File.OpenText(path);
            string s;
            string[] line;
            while ((s = sr.ReadLine()) != null)
            {

                s = Regex.Replace(s, @"([abcdefhlmABCDEFHLM]{1})\s*,\s*", "$1, ").ToUpper();  //odstraneni mezery mezi prvnim parametrem a carkou
                s = Regex.Replace(s, @"\s*-\s*", " -");
                line = s.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();

                switch (line.Length)
                {
                    case 0:
                        break;
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
                        }
                        else
                        {
                            //error: Ocekavala se carka za prvnim parametrem
                            Console.WriteLine("Ocekavala se carka za prvnim parametrem");
                        }

                        break;

                    default:

                        //error: prilis mnoho argumentu
                        Console.WriteLine("prilis mnoho argumentu");
                        break;
                }
            }

        }

    }
}
