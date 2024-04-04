using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Linq;

namespace countofribs 
{
    class Program 
    {
        static void Main(string[] args)
        {
            int size;
            int count = 0;

            Console.Write("Введите размер матрицы: ");

            size = Convert.ToInt32(Number(Console.ReadLine()));

            Console.WriteLine($"Теперь введите веса дуг массива через пробел {size} раз(а), нажимая в конце Enter!");

            for (int i = 0; i < size; i++)
            {
                string[] weights = new string[size];
                weights = Console.ReadLine().Trim().Split();

                for (int j = 0; j < size; j++)
                {
                    if (Convert.ToInt32(weights[j]) != 0)
                    {
                        count += 1;
                    }
                }
            }

            Console.WriteLine("Колличество ребер: " + count/2);
        }

        private static string Number (string s)
        {
            while (!int.TryParse(s, out _) | string.IsNullOrWhiteSpace(s) | s.Contains(" "))
            {
                Console.Write("Введите число в месте ошибки, без пробела!: ");

                s = Console.ReadLine();
            }

            return s;
        }
    }
}
