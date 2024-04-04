using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace shortestway
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[]> adjacency = new List<double[]>();
            List<List<double>[]> D = new List<List<double>[]>();
            List<int> start = new List<int>();
            List<int> viewed = new List<int>();

            string[] text = ["Вершина", "Растояние", "Маршрут"];

            double infinity = Math.Pow(10, 31);

            int size;
            int space;
            int first;

            bool stop;

            Stopwatch stopwatch = new Stopwatch();

            Console.Write("Введите размер матрицы: ");

            size = Convert.ToInt32(Number(Console.ReadLine()));

            space = text.Select(n => n.Length).Max() + 1; // Для форматированного вывода

            Console.WriteLine($"Теперь введите веса дуг массива через пробел {size} раз(а), нажимая в конце Enter!");

            for (int i = 0; i < size; i++)
            {
                adjacency.Add(ConvertToRight(Console.ReadLine().Trim().Split(), infinity, size)); // Заполнение матрицы

                List<double> vertex = new List<double>(){ i, infinity}; // Список{вершина, раст-е до вершины(по умолчанию - бесконечность)} - Коробка
                List<double> vertices = new List<double>(); // Список{начало - промежуточные вершины - конечное} - Коробка
                List<double>[] array = [vertex, vertices]; // Массив[ Список{вершина,раст-е до вершины(по умолчанию - бесконечность)}, Список{начало - промежуточные вершины - конечное} ] - Полка

                D.Add(array); // Заполнение вспомогательного списка, содержащего инф о вершине, начального растояния до неё и маршрут до конечн. вершины - Шкаф
            }

            Console.Write("Выберите откуда начать построение маршрута (номер вершины): ");

            first = Convert.ToInt32(Number(Console.ReadLine()));

            start.Add(first);

            D[start[0]][0][1] = 0; // Растояние до отправной вершины
            D[start[0]][1].Add(start[0]); // Маршрут до отправной вершины - сама вершина

            stopwatch.Start();

            for (int k = 0; k < size; k++) // Для неск-го прохода по графу и нахожд-я оптим маршрута при переоценке раст-ий до вершин
            {
                stop = true;

                for (int i = 0; i < size; i++) // Для прохода по вершина такое кол-во раз, в зав-ти от кол-ов этих вершин
                {

                    for (int j = 0; j < size; j++) // От нашей текущ вершины смотрим на следующие(j)
                    {
                        if (D[start[0]][0][1] + adjacency[start[0]][j] < D[j][0][1] && !D[start[0]][1].Contains(j) && start[0] != j) // Если: длина пред. пути до текущ. верш. + от текущ. верш. до расм. меньше прошл. пути до расм. верш.; не было 0->5->2->6->2 т. к нет смысла см. 2->6->2; нет петель 5->5
                        {
                            D[j][0][1] = D[start[0]][0][1] + adjacency[start[0]][j]; // Изм-е знач. раст-я на более короткое во вспомогат. списке

                            // Перезапись маршрута до расм. вершины - истории пути по вершинам
                            D[j][1].Clear();
                            D[j][1] = new List<double>(D[start[0]][1]);
                            D[j][1].Add(j);

                            stop = false;
                        }

                        if (adjacency[start[0]][j] != infinity && !viewed.Contains(j) && !start.Contains(j)) // Добавление след. вершин по которым нужно пройтись если: есть связь; не была ранее просмотрена; не была добавлена ранее
                        {
                            start.Add(j);
                        }
                    }
                    viewed.Add(start[0]); // Добавление в просмотренные вершины, чтобы не было сит-ии от 0 -> 1 -> 0 -> ... или 0 -> 1 -> 0 -> 2 -> 5 -> 6 -> 1
                    start.RemoveAt(0); // Удаление просмотренной вершины для перехода на просм. след-их вершин
                }
                
                if (stop) // Оптимизация кода - избавление от лишних проходов если не было переоценок хотя бы 1-го пути до какой-либо вершины
                {
                    break;
                }

                // Для повторного прохода по всему графу
                viewed.Clear();
                start.Add(first);
            }

            stopwatch.Stop();

            Console.WriteLine(text[0].PadRight(space) + text[1].PadRight(space) + text[2].PadRight(space)); // Форматированный вывод вершины растояния и маршрута

            for (int i = 0; i < size; i++) // Форматированный вывод вершины растояния и маршрута
            {
                Console.WriteLine(Convert.ToString(D[i][0][0]).PadRight(space) + Convert.ToString(D[i][0][1]).PadRight(space) + string.Join(" ", D[i][1].Select(n => n.ToString()).ToList()));
            }

            Console.WriteLine($"Время выполнения алгоритма: {stopwatch.Elapsed}");
        }

        private static string Number (string value) // Проверка на численное значение
        {
            while (!int.TryParse(value, out _) | string.IsNullOrWhiteSpace(value) | value.Contains(" "))
            {
                Console.Write("Введите число в месте ошибки, без пробела!: ");

                value = Console.ReadLine();
            }

            return value;
        }

        private static double[] ConvertToRight (string[] weights,double infinity, int size) // Преобразование матрицы в требуемый вид
        {
            while (weights.Count() != size) // Проверка на совпадение длинны матрицы с требуемой
            {
                Console.WriteLine("Введите веса заново, согласно условию!:");

                weights = Console.ReadLine().Trim().Split();
            }

            for (int i = 0; i < weights.Length; i++) // Проверка на значения весов
            {
                weights[i] = Number(weights[i]);
            }

            double[] result = new double[weights.Length]; // Возвращаемый массив чисел

            for (int i = 0; i < weights.Length; i++) // Преобразование типа элементов матрицы
            {
                if (double.TryParse(weights[i], out double number))
                {
                    if (number == 0)
                    {
                        number = infinity;
                    }

                    result[i] = number;
                }
                else
                {
                    result[i] = 0.0;
                }
            }

            return result;
        }
    }
}
