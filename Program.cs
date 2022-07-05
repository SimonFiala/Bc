
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Bc
{
    class Program
    {
        static void Main(string[] args)
        {

            //byte a = 0x10;
            //byte b = 0xa0;
            //Console.WriteLine($"{(byte)(a + b)}    {(sbyte)(a + b)}\n\n");
            // byte h = 200;
            // byte l = 100;
            // byte o = (byte)(h + l);
            // Console.WriteLine(o);

            //if(h+l > 255)
            //{
            //    Console.WriteLine((byte)(h + l) + "\n");
            //    Console.WriteLine("carry\n");
            //} else
            //{
            //    Console.WriteLine(Convert.ToString((byte)(h + l), 16) + "\n");
            //    Console.WriteLine("no carry\n");
            //}
            //byte x = 0x05;
            //byte y = 0x07;
            //Console.WriteLine(Convert.ToString((x & 0xF) + (~y & 0xF)+1, 2));
            //Console.WriteLine((x & 0xF) + ((~y & 0xF) + 1) > 0xF);
            //byte h = 0b1100;
            //byte l = 0x1;
            //ushort hl = (ushort)((h << 8) + l);
            //Console.WriteLine(hl);


            Instructon_Set iset = new Instructon_Set();
            //var set0 = iset.SetZeroParam;
            //var set1 = iset.SetOneParam;
            //var set2 = iset.SetTwoParam;




            iset.ReadLines(@"asm.txt");
            iset.InterpretLines();

            Console.WriteLine("\nLabels:");
            foreach (var e in iset.Labels)
            {
                Console.WriteLine(e);
            }
            System.Console.WriteLine("Registers:");
            foreach (var a in iset.Registers)
            {
                Console.WriteLine(a.Key + "     " + a.Value.ToString("X2"));
            }
            foreach (var f in iset.SpecialRegisters)
            {
                Console.WriteLine(f.Key + "     " + f.Value.ToString("X4"));
            }
            Console.WriteLine("\nFlags:");
            foreach (var b in iset.Flags)
            {
                Console.WriteLine(b.Key + "     " + Convert.ToByte(b.Value));
            }
            Console.WriteLine("\nMemory:");
            int counter = 0;
            foreach (var c in iset.Memory)
            {
                if (c != 0)
                {
                    Console.WriteLine("0x" + counter.ToString("X4") + " - " + c.ToString("X2"));
                }
                counter++;
            }
            Console.WriteLine("\nTranslated Code:");
            foreach (var d in iset.TranslatedCode)
            {
                Console.WriteLine(d);
            }
            // Console.WriteLine("\nCorresponding lines:");
            // foreach (var g in iset.CorrespondingLinesToMemory)
            // {
            //     if (g != 0)
            //     {
            //         Console.WriteLine(g);
            //     }
            // }


            //Dictionary<ushort, byte> memory = new Dictionary<ushort, byte> { };
            //memory[3005] = (byte)20;
            //foreach (var x in memory)
            //{
            //    Console.WriteLine(x);
            //}
            //memory[3005] = (byte)50;
            //foreach (var x in memory)
            //{
            //    Console.WriteLine(x);
            //}

        }

    }
}
