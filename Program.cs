using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bc
{
    class Program
    {
        static void Main(string[] args)
        {

            //byte a = 0x10;
            //byte b = 0xa0;
            //Console.WriteLine($"{(byte)(a + b)}    {(sbyte)(a + b)}\n\n");

            //byte h = 200;
            //byte l = 56;
            //byte o = (byte)(h + l);
            //Console.WriteLine();



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


            iset.ReadLines(@"C:\Users\Šimon Fiala\Desktop\Bc testovaci soubor.txt");


            

            foreach (var a in iset.Registers)
            {
                Console.WriteLine(a.Key + "     " + Convert.ToString(a.Value, 16).ToUpper());
            }
            Console.WriteLine("\n");
            foreach (var b in iset.Flags)
            {
                Console.WriteLine(b.Key + "     " + Convert.ToByte(b.Value));
            }
            foreach (var c in iset.Memory)
            {
                Console.WriteLine(Convert.ToString(c.Key, 16).ToUpper() + "   " + Convert.ToString(c.Value, 16).ToUpper());
            }



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
