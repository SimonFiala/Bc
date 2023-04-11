using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bc.Exceptions;

namespace Bc.Modules
{
    public class _8085sim
    {
        public _8085sim()
        {
            isHalted = false;
            isPaused = false;

            instructions[0xCE] = () => setOneParam["ACI"]("");

            instructions[0x8F] = () => setOneParam["ADC"]("A");
            instructions[0x88] = () => setOneParam["ADC"]("B");
            instructions[0x89] = () => setOneParam["ADC"]("C");
            instructions[0x8A] = () => setOneParam["ADC"]("D");
            instructions[0x8B] = () => setOneParam["ADC"]("E");
            instructions[0x8C] = () => setOneParam["ADC"]("H");
            instructions[0x8D] = () => setOneParam["ADC"]("L");
            instructions[0x8E] = () => setOneParam["ADC"]("M");

            instructions[0x87] = () => setOneParam["ADD"]("A");
            instructions[0x80] = () => setOneParam["ADD"]("B");
            instructions[0x81] = () => setOneParam["ADD"]("C");
            instructions[0x82] = () => setOneParam["ADD"]("D");
            instructions[0x83] = () => setOneParam["ADD"]("E");
            instructions[0x84] = () => setOneParam["ADD"]("H");
            instructions[0x85] = () => setOneParam["ADD"]("L");
            instructions[0x86] = () => setOneParam["ADD"]("M");

            instructions[0xC6] = () => setOneParam["ADI"]("");

            instructions[0xA7] = () => setOneParam["ANA"]("A");
            instructions[0xA0] = () => setOneParam["ANA"]("B");
            instructions[0xA1] = () => setOneParam["ANA"]("C");
            instructions[0xA2] = () => setOneParam["ANA"]("D");
            instructions[0xA3] = () => setOneParam["ANA"]("E");
            instructions[0xA4] = () => setOneParam["ANA"]("H");
            instructions[0xA5] = () => setOneParam["ANA"]("L");
            instructions[0xA6] = () => setOneParam["ANA"]("M");

            instructions[0xE6] = () => setOneParam["ANI"]("");

            instructions[0xCD] = () => setOneParam["CALL"]("");
            instructions[0xDC] = () => setOneParam["CC"]("");
            instructions[0xFC] = () => setOneParam["CM"]("");

            instructions[0x2F] = () => setZeroParam["CMA"]();
            instructions[0x3F] = () => setZeroParam["CMC"]();

            instructions[0xBF] = () => setOneParam["CMP"]("A");
            instructions[0xB8] = () => setOneParam["CMP"]("B");
            instructions[0xB9] = () => setOneParam["CMP"]("C");
            instructions[0xBA] = () => setOneParam["CMP"]("D");
            instructions[0xBB] = () => setOneParam["CMP"]("E");
            instructions[0xBC] = () => setOneParam["CMP"]("H");
            instructions[0xBD] = () => setOneParam["CMP"]("L");
            instructions[0xBE] = () => setOneParam["CMP"]("M");

            instructions[0xD4] = () => setOneParam["CNC"]("");
            instructions[0xC4] = () => setOneParam["CNZ"]("");
            instructions[0xF4] = () => setOneParam["CP"]("");
            instructions[0xEC] = () => setOneParam["CPE"]("");

            instructions[0xFE] = () => setOneParam["CPI"]("");

            instructions[0xE4] = () => setOneParam["CPO"]("");
            instructions[0xCC] = () => setOneParam["CZ"]("");

            instructions[0x27] = () => setZeroParam["DAA"]();

            instructions[0x09] = () => setOneParam["DAD"]("B");
            instructions[0x19] = () => setOneParam["DAD"]("D");
            instructions[0x29] = () => setOneParam["DAD"]("H");
            instructions[0x39] = () => setOneParam["DAD"]("SP");

            instructions[0x3D] = () => setOneParam["DCR"]("A");
            instructions[0x05] = () => setOneParam["DCR"]("B");
            instructions[0x0D] = () => setOneParam["DCR"]("C");
            instructions[0x15] = () => setOneParam["DCR"]("D");
            instructions[0x1D] = () => setOneParam["DCR"]("E");
            instructions[0x25] = () => setOneParam["DCR"]("H");
            instructions[0x2D] = () => setOneParam["DCR"]("L");
            instructions[0x35] = () => setOneParam["DCR"]("M");

            instructions[0x0B] = () => setOneParam["DCX"]("B");
            instructions[0x1B] = () => setOneParam["DCX"]("D");
            instructions[0x2B] = () => setOneParam["DCX"]("H");
            instructions[0x3B] = () => setOneParam["DCX"]("SP");

            instructions[0xF3] = () => setZeroParam["NOP"](); // DI
            instructions[0xFB] = () => setZeroParam["NOP"](); // EI

            instructions[0x76] = () => setZeroParam["HLT"]();

            instructions[0xDB] = () => setZeroParam["NOP"](); // IN

            instructions[0x3C] = () => setOneParam["INR"]("A");
            instructions[0x04] = () => setOneParam["INR"]("B");
            instructions[0x0C] = () => setOneParam["INR"]("C");
            instructions[0x14] = () => setOneParam["INR"]("D");
            instructions[0x1C] = () => setOneParam["INR"]("E");
            instructions[0x24] = () => setOneParam["INR"]("H");
            instructions[0x2C] = () => setOneParam["INR"]("L");
            instructions[0x34] = () => setOneParam["INR"]("M");

            instructions[0x03] = () => setOneParam["INX"]("B");
            instructions[0x13] = () => setOneParam["INX"]("D");
            instructions[0x23] = () => setOneParam["INX"]("H");
            instructions[0x33] = () => setOneParam["INX"]("SP");

            instructions[0xDA] = () => setOneParam["JC"]("");
            instructions[0xFA] = () => setOneParam["JM"]("");
            instructions[0xC3] = () => setOneParam["JMP"]("");
            instructions[0xD2] = () => setOneParam["JNC"]("");
            instructions[0xC2] = () => setOneParam["JNZ"]("");
            instructions[0xF2] = () => setOneParam["JP"]("");
            instructions[0xEA] = () => setOneParam["JPE"]("");
            instructions[0xE2] = () => setOneParam["JPO"]("");
            instructions[0xCA] = () => setOneParam["JZ"]("");

            instructions[0x3A] = () => setOneParam["LDA"]("");

            instructions[0x0A] = () => setOneParam["LDAX"]("B");
            instructions[0x1A] = () => setOneParam["LDAX"]("D");

            instructions[0x2A] = () => setOneParam["LHLD"]("");

            instructions[0x01] = () => setTwoParam["LXI"]("B", "");
            instructions[0x11] = () => setTwoParam["LXI"]("D", "");
            instructions[0x21] = () => setTwoParam["LXI"]("H", "");
            instructions[0x31] = () => setTwoParam["LXI"]("SP", "");

            instructions[0x7F] = () => setTwoParam["MOV"]("A", "A");
            instructions[0x78] = () => setTwoParam["MOV"]("A", "B");
            instructions[0x79] = () => setTwoParam["MOV"]("A", "C");
            instructions[0x7A] = () => setTwoParam["MOV"]("A", "D");
            instructions[0x7B] = () => setTwoParam["MOV"]("A", "E");
            instructions[0x7C] = () => setTwoParam["MOV"]("A", "H");
            instructions[0x7D] = () => setTwoParam["MOV"]("A", "L");
            instructions[0x7E] = () => setTwoParam["MOV"]("A", "M");

            instructions[0x47] = () => setTwoParam["MOV"]("B", "A");
            instructions[0x40] = () => setTwoParam["MOV"]("B", "B");
            instructions[0x41] = () => setTwoParam["MOV"]("B", "C");
            instructions[0x42] = () => setTwoParam["MOV"]("B", "D");
            instructions[0x43] = () => setTwoParam["MOV"]("B", "E");
            instructions[0x44] = () => setTwoParam["MOV"]("B", "H");
            instructions[0x45] = () => setTwoParam["MOV"]("B", "L");
            instructions[0x46] = () => setTwoParam["MOV"]("B", "M");

            instructions[0x4F] = () => setTwoParam["MOV"]("C", "A");
            instructions[0x48] = () => setTwoParam["MOV"]("C", "B");
            instructions[0x49] = () => setTwoParam["MOV"]("C", "C");
            instructions[0x4A] = () => setTwoParam["MOV"]("C", "D");
            instructions[0x4B] = () => setTwoParam["MOV"]("C", "E");
            instructions[0x4C] = () => setTwoParam["MOV"]("C", "H");
            instructions[0x4D] = () => setTwoParam["MOV"]("C", "L");
            instructions[0x4E] = () => setTwoParam["MOV"]("C", "M");

            instructions[0x57] = () => setTwoParam["MOV"]("D", "A");
            instructions[0x50] = () => setTwoParam["MOV"]("D", "B");
            instructions[0x51] = () => setTwoParam["MOV"]("D", "C");
            instructions[0x52] = () => setTwoParam["MOV"]("D", "D");
            instructions[0x53] = () => setTwoParam["MOV"]("D", "E");
            instructions[0x54] = () => setTwoParam["MOV"]("D", "H");
            instructions[0x55] = () => setTwoParam["MOV"]("D", "L");
            instructions[0x56] = () => setTwoParam["MOV"]("D", "M");

            instructions[0x5F] = () => setTwoParam["MOV"]("E", "A");
            instructions[0x58] = () => setTwoParam["MOV"]("E", "B");
            instructions[0x59] = () => setTwoParam["MOV"]("E", "C");
            instructions[0x5A] = () => setTwoParam["MOV"]("E", "D");
            instructions[0x5B] = () => setTwoParam["MOV"]("E", "E");
            instructions[0x5C] = () => setTwoParam["MOV"]("E", "H");
            instructions[0x5D] = () => setTwoParam["MOV"]("E", "L");
            instructions[0x5E] = () => setTwoParam["MOV"]("E", "M");

            instructions[0x67] = () => setTwoParam["MOV"]("H", "A");
            instructions[0x60] = () => setTwoParam["MOV"]("H", "B");
            instructions[0x61] = () => setTwoParam["MOV"]("H", "C");
            instructions[0x62] = () => setTwoParam["MOV"]("H", "D");
            instructions[0x63] = () => setTwoParam["MOV"]("H", "E");
            instructions[0x64] = () => setTwoParam["MOV"]("H", "H");
            instructions[0x65] = () => setTwoParam["MOV"]("H", "L");
            instructions[0x66] = () => setTwoParam["MOV"]("H", "M");

            instructions[0x6F] = () => setTwoParam["MOV"]("L", "A");
            instructions[0x68] = () => setTwoParam["MOV"]("L", "B");
            instructions[0x69] = () => setTwoParam["MOV"]("L", "C");
            instructions[0x6A] = () => setTwoParam["MOV"]("L", "D");
            instructions[0x6B] = () => setTwoParam["MOV"]("L", "E");
            instructions[0x6C] = () => setTwoParam["MOV"]("L", "H");
            instructions[0x6D] = () => setTwoParam["MOV"]("L", "L");
            instructions[0x6E] = () => setTwoParam["MOV"]("L", "M");

            instructions[0x77] = () => setTwoParam["MOV"]("M", "A");
            instructions[0x70] = () => setTwoParam["MOV"]("M", "B");
            instructions[0x71] = () => setTwoParam["MOV"]("M", "C");
            instructions[0x72] = () => setTwoParam["MOV"]("M", "D");
            instructions[0x73] = () => setTwoParam["MOV"]("M", "E");
            instructions[0x74] = () => setTwoParam["MOV"]("M", "H");
            instructions[0x75] = () => setTwoParam["MOV"]("M", "L");

            instructions[0x3E] = () => setTwoParam["MVI"]("A", "");
            instructions[0x06] = () => setTwoParam["MVI"]("B", "");
            instructions[0x0E] = () => setTwoParam["MVI"]("C", "");
            instructions[0x16] = () => setTwoParam["MVI"]("D", "");
            instructions[0x1E] = () => setTwoParam["MVI"]("E", "");
            instructions[0x26] = () => setTwoParam["MVI"]("H", "");
            instructions[0x2E] = () => setTwoParam["MVI"]("L", "");
            instructions[0x36] = () => setTwoParam["MVI"]("M", "");

            instructions[0x00] = () => setZeroParam["NOP"]();

            instructions[0xB7] = () => setOneParam["ORA"]("A");
            instructions[0xB0] = () => setOneParam["ORA"]("B");
            instructions[0xB1] = () => setOneParam["ORA"]("C");
            instructions[0xB2] = () => setOneParam["ORA"]("D");
            instructions[0xB3] = () => setOneParam["ORA"]("E");
            instructions[0xB4] = () => setOneParam["ORA"]("H");
            instructions[0xB5] = () => setOneParam["ORA"]("L");
            instructions[0xB6] = () => setOneParam["ORA"]("M");

            instructions[0xF6] = () => setOneParam["ORI"]("");

            instructions[0xD3] = () => setZeroParam["NOP"](); // OUT

            instructions[0xE9] = () => setZeroParam["PCHL"]();

            instructions[0xC1] = () => setOneParam["POP"]("B");
            instructions[0xD1] = () => setOneParam["POP"]("D");
            instructions[0xE1] = () => setOneParam["POP"]("H");
            instructions[0xF1] = () => setOneParam["POP"]("PSW");

            instructions[0xC5] = () => setOneParam["PUSH"]("B");
            instructions[0xD5] = () => setOneParam["PUSH"]("D");
            instructions[0xE5] = () => setOneParam["PUSH"]("H");
            instructions[0xF5] = () => setOneParam["PUSH"]("PSW");

            instructions[0x17] = () => setZeroParam["RAL"]();
            instructions[0x1F] = () => setZeroParam["RAR"]();

            instructions[0xD8] = () => setZeroParam["RC"]();
            instructions[0xC9] = () => setZeroParam["RET"]();
            instructions[0x20] = () => setZeroParam["NOP"]();  //RIM

            instructions[0x07] = () => setZeroParam["RLC"]();

            instructions[0xF8] = () => setZeroParam["RM"]();
            instructions[0xD0] = () => setZeroParam["RNC"]();
            instructions[0xC0] = () => setZeroParam["RNZ"]();
            instructions[0xF0] = () => setZeroParam["RP"]();
            instructions[0xE8] = () => setZeroParam["RPE"]();
            instructions[0xE0] = () => setZeroParam["RPO"]();

            instructions[0x0F] = () => setZeroParam["RRC"]();

            instructions[0xC7] = () => setOneParam["RST"]("0");
            instructions[0xCF] = () => setOneParam["RST"]("1");
            instructions[0xD7] = () => setOneParam["RST"]("2");
            instructions[0xDF] = () => setOneParam["RST"]("3");
            instructions[0xE7] = () => setOneParam["RST"]("4");
            instructions[0xEF] = () => setOneParam["RST"]("5");
            instructions[0xF7] = () => setOneParam["RST"]("6");
            instructions[0xFF] = () => setOneParam["RST"]("7");

            instructions[0xC8] = () => setZeroParam["NOP"]();

            instructions[0x9F] = () => setOneParam["SBB"]("A");
            instructions[0x98] = () => setOneParam["SBB"]("B");
            instructions[0x99] = () => setOneParam["SBB"]("C");
            instructions[0x9A] = () => setOneParam["SBB"]("D");
            instructions[0x9B] = () => setOneParam["SBB"]("E");
            instructions[0x9C] = () => setOneParam["SBB"]("H");
            instructions[0x9D] = () => setOneParam["SBB"]("L");
            instructions[0x9E] = () => setOneParam["SBB"]("M");

            instructions[0xDE] = () => setOneParam["SBI"]("");

            instructions[0x22] = () => setOneParam["SHLD"]("");

            instructions[0x30] = () => setZeroParam["NOP"](); // SIM

            instructions[0xF9] = () => setZeroParam["SPHL"]();

            instructions[0x32] = () => setOneParam["STA"]("");

            instructions[0x02] = () => setOneParam["STAX"]("B");
            instructions[0x12] = () => setOneParam["STAX"]("D");

            instructions[0x37] = () => setZeroParam["STC"]();

            instructions[0x97] = () => setOneParam["SUB"]("A");
            instructions[0x90] = () => setOneParam["SUB"]("B");
            instructions[0x91] = () => setOneParam["SUB"]("C");
            instructions[0x92] = () => setOneParam["SUB"]("D");
            instructions[0x93] = () => setOneParam["SUB"]("E");
            instructions[0x94] = () => setOneParam["SUB"]("H");
            instructions[0x95] = () => setOneParam["SUB"]("L");
            instructions[0x96] = () => setOneParam["SUB"]("M");

            instructions[0xD6] = () => setOneParam["SUI"]("");

            instructions[0xEB] = () => setZeroParam["XCHG"]();

            instructions[0xAF] = () => setOneParam["XRA"]("A");
            instructions[0xA8] = () => setOneParam["XRA"]("B");
            instructions[0xA9] = () => setOneParam["XRA"]("C");
            instructions[0xAA] = () => setOneParam["XRA"]("D");
            instructions[0xAB] = () => setOneParam["XRA"]("E");
            instructions[0xAC] = () => setOneParam["XRA"]("H");
            instructions[0xAD] = () => setOneParam["XRA"]("L");
            instructions[0xAE] = () => setOneParam["XRA"]("M");

            instructions[0xEE] = () => setOneParam["XRI"]("");
            instructions[0xE3] = () => setZeroParam["XTHL"]();

        }
        private static Action[] instructions { get; } = new Action[256];

