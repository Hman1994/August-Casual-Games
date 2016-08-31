using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Login
{
   public class Program
    {
        static void Main(string[] args)
        {
            string input;
            Console.WriteLine("Please enter your username ");
            input = Console.ReadLine();

            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Users\\Alan\\Documents\\College Stuff\\Casual Games\\Repeat Assignment\\Repeat Project\\GameAssignment\\PlayerHistory\\userHistory.txt");
            file.WriteLine(input);

            file.Close();

            if (input != null)
            {
                Environment.Exit(0);
                
                
             
            }

        }
    }
}
