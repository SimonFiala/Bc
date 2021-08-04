using System;

namespace Bc
{
    class Program
    {
        static void Main(string[] args)
        {

            byte a = 0x10;
            byte b = 0xa0;
            Console.WriteLine($"{(byte)((sbyte)(a + b))}    {(sbyte)(a + b)}\n\n");

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
            //byte x = 5;
            //Console.WriteLine(Convert.ToString((x & 0xF) + (x & 0xF), 2));
            //Console.WriteLine((x & 0xF) + (x & 0xF) > 0xF);
        }
    }
}