        private string path { get; set; }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }


        public Action[] Instructions
        {
            get { return instructions; }
        }

        //-------------HELPING VARIABLES-------------
        private static bool translatingCode = false;
        private static int lineCounter = 0;
        private static ushort programStart = 0x0000;
        private static volatile bool isHalted = false;
        private static volatile bool isPaused = false;

        public bool IsHalted
        {
            get { return isHalted; } set { isHalted = value; } 
        }

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
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
                { "Temp", 0 }
            };

        public Dictionary<string, byte> Registers
        {
            get
            {
                var tmp = registers;
                tmp.Remove("Temp");
                return tmp;
            }

        }

        private static Dictionary<string, ushort> specialRegisters { get; } = new Dictionary<string, ushort>
        {
            { "PSW", 0 },
            { "SP", 0x4000 },
            { "PC", 0 }
        };

        public Dictionary<string, ushort> SpecialRegisters
        {
            get { return specialRegisters; }
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
        private static byte[] memory { get; set; } = new byte[0xFFFF];
        public byte[] Memory
        {
            get
            {
                return memory;
            }
        }

        private static int[] correspondingLinesToMemory { get; set; } = new int[0xFFFF];

        public int[] CorrespondingLinesToMemory
        {
            get { return correspondingLinesToMemory; }
        }


        //-------------LABELS-------------
        private static Dictionary<string, ushort> labels { get; } = new Dictionary<string, ushort> { };

        public Dictionary<string, ushort> Labels
        {
            get { return labels; }
        }

        private static Dictionary<string, List<ushort>> instructionsWaitingForLabelAddress = new Dictionary<string, List<ushort>> { };


        private static Dictionary<string, List<ushort>> translationWaitingForLabelAddress = new Dictionary<string, List<ushort>> { };

        //-------------ARRAY FOR TRANSLATED CODE-------------
        private static List<string> translatedCode { get; } = new List<string>();

        public string TranslatedCode
        {
            get
            {
                string tmp = "";
                foreach (var i in translatedCode)
                {
                    tmp += i + "\n";
                }
                return tmp;
            }
        }

        //-------------ARRAY FOR PARSED CODE------------- 
        private static List<string> parsedCode { get; } = new List<string>();

        public List<string> ParsedCode
        {
            get { return parsedCode; }
        }

        //-------------INSTRUCTION SETS-------------
        private Dictionary<string, Action> setZeroParam { get; } = new Dictionary<string, Action>
        {
            {"XCHG", () =>{
                if(translatingCode)
                {
                    translatedCode.Add("EB");
                }
                else
                {
                    byte tmp = registers["H"];
                    registers["H"] = registers["D"];
                    registers["D"] = tmp;

                    tmp = registers["L"];
                    registers["L"] = registers["E"];
                    registers["E"] = tmp;
                }
                specialRegisters["PC"]++;
            }},

            {"STC", () => {
                if(translatingCode)
                {
                    translatedCode.Add("37");
                }
                else
                {
                    flags["C"] = true;
                }
                specialRegisters["PC"]++;
            }},

            {"RLC", () => {
                if(translatingCode)
                {
                    translatedCode.Add("07");
                }
                else
                {
                    if ((registers["A"] & 128) > 0)
                    {
                        registers["A"] = (byte)(registers["A"] << 1);
                        registers["A"] += 1;
                        flags["C"] = true;
                    }
                    else
                    {
                        registers["A"] = (byte)(registers["A"] << 1);
                        flags["C"] = false;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RRC", () => {
                if(translatingCode)
                {
                    translatedCode.Add("0F");
                }
                else
                {
                    if ((registers["A"] & 1) > 0)
                    {
                        registers["A"] = (byte)(registers["A"] >> 1);
                        registers["A"] += 128;
                        flags["C"] = true;
                    }
                    else
                    {
                        registers["A"] = (byte)(registers["A"] >> 1);
                        flags["C"] = false;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RAL", () => {

                if(translatingCode)
                {
                    translatedCode.Add("17");
                }
                else
                {
                    if ((registers["A"] & 128) > 0)
                    {
                        registers["A"] = (byte)(registers["A"] << 1);
                        registers["A"] += Convert.ToByte(flags["C"]);
                        flags["C"] = true;
                    }
                    else
                    {
                        registers["A"] = (byte)(registers["A"] << 1);
                        registers["A"] += Convert.ToByte(flags["C"]);
                        flags["C"] = false;
                    }

                }
                specialRegisters["PC"]++;
            }},

            {"RAR", () => {

                if(translatingCode)
                {
                    translatedCode.Add("1F");
                }
                else
                {

                    if ((registers["A"] & 1) > 0)
                    {
                        registers["A"] = (byte)(registers["A"] >> 1);
                        registers["A"] += (byte)(Convert.ToByte(flags["C"])*128);
                        flags["C"] = true;
                    }
                    else
                    {
                        registers["A"] = (byte)(registers["A"] >> 1);
                        registers["A"] += (byte)(Convert.ToByte(flags["C"])*128);
                        flags["C"] = false;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"CMA", () => {

                if(translatingCode)
                {
                    translatedCode.Add("2F");
                }
                else
                {
                    registers["A"] = (byte)~registers["A"];
                }
                specialRegisters["PC"]++;
            }},

            {"CMC", () => {
                if(translatingCode)
                {
                    translatedCode.Add("3F");
                }
                else
                {
                    flags["C"] = !flags["C"];
                }
                specialRegisters["PC"]++;
            }},

            {"NOP", () => {
                if(translatingCode)
                {
                    translatedCode.Add("00");
                }
                specialRegisters["PC"]++;
            }},

            {"HLT", () => {
                if(translatingCode)
                {
                    translatedCode.Add("76");
                }
                else
                {
                    isHalted = true;
                }
                specialRegisters["PC"]++;
            }},

            {"RET", () => {
                if(translatingCode)
                {
                    translatedCode.Add("C9");
                    specialRegisters["PC"]++;
                }
                else
                {
                    LoadValueFromStack("PC");
                    lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                }

            }},

            {"RC", () => {
                if(translatingCode)
                {
                    translatedCode.Add("D8");
                }
                else
                {
                    if(flags["C"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RNC", () => {
                if(translatingCode)
                {
                    translatedCode.Add("D0");

                }
                else
                {
                    if(!flags["C"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RP", () => {
                if(translatingCode)
                {
                    translatedCode.Add("F0");

                }
                else
                {
                    if(!flags["S"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RM", () => {
                if(translatingCode)
                {
                    translatedCode.Add("F8");

                }
                else
                {
                    if(!flags["S"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RZ", () => {
                if(translatingCode)
                {
                    translatedCode.Add("C8");

                }
                else
                {
                    if(flags["Z"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RNZ", () => {
                if(translatingCode)
                {
                    translatedCode.Add("C0");

                }
                else
                {
                    if(!flags["Z"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RPE", () => {
                if(translatingCode)
                {
                    translatedCode.Add("E8");

                }
                else
                {
                    if(flags["P"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"RPO", () => {
                if(translatingCode)
                {
                    translatedCode.Add("E0");

                }
                else
                {
                    if(!flags["P"])
                    {
                        LoadValueFromStack("PC");
                        lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                        return;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"PCHL", () => {
                if(translatingCode)
                {
                    translatedCode.Add("E9");
                }
                else
                {
                    ushort val = GetValueFromRegisterPair("H");
                    SaveValueToRegisterPair("PC", val);
                    return;
                }
                specialRegisters["PC"]++;
            }},

            {"SPHL", () => {
                if(translatingCode)
                {
                    translatedCode.Add("F9");
                }
                else
                {
                    ushort val = GetValueFromRegisterPair("H");
                    SaveValueToRegisterPair("SP", val);
                }
                specialRegisters["PC"]++;
            }},

            {"XTHL", () => {
                if(translatingCode)
                {
                    translatedCode.Add("E3");
                }
                else
                {
                    ushort val = GetValueFromRegisterPair("H");
                    LoadValueFromStack("H");
                    SaveValueOnStack(val);
                }
                specialRegisters["PC"]++;
            }},
        };

        public Dictionary<string, Action> SetZeroParam
        {
            get { return setZeroParam; }
        }

        private static Dictionary<string, Action<string>> setOneParam { get; } = new Dictionary<string, Action<string>>
        {
            {"ADD", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"A", "87"},
                            {"B", "80"},
                            {"C", "81"},
                            {"D", "82"},
                            {"E", "83"},
                            {"H", "84"},
                            {"L", "85"},
                            {"M", "86"}
                        };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);

                    CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '+', false);
                    registers["A"] += valueFromParametr;
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"]++;
            }},

            {"ADC", (RM) => {

                 if(translatingCode)
                 {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"A", "8F"},
                        {"B", "88"},
                        {"C", "89"},
                        {"D", "8A"},
                        {"E", "8B"},
                        {"H", "8C"},
                        {"L", "8D"},
                        {"M", "8E"}
                    };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);
                    int carry =  Convert.ToInt32(flags["C"]);

                    CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '+', true);
                    registers["A"] += (byte)(valueFromParametr + carry);
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"]++;
            }},

            {"ADI", (Data) => {
                byte DataVal;

                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("C6 "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    CheckFlagsCarryAuxCarry(registers["A"], DataVal, '+', false);
                    registers["A"] += DataVal;
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"] += 2;
            }},

            {"ACI", (Data) => {
                byte DataVal;
                int carry =  Convert.ToInt32(flags["C"]);

                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("CE "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    CheckFlagsCarryAuxCarry(registers["A"], DataVal, '+', true);
                    registers["A"] += (byte)(DataVal + carry);
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"] += 2;
            }},

            {"DAD", (RegPair) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"B",  "09"},
                        {"D",  "19"},
                        {"H",  "29"},
                        {"SP", "39"}
                    };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArugment must be register pair or SP.");
                    }
                }
                else
                {
                    ushort valueFromRegisterPair = 0;
                    ushort HLpairValue = GetValueFromRegisterPair("H");

                    if(RegPair == "B" || RegPair == "D")
                    {
                        valueFromRegisterPair = GetValueFromRegisterPair(RegPair);
                    }
                    else
                    {
                        valueFromRegisterPair = specialRegisters[RegPair];
                    }

                    CheckCarryFor16bitNumbers(HLpairValue, valueFromRegisterPair);
                    valueFromRegisterPair += HLpairValue;
                    SaveValueToRegisterPair("H", valueFromRegisterPair);
                }
                specialRegisters["PC"]++;
            }},

            {"SUB", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"A", "97"},
                        {"B", "90"},
                        {"C", "91"},
                        {"D", "92"},
                        {"E", "93"},
                        {"H", "94"},
                        {"L", "95"},
                        {"M", "96"}
                    };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);

                    CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', false);
                    registers["A"] += (byte)(~valueFromParametr + 1);
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"]++;
            }},

            {"SBB", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"A", "9F"},
                        {"B", "98"},
                        {"C", "99"},
                        {"D", "9A"},
                        {"E", "9B"},
                        {"H", "9C"},
                        {"L", "9D"},
                        {"M", "9E"}
                    };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);
                    int carry =  Convert.ToInt32(flags["C"]);

                    CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', true);
                    registers["A"] += (byte)(~(valueFromParametr + carry) + 1);
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"]++;
            }},

            {"SUI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);

                if(translatingCode)
                {
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("D6 "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    CheckFlagsCarryAuxCarry(registers["A"], DataVal, '-', false);
                    registers["A"] -= DataVal;
                    CheckFlagsZeroParitySign();
                }
                specialRegisters["PC"] += 2;
            }},

            {"SBI", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);
                int carry =  Convert.ToInt32(flags["C"]);

                if(translatingCode)
                {
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("DE "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    CheckFlagsCarryAuxCarry(registers["A"], DataVal, '-', true);
                    registers["A"] -= (byte)(DataVal + carry);
                    CheckFlagsZeroParitySign();
                }
                 specialRegisters["PC"] += 2;
            }},

            {"LDA", (Address) => {
                ushort parsedAddress;
                if(translatingCode)
                {
                    parsedAddress = (ushort)ParseAndCheckNumber(Address, 16);
                    Address = Translate16BitNumber(parsedAddress);
                    translatedCode.Add("3A "+Address);
                }
                else
                {
                    parsedAddress = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    registers["A"] = memory[parsedAddress];
                }
                specialRegisters["PC"] += 3;
            }},

            {"LDAX", (RegPair) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"B", "0A"},
                        {"D", "1A"}
                    };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nRegister must be B or D");
                    }
                }
                else
                {
                    byte valueFromMemory =  GetValueFromMemory(RegPair);
                    registers["A"] = valueFromMemory;
                }
                specialRegisters["PC"]++;
            }},

            {"LHLD", (Address) => {
                ushort parsedAddress;

                if(translatingCode)
                {
                    parsedAddress = (ushort)ParseAndCheckNumber(Address, 16);
                    Address = Translate16BitNumber(parsedAddress);
                    translatedCode.Add("2A "+Address);
                }
                else
                {
                    parsedAddress = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    registers["L"] =  GetValueFromMemory(parsedAddress.ToString());
                    registers["H"] =  GetValueFromMemory((parsedAddress + 1).ToString());
                }
                specialRegisters["PC"] += 3;
            }},

            {"STA", (Address) => {
                ushort parsedAddress;

                if(translatingCode)
                {
                    parsedAddress = (ushort)ParseAndCheckNumber(Address,16);
                    Address = Translate16BitNumber(parsedAddress);
                    translatedCode.Add("32 "+Address);
                }
                else
                {
                    parsedAddress = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    memory[parsedAddress] = registers["A"];
                }
                specialRegisters["PC"] += 3;
            }},

            {"STAX", (RegPair) => {

                if(translatingCode)
                {
                    Dictionary<string, string> opcodes = new Dictionary<string, string>
                    {
                        {"B", "01"},
                        {"D", "12"}
                    };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nRegister must be B or D");
                    }
                }
                else
                {
                    ushort address =  GetValueFromRegisterPair(RegPair);
                    memory[address] = registers["A"];
                }
                specialRegisters["PC"]++;
            }},

            {"SHLD", (Address) => {
                ushort parsedAddress;

                if(translatingCode)
                {
                    parsedAddress = (ushort)ParseAndCheckNumber(Address,16);
                    Address = Translate16BitNumber(parsedAddress);
                    translatedCode.Add("22 "+Address);
                }
                else
                {
                    parsedAddress = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    memory[parsedAddress] = registers["L"];
                    memory[++parsedAddress] = registers["H"];
                }
                specialRegisters["PC"] += 3;
            }},

            {"INR", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"A", "3C"},
                        {"B", "04"},
                        {"C", "0C"},
                        {"D", "14"},
                        {"E", "1C"},
                        {"H", "24"},
                        {"L", "2C"},
                        {"M", "34"}
                    };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    bool carry = flags["C"];

                    if (registers.ContainsKey(RM))
                    {
                        CheckFlagsCarryAuxCarry(registers[RM], 1, '+', false);
                        registers[RM] += 1;
                        CheckFlagsZeroParitySign();
                    }
                    else if (RM == "M")
                    {
                        ushort addresInHL = GetValueFromRegisterPair("H");
                        CheckFlagsCarryAuxCarry(memory[addresInHL], 1, '+', false);
                        memory[addresInHL] += 1;
                        CheckFlagsZeroParitySign();
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");

                    }

                    flags["C"] = carry;
                }
                specialRegisters["PC"]++;
            }},

            {"INX", (RegPair) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"B",  "03"},
                            {"D",  "13"},
                            {"H",  "23"},
                            {"SP", "33"}
                        };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register pair");
                    }
                }
                else
                {
                    if (RegPair == "B" || RegPair == "D" || RegPair == "H")
                    {
                        ushort val = (ushort)(GetValueFromRegisterPair(RegPair) + 1);
                        SaveValueToRegisterPair(RegPair,val);
                    }
                    else if(RegPair == "SP")
                    {
                        specialRegisters["SP"] += 1;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"DCR", (RM) => {

                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"A", "3D"},
                        {"B", "05"},
                        {"C", "0D"},
                        {"D", "15"},
                        {"E", "1D"},
                        {"H", "25"},
                        {"L", "2D"},
                        {"M", "35"}
                    };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    bool carry = flags["C"];

                    if (registers.ContainsKey(RM))
                    {
                        CheckFlagsCarryAuxCarry(registers[RM], 1, '-', false);
                        registers[RM] -= 1;
                        CheckFlagsZeroParitySign();
                    }
                    else if (RM == "M")
                    {
                        ushort addresInHL = GetValueFromRegisterPair("H");
                        CheckFlagsCarryAuxCarry(memory[addresInHL], 1, '-', false);
                        memory[addresInHL] -= 1;
                        CheckFlagsZeroParitySign();
                    }
                flags["C"] = carry;
                }

                specialRegisters["PC"]++;
            }},

            {"DCX", (RegPair) => {

                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"B",  "0B"},
                            {"D",  "1B"},
                            {"H",  "2B"},
                            {"SP", "3B"},
                        };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register pair");
                    }
                }
                else
                {
                    if (RegPair == "B" || RegPair == "D" || RegPair == "H")
                    {
                        ushort val = (ushort)(GetValueFromRegisterPair(RegPair) - 1);
                        SaveValueToRegisterPair(RegPair,val);
                    }
                    else if (RegPair == "SP")
                    {
                        specialRegisters["SP"] -= 1;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"CMP", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"A", "BF"},
                            {"B", "B8"},
                            {"C", "B9"},
                            {"D", "BA"},
                            {"E", "BB"},
                            {"H", "BC"},
                            {"L", "BD"},
                            {"M", "BE"}
                        };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);

                    CheckFlagsCarryAuxCarry(registers["A"], valueFromParametr, '-', false);
                    registers["Temp"] = (byte)(registers["A"] + (byte)(~valueFromParametr + 1));
                    CheckFlagsZeroParitySign("Temp");
                }
                specialRegisters["PC"]++;
            }},

            {"CPI", (Data) => {
                byte DataVal;
                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("FE "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    CheckFlagsCarryAuxCarry(registers["A"], DataVal, '-', false);
                    registers["Temp"] = (byte)(registers["A"] + (byte)(~DataVal + 1));
                    CheckFlagsZeroParitySign("Temp");
                }
                specialRegisters["PC"] += 2;
            }},

            {"ANA", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"A", "A7"},
                            {"B", "A0"},
                            {"C", "A1"},
                            {"D", "A2"},
                            {"E", "A3"},
                            {"H", "A4"},
                            {"L", "A5"},
                            {"M", "A6"}
                        };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);

                    registers["A"] &= valueFromParametr;
                    CheckFlagsZeroParitySign();
                    flags["C"] = false;
                    flags["AC"] = true;
                }
                specialRegisters["PC"]++;
            }},

            {"ANI", (Data) => {
                byte DataVal;

                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("E6 "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    registers["A"] &= DataVal;
                    CheckFlagsZeroParitySign();
                    flags["C"] = false;
                    flags["AC"] = true;
                }
                specialRegisters["PC"] += 2;
            }},

            {"XRA", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"A", "AF"},
                            {"B", "A8"},
                            {"C", "A9"},
                            {"D", "AA"},
                            {"E", "AB"},
                            {"H", "AC"},
                            {"L", "AD"},
                            {"M", "AE"}
                        };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);
                    registers["A"] ^= valueFromParametr;
                    CheckFlagsZeroParitySign();
                    flags["C"] = false;
                    flags["AC"] = false;
                }
                specialRegisters["PC"]++;
            }},

            {"XRI", (Data) => {
                byte DataVal;

                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("EE "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    registers["A"] ^= DataVal;
                    CheckFlagsZeroParitySign();
                    flags["C"] = false;
                    flags["AC"] = false;
                }
                specialRegisters["PC"] += 2;
            }},

            {"ORA", (RM) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                        {
                            {"A", "B7"},
                            {"B", "B0"},
                            {"C", "B1"},
                            {"D", "B2"},
                            {"E", "B3"},
                            {"H", "B4"},
                            {"L", "B5"},
                            {"M", "B6"}
                        };

                    if(opcodes.ContainsKey(RM.ToUpper()))
                    {
                        translatedCode.Add(opcodes[RM]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    byte valueFromParametr = GetValueFromRegisterOrMemory(RM);
                    registers["A"] |= valueFromParametr;
                    CheckFlagsZeroParitySign();
                    flags["C"] = false;
                    flags["AC"] = false;
                }
                specialRegisters["PC"]++;
            }},

            {"ORI", (Data) => {
                byte DataVal;

                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Data = DataVal.ToString("X2");
                    translatedCode.Add("F6 "+Data);
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    registers["A"] |= DataVal;
                    CheckFlagsZeroParitySign();
                    flags["C"] = false;
                    flags["AC"] = false;
                }
                specialRegisters["PC"] += 2;
            }},

            {"JMP", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("C3",Label);
                }
                else
                {
                    ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    MoveToMemory(addressOfMemory);
                    return;
                }
                specialRegisters["PC"] += 3;
            }},

            {"JC", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("DA",Label);
                }
                else
                {
                    if(flags["C"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JNC", (Label) => {

                if(translatingCode)
                {
                    ProcessLabel("D2",Label);
                }
                else
                {
                    if(!flags["C"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JM", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("FA",Label);
                }
                else
                {
                    if(flags["S"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JP", (Label) => {


                if(translatingCode)
                {
                    ProcessLabel("F2",Label);
                }
                else
                {
                    if(!flags["S"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JZ", (Label) => {


                if(translatingCode)
                {
                    ProcessLabel("CA",Label);
                }
                else
                {
                    if(flags["Z"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JNZ", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("C2",Label);
                }
                else
                {
                    if(!flags["Z"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JPE", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("EA",Label);
                }
                else
                {
                    if(flags["P"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"JPO", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("E2",Label);
                }
                else
                {
                    if(!flags["P"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"PUSH" , (RegPair) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string> {
                        {"B", "C5"},
                        {"D", "D5"},
                        {"H", "E5"},
                        {"PSW", "F5"},
                    };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new Exception("On line "+lineCounter+":\nArgument must be register pair");
                    }
                }
                else
                {
                    ushort valueFromRegisterPair = GetValueFromRegisterPair(RegPair);
                    SaveValueOnStack(valueFromRegisterPair);
                }

            specialRegisters["PC"]++;
            }},

            {"POP" , (RegPair) => {
                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string> {
                        {"B", "C1"},
                        {"D", "D1"},
                        {"H", "E1"},
                        {"PSW", "F1"},
                    };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        translatedCode.Add(opcodes[RegPair]);
                    }
                    else
                    {
                        throw new Exception("On line "+lineCounter+":\nArgument must be register pair");
                    }
                }
                else
                {
                    LoadValueFromStack(RegPair);
                    if(RegPair == "PSW")
                    {
                        CheckFlags();
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"CALL", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("CD",Label);
                }
                else
                {
                    ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    specialRegisters["PC"] += 3;
                    SaveValueOnStack(specialRegisters["PC"]);
                    lineCounter = correspondingLinesToMemory[addressOfMemory];
                    specialRegisters["PC"] = addressOfMemory;
                    return;

                }
                specialRegisters["PC"] += 3;
            }},

            {"CC", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("DC",Label);
                }
                else
                {
                    if(flags["C"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"CNC", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("D4",Label);
                }
                else
                {
                    if(!flags["C"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"CP", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("F4",Label);
                }
                else
                {
                    if(!flags["S"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"CM", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("FC",Label);
                }
                else
                {
                    if(flags["S"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }
                }
                specialRegisters["PC"] += 3;
            }},

            {"CZ", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("CC",Label);
                }
                else
                {
                    if(flags["Z"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"CNZ", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("C4",Label);
                }
                else
                {
                    if(!flags["Z"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"CPE", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("EC",Label);
                }
                else
                {
                    if(flags["P"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"CPO", (Label) => {
                if(translatingCode)
                {
                    ProcessLabel("E4",Label);
                }
                else
                {
                    if(!flags["P"])
                    {
                        ushort addressOfMemory = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                        specialRegisters["PC"] += 3;
                        SaveValueOnStack(specialRegisters["PC"]);
                        MoveToMemory(addressOfMemory);
                        return;
                    }

                }
                specialRegisters["PC"] += 3;
            }},

            {"RST", (Data) => {
                byte DataVal = (byte)ParseAndCheckNumber(Data, 8);
                if(translatingCode)
                {
                    Dictionary<string,string> opcode = new Dictionary<string, string> {
                        {"0", "C7"},
                        {"1", "CF"},
                        {"2", "D7"},
                        {"3", "DF"},
                        {"4", "E7"},
                        {"5", "EF"},
                        {"6", "F7"},
                        {"7", "FF"},
                    };
                    if(opcode.ContainsKey(Data))
                    {
                        translatedCode.Add(opcode[Data]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument for RST must be 0-7.");
                    }
                }
                else
                {
                    specialRegisters["PC"] = (ushort)(DataVal*8);
                    return;
                }
                specialRegisters["PC"]++;
            }},
        };
        public Dictionary<string, Action<string>> SetOneParam
        {
            get { return setOneParam; }
        }

        private static Dictionary<string, Action<string, string>> setTwoParam { get; } = new Dictionary<string, Action<string, string>>
        {
            {"MOV", (RMd, RMs) => {

                if(translatingCode)
                {
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"AA", "7F"},
                        {"AB", "78"},
                        {"AC", "79"},
                        {"AD", "7A"},
                        {"AE", "7B"},
                        {"AH", "7C"},
                        {"AL", "7D"},
                        {"AM", "7E"},

                        {"BA", "47"},
                        {"BB", "40"},
                        {"BC", "41"},
                        {"BD", "42"},
                        {"BE", "43"},
                        {"BH", "44"},
                        {"BL", "45"},
                        {"BM", "46"},

                        {"CA", "4F"},
                        {"CB", "48"},
                        {"CC", "49"},
                        {"CD", "4A"},
                        {"CE", "4B"},
                        {"CH", "4C"},
                        {"CL", "4D"},
                        {"CM", "4E"},

                        {"DA", "57"},
                        {"DB", "50"},
                        {"DC", "51"},
                        {"DD", "52"},
                        {"DE", "53"},
                        {"DH", "54"},
                        {"DL", "55"},
                        {"DM", "56"},

                        {"EA", "5F"},
                        {"EB", "58"},
                        {"EC", "59"},
                        {"ED", "5A"},
                        {"EE", "5B"},
                        {"EH", "5C"},
                        {"EL", "5D"},
                        {"EM", "5E"},

                        {"HA", "67"},
                        {"HB", "60"},
                        {"HC", "61"},
                        {"HD", "62"},
                        {"HE", "63"},
                        {"HH", "64"},
                        {"HL", "65"},
                        {"HM", "66"},

                        {"LA", "6F"},
                        {"LB", "68"},
                        {"LC", "69"},
                        {"LD", "6A"},
                        {"LE", "6B"},
                        {"LH", "6C"},
                        {"LL", "6D"},
                        {"LM", "6E"},

                        {"MA", "77"},
                        {"MB", "70"},
                        {"MC", "71"},
                        {"MD", "72"},
                        {"ME", "73"},
                        {"MH", "74"},
                        {"ML", "75"},
                    };

                    if(opcodes.ContainsKey(RMd+RMs))
                    {
                        translatedCode.Add(opcodes[RMd+RMs]);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArguments must be register or memory.\nAlso 8085 doesn't support MOV M, M.");
                    }
                }
                else
                {
                    if(registers.ContainsKey(RMd) && registers.ContainsKey(RMs))
                    {
                        registers[RMd] = registers[RMs];
                    }
                    else if(RMd == "M")
                    {
                        memory[GetValueFromRegisterPair("H")] = registers[RMs];
                    }
                    else
                    {
                        byte valueFromMemory = GetValueFromMemory("H");
                        registers[RMd] = valueFromMemory;
                    }
                }
                specialRegisters["PC"]++;
            }},

            {"MVI", (RMd, Data) => {
                byte DataVal;

                if(translatingCode)
                {
                    DataVal = (byte)ParseAndCheckNumber(Data, 8);
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"A", "3E"},
                        {"B", "06"},
                        {"C", "0E"},
                        {"D", "16"},
                        {"E", "1E"},
                        {"H", "26"},
                        {"L", "2E"},
                        {"M", "36"}
                    };
                    if(opcodes.ContainsKey(RMd))
                    {
                        Data = DataVal.ToString("X2");
                        translatedCode.Add(opcodes[RMd]+" "+Data);
                    }
                    else
                    {
                        throw new ArgumentException("On line "+lineCounter+":\nArgument must be register or memory");
                    }
                }
                else
                {
                    DataVal = memory[specialRegisters["PC"] + 1];
                    if (registers.ContainsKey(RMd))
                    {
                        registers[RMd] = DataVal;
                    }
                    else if (RMd == "M")
                    {
                        memory[GetValueFromRegisterPair("H")] = DataVal;
                    }
                }
                specialRegisters["PC"] += 2;
            }},

            {"LXI", (RegPair, Data) => {
                ushort DataVal;

                if(translatingCode)
                {
                    DataVal = (ushort)ParseAndCheckNumber(Data,16);
                    Dictionary<string,string> opcodes = new Dictionary<string, string>
                    {
                        {"B",  "01"},
                        {"D",  "11"},
                        {"H",  "21"},
                        {"SP", "31"}
                    };
                    if(opcodes.ContainsKey(RegPair))
                    {
                        Data = Translate16BitNumber(DataVal);
                        translatedCode.Add(opcodes[RegPair]+" "+Data);
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line "+lineCounter+":\nArgument must be register pair.");
                    }
                }
                else
                {
                    DataVal = Get16bitValueFromMemory(specialRegisters["PC"] + 1);
                    SaveValueToRegisterPair(RegPair, DataVal);
                }
                specialRegisters["PC"] += 3;
            }},


        };
        public Dictionary<string, Action<string, string>> SetTwoParam
        {
            get { return setTwoParam; }
        }

        // POMOCNE FUNKCE

        private void Reset()
        {
            isHalted = false;
            isPaused = false;
            foreach (string key in registers.Keys.ToArray())
            {
                registers[key] = 0;
            }

            foreach (string key in flags.Keys.ToArray())
            {
                flags[key] = false;
            }
            specialRegisters["PC"] = programStart;
            specialRegisters["PSW"] = 0;
            translatedCode.Clear();
            translationWaitingForLabelAddress.Clear();
            instructionsWaitingForLabelAddress.Clear();
            memory = new byte[0xFFFF];
            correspondingLinesToMemory = new int[0xFFFF];
            labels.Clear();
            lineCounter = 0;
        }

        private static void CheckFlagsZeroParitySign(string register = "A")
        {
            flags["Z"] = registers[register] == 0;

            int numberOfOnesInAcc = Convert.ToString(registers[register], 2).Replace("0", "").Length;
            flags["P"] = numberOfOnesInAcc % 2 == 0;

            flags["S"] = (sbyte)registers[register] < 0;
            //CalculatePSW();
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
                        ((a & 0xF) - ((b + carry) & 0xF)) > 0xF :
                        ((a & 0xF) - (b & 0xF)) > 0xF;
                    break;
            }
            //CalculatePSW();
        }

        private static byte GetValueFromRegisterOrMemory(string RM)
        {

            byte valueFromParametr = 0;
            if (registers.ContainsKey(RM))
            {
                valueFromParametr = registers[RM];
            }
            else if (RM == "M")
            {
                valueFromParametr = GetValueFromMemory("H");
            }
            else
            {
                throw new WrongArgumentsException("On line " + lineCounter + ":\nArgument must be register or memory");
            }
            return valueFromParametr;
        }

        private static void SaveValueOnStack(ushort value)
        {
            byte highNibble = (byte)((value >> 8) & 0xff);
            byte lowNibble = (byte)((value & 0xff));
            memory[(ushort)(--specialRegisters["SP"])] = highNibble;
            memory[(ushort)(--specialRegisters["SP"])] = lowNibble;
        }

        private static void LoadValueFromStack(string RegPair)
        {
            byte lowNibble = memory[(ushort)(specialRegisters["SP"])++];
            byte highNibble = memory[(ushort)(specialRegisters["SP"])++];
            int value = highNibble << 8;
            value += lowNibble;
            SaveValueToRegisterPair(RegPair, value);
        }

        private static void SaveValueToRegisterPair(string regPair, int value)
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

                case "SP":
                    specialRegisters["SP"] = val;
                    break;

                case "PC":
                    specialRegisters["PC"] = val;
                    break;

                case "PSW":
                    registers["A"] = highNibble;
                    specialRegisters["PSW"] = lowNibble;
                    break;
                default:
                    if (regPair == "C" || regPair == "E" || regPair == "L")
                    {
                        throw new WrongArgumentsException("On line " + lineCounter + ":\nRegister pairs are being reference by B for BC, D for DE and H for HL.");
                    }
                    else
                    {
                        throw new WrongArgumentsException("On line " + lineCounter + ":\nArgument is not a register pair.");
                    }
            }
        }

        private static void CheckCarryFor16bitNumbers(int a, int b)
        {
            flags["C"] = (a + b) > ushort.MaxValue;
            CalculatePSW();
        }

        private static ushort GetValueFromRegisterPair(string Reg)
        {
            ushort address = Reg switch
            {
                "B" => (ushort)((registers["B"] << 8) + registers["C"]),
                "D" => (ushort)((registers["D"] << 8) + registers["E"]),
                "H" => (ushort)((registers["H"] << 8) + registers["L"]),
                "PSW" => (ushort)((registers["A"] << 8) + specialRegisters["PSW"]),
                _ => Convert.ToUInt16(Reg)
            };

            return address;
        }

        private static ushort Get16bitValueFromMemory(int address)
        {
            if (address <= ushort.MaxValue)
            {
                byte lowNibble = memory[address];
                byte highNibble = memory[address + 1];
                ushort ret = (ushort)(highNibble << 8);
                ret += lowNibble;
                return ret;
            }

            return 0;
        }

        private static byte GetValueFromMemory(string Reg)
        {
            ushort address = GetValueFromRegisterPair(Reg);
            return memory[address];
        }

        private static bool CheckIfNumberIsInRange(int number, int numberOfBits)
        {
            double range = Math.Pow(2, numberOfBits);
            return -range <= number && number < range;
        }

        private static int ParseAndCheckNumber(string number, int numberOfBits)
        {
            Regex r;
            if (Regex.IsMatch(number, "H$"))
            {
                r = new Regex(@"^[-]{0,1}[A-F0-9]{1,5}H{0,1}$");
            }
            else
            {
                r = new Regex(@"^[-]{0,1}[0-9]{1,5}$");
            }

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
                    throw new WrongArgumentsException("On line " + lineCounter + ":\nNumber is too big or too small.");
                }
                return DataVal;
            }
            else
            {
                throw new WrongArgumentsException("On line " + lineCounter + ":\nNumber is too big or too small.");
            }

        }

        private static string Translate16BitNumber(ushort number)
        {
            byte highNibble = (byte)((number >> 8) & 0xff);
            byte lowNibble = (byte)((number & 0xff));
            string hn = highNibble.ToString("X2");
            string ln = lowNibble.ToString("X2");
            return ln + " " + hn;
        }

        private static void CheckForLabel(string line, int lineCounter)
        {
            string label;
            if (Regex.IsMatch(line, "^[A-Z]+[A-Z0-9]*:.*"))
            {
                label = line.Split(":")[0].Trim();
                if (label.Split(" ").Length > 1)
                {

                    throw new WrongArgumentsException("On line " + lineCounter + ":\nInvalid label name: " + label);
                }
                else
                {
                    if (labels.ContainsKey(label))
                    {
                        throw new WrongArgumentsException("On line " + lineCounter + ":\nDuplicate labels: " + label);
                    }
                    else
                    {
                        labels.Add(label, specialRegisters["PC"]);
                        if (instructionsWaitingForLabelAddress.ContainsKey(label))
                        {
                            // ushort address = instructionsWaitingForLabelAddress[label];
                            foreach (ushort addr in instructionsWaitingForLabelAddress[label])
                            {
                                ushort translatedCodeLine = translationWaitingForLabelAddress[label][0];
                                string[] nibbles = Translate16BitNumber(specialRegisters["PC"]).Split(" ");

                                memory[addr] = Convert.ToByte(nibbles[0], 16);
                                memory[addr + 1] = Convert.ToByte(nibbles[1], 16);
                                translatedCode[translatedCodeLine] += " " + nibbles[0] + " " + nibbles[1];
                                translationWaitingForLabelAddress[label].RemoveAt(0);


                            }
                            instructionsWaitingForLabelAddress.Remove(label);
                        }
                    }
                }
            }
        }

        private static void ProcessLabel(string opcode, string Label)
        {
            ushort addressOfMemory = 0;
            if (labels.ContainsKey(Label))
            {
                addressOfMemory = labels[Label];

                translatedCode.Add(opcode + " " + Translate16BitNumber(addressOfMemory));
            }
            else
            {
                if (Regex.IsMatch(Label, @"^[0-9]+") == true || Regex.IsMatch(Label, @"^[0-9A-F]+H$") == true)
                {
                    addressOfMemory = (ushort)ParseAndCheckNumber(Label, 16);
                    translatedCode.Add(opcode + " " + Translate16BitNumber(addressOfMemory));
                }
                else
                {
                    if (!instructionsWaitingForLabelAddress.ContainsKey(Label))
                    {
                        instructionsWaitingForLabelAddress.Add(Label, new List<ushort>());
                    }
                    if (!translationWaitingForLabelAddress.ContainsKey(Label))
                    {
                        translationWaitingForLabelAddress.Add(Label, new List<ushort>());
                    }
                    instructionsWaitingForLabelAddress[Label].Add((ushort)(specialRegisters["PC"] + 1));
                    translationWaitingForLabelAddress[Label].Add((ushort)lineCounter);
                    translatedCode.Add(opcode);
                }
            }
        }

        private static void MoveToMemory(ushort addressOfMemory)
        {
            lineCounter = correspondingLinesToMemory[addressOfMemory];
            specialRegisters["PC"] = addressOfMemory;
        }

        private static void CalculatePSW()
        {
            ushort tmp = 0;
            tmp += (flags["S"]) ? (ushort)(128) : (ushort)0;
            tmp += (flags["Z"]) ? (ushort)(64) : (ushort)0;
            tmp += (flags["AC"]) ? (ushort)(16) : (ushort)0;
            tmp += (flags["P"]) ? (ushort)(4) : (ushort)0;
            tmp += (flags["C"]) ? (ushort)(1) : (ushort)0;
            specialRegisters["PSW"] = tmp;
        }

        private static void CheckFlags()
        {
            flags["S"] = ((specialRegisters["PSW"] & 128) > 0);
            flags["Z"] = ((specialRegisters["PSW"] & 64) > 0);
            flags["AC"] = ((specialRegisters["PSW"] & 16) > 0);
            flags["P"] = ((specialRegisters["PSW"] & 4) > 0);
            flags["C"] = ((specialRegisters["PSW"] & 1) > 0);
        }

        public void ReadLines()
        {
            Reset();

            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                string[] line;
                uint lastPCindex = programStart;
                translatingCode = true;
                while ((s = sr.ReadLine()) != null)
                {
                    lastPCindex = specialRegisters["PC"];
                    s = s.ToUpper();
                    CheckForLabel(s, lineCounter);
                    s = Regex.Replace(s, @"#.*", "");
                    s = Regex.Replace(s, @".*:(.*)", "$1");
                    s = Regex.Replace(s, @"^\s+", "");
                    s = Regex.Replace(s, @"\s+$", "");
                    s = Regex.Replace(s, @"\s{2,}", " ");
                    s = Regex.Replace(s, @"([ABCDEFHLM]{1})\s*,\s*", "$1, ");  //odstraneni mezery mezi prvnim parametrem a carkou
                    s = Regex.Replace(s, @"\s*-\s*", " -");
                    parsedCode.Add(s);

                    line = s.Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();

                    switch (line.Length)
                    {
                        case 0:
                            translatedCode.Add("");
                            break;
                        case 1:
                            if (!setZeroParam.ContainsKey(line[0]))
                            {
                                throw new WrongArgumentsException("On line " + lineCounter + ":\nUnkown instruction " + line[0]);
                            }
                            setZeroParam[line[0]]();
                            break;

                        case 2:
                            if (!setOneParam.ContainsKey(line[0]))
                            {
                                throw new WrongArgumentsException("On line " + lineCounter + ":\nUnkown instruction " + line[0]);
                            }
                            setOneParam[line[0]](line[1]);
                            break;

                        case 3:
                            if (!setTwoParam.ContainsKey(line[0]))
                            {
                                throw new WrongArgumentsException("On line " + lineCounter + ":\nUnkown instruction " + line[0]);
                            }

                            if ((line[1] = line[1].Replace(" ", "")).EndsWith(","))
                            {
                                line[1] = line[1].Replace(",", "");
                                setTwoParam[line[0]](line[1], line[2]);
                            }
                            else
                            {
                                throw new WrongArgumentsException("On line " + lineCounter + ":\nExcepted comma after first argument.");
                            }

                            break;

                        default:
                            throw new WrongArgumentsException("On line " + lineCounter + ":\nToo many arguments.");
                    }
                    for (var i = lastPCindex; i < specialRegisters["PC"]; i++)
                    {
                        correspondingLinesToMemory[i] = lineCounter;
                    }
                    lastPCindex = specialRegisters["PC"];
                    lineCounter++;
                }
                if (instructionsWaitingForLabelAddress.Count > 0)
                {
                    var tmp = "";
                    foreach (var i in instructionsWaitingForLabelAddress.Keys.ToList())
                    {
                        tmp += i + "\n";
                    }
                    throw new WrongArgumentsException("There are missing labels:\n" + tmp);
                }
                loadParsedCodeToMemory();
                sr.Close();
                specialRegisters["PC"] = programStart;
                translatingCode = false;
            }
        }

        private static void loadParsedCodeToMemory()
        {
            int counter = programStart;
            foreach (string line in translatedCode)
            {
                if (line.Length == 0)
                    continue;
                string[] bytes = line.Split(" ");
                foreach (string b in bytes)
                {
                    if (memory[counter] == 0)
                    {
                        memory[counter] = Convert.ToByte(b, 16);
                    }
                    counter++;
                }
            }
        }

        public void InterpretLines()
        {
            specialRegisters["PC"] = programStart;
            while (!isHalted)
            {
                byte val = memory[specialRegisters["PC"]];
                instructions[val]();
                CalculatePSW();
                lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
                while (isPaused) ;
            }
            Reset();
        }

        public void InterpretSingleLine()
        {
            if (!isHalted)
            {
                byte val = memory[specialRegisters["PC"]];
                instructions[val]();
                CalculatePSW();
                lineCounter = correspondingLinesToMemory[specialRegisters["PC"]];
            }
        }
    }
}


