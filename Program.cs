using System;
using System.Collections.Generic;

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
            var set1 = iset.SetOneParam;
            var set2 = iset.SetTwoParam;

            set2["LXI"]("h", "16464");
            set2["MVI"]("M", "bbh");
            set2["LXI"]("h", "16465");
            set2["MVI"]("M", "aah");
            set1["LHLD"]("4050h");



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
                Console.WriteLine(Convert.ToString(c.Key, 16).ToUpper() + "   " + c.Value);
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
