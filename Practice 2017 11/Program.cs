using System;

// Задание №5 практики 2017г.
// Задание 838, стр. 167: Доказать, что  матрица  [a,y]i=1..10  j=1..10  может  служить  ключом шифра, если из элементов: a[i,j], a[10-i+1, j], a[i, 10-j+1], a[10-i+1, 10-j+1], только  один  равен  нулю.
// Ссылка на "Задачи по программированию": http://ideafix.name/wp-content/uploads/stuff/book95.pdf

namespace Practice_2017_11
{
    class Program
    {
        static Random Rnd=new Random();

        const int n = 10; // Константа - размерность матрицы

        static string GenerateText()
        {
            string text = "";
            const string alphabet = "abcdefghijklmnopqrstuvwxyz"; // Алфавит для текста
            
            for (int i = 0; i < n*n; i++)
                text += alphabet[Rnd.Next(alphabet.Length)];      // Выбор случайных элементов

            return text;
        }                                                 // Генерация текста
        static bool[,] GenerateKeyMatr()
        {
            bool[,] keyMatr = new bool[n, n];            // true = прорезь!

            int i = Rnd.Next(n) <= 5 ? 0 : 5, k = i + 5, // Выбираем случайную половину рядом
                j = Rnd.Next(n) <= 5 ? 0 : 5, l = j + 5; // Выбираем случайную половину столбцов
            for (i = i; i < k; i++)                      // Таким образом создаётся четверть заполненная "дырками". Во-первых такой генератор быстрее и проще, а во-вторых работа с такой матрицей-ключом нагляднее
            {
                for (j = j; j < l; j++)
                    keyMatr[i, j] = true;
                
                j -= 5;                                  // Сбрасывание обхода столбцов
            }
            return keyMatr;
        }                                             // Генерирует простую ключ-матрицу

        static void PrintMatr(bool[,] matr)
        {                    // Матрица для печати
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Console.Write((matr[i,j] ? 0 : 1)+" ");
                
                Console.WriteLine();
            }
        }                                          // Печать булевой матрицы (true=0, false=1)
        static void PrintMatr(char[,] matr)
        {                    // Матрица для печати
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Console.Write(matr[i, j] + " ");

                Console.WriteLine();
            }
        }                                          // Печать буквенной матрицы

        static bool[,] TurnMatrixBy90Clockwise(bool[,] matrix)
        {                                      // Матрица для поворота
            int order = matrix.GetLength(0);                 // Узнаём размерность матрицы
            bool[,] newMatrix=new bool[order, order];        // Создаём матрицу такой же размерности

            for (int i = 0; i < order; i++)
                for (int j = 0; j < order; j++)
                    newMatrix[i, j]= matrix[order - j-1, i]; // Записываем элементы с нужных позиций
            
            return newMatrix;
        }                       // Поворот матрицы по часовой стрелке на 90*

        static char[,] Encrypt(string text, bool[,] keyMatr, char[,] encryptedMatr)
        {                     // Текст для шифровки, матрица-ключ, матрица с зашифрованным текстом
            if (text == "") return encryptedMatr;                                                   // Если текст кончился, то шифрованная матрица зашифрована

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (keyMatr[i, j])                                                              // Если в ключе-матрице "дыра", значит букву при наложение требуется переписать в зашифрованную матрицу
                    {
                        encryptedMatr[i,j] = text[0];                                               // Берём всегда первый элемент, т.к. после записи он будет удалён
                        text=text.Remove(0, 1);                                                     // Удаляем первый элемент
                        if (text.Length%(n*n/4) == 0)                                               // Если длина текста кратна четверти длины матрицы, значит обход закончен
                            return Encrypt(text, TurnMatrixBy90Clockwise(keyMatr), encryptedMatr);  // Вызов рекурсии с поворотом матрицы-ключа на 90* по часовой стрелке
                        
                    }

            return new char[0,0];
        }
        static char[,] Cipher(string text, bool[,] keyMatr)
        {                    // Текст для шифровки, матрица-ключ
            return Encrypt(text, keyMatr, new char[n, n]); // Вызов рекурсивной функции для заполнения зашифрованной матрицы
        }                          // Занимается шифровкой текста по матрице-ключу

        static string Decrypt(char[,] encryptedMatr, bool[,] keyMatr, string text)
        {                     // Зашифрованная матрица, матрица-ключ и расшифрованный текст
            if (text.Length == n * n) return text;                                                  // Если длина текста равна длине матрицы, значит дешифровка окончена

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (keyMatr[i, j])                                                              // Если в ключе-матрице "дыра", значит букву при наложение требуется переписать в текст
                    {
                        text += encryptedMatr[i, j];
                        if (text.Length % (n * n / 4) == 0)                                         // Если длина текста равна четверти длины матрицы, то нужное количество букв на этоп этапе дешифровано
                            return Decrypt(encryptedMatr, TurnMatrixBy90Clockwise(keyMatr), text);  // Вызов рекурсии, матрица-ключ поворачивается по часовой
                    }

            return "";                                                                              // Недостижимый возврат в случае корректных данных
        }   // Рекурсивная дешифровка зашифрованной матрицы
        static string Decipher(char[,] encryptedMatr, bool[,] keyMatr)
        {                      // Зашифрованная матрица и матрица-ключ
            return Decrypt(encryptedMatr, keyMatr,""); // Вызов рекурсивной функции дешифровки
        }               // Занимается расшифровой

        static void ShowCryptingAndDecrypting(string text, bool[,] keyMatr)
        {                                    // Текст для шифровки и матрица-ключ
            Console.WriteLine("Начальный текст:\n"+text+"\n\nМатрица-ключ:");
            PrintMatr(keyMatr);

            char[,] encryptedMatr = Cipher(text, keyMatr); // Шифрует текст по ключу-матрице
            Console.WriteLine("\nЗашифрованный текст превратился в матрицу с помощью матрицы-ключа:");
            PrintMatr(encryptedMatr);

            Console.WriteLine("\nЗашифрованная матрица расшифровывается с помощью матрицы-ключа в первоначальный текст:\n"+Decipher(encryptedMatr, keyMatr));
        }          // Показывает работу шифратора и дешифратора

        public static int Menu(params string[] items)
        {
            int[] span = new int[items.Length + 1];
            for (int i = 1; i < span.Length; i++)
            {
                span[i] = items[i - 1].Length / Console.WindowWidth + 1;
            }

            Console.CursorVisible = false;
            for (int i = 0; i < items.Length; i++)
                Console.WriteLine("\n");
            int positionY = Console.CursorTop - 2 * items.Length + 1;
            int currentIndex = 0, previousIndex = 0;
            int positionX = 2;
            bool itemSelected = false;

            int sum = 0;
            //Начальный вывод пунктов меню.
            for (int i = 0; i < items.Length; i++)
            {
                sum += span[i];
                Console.CursorLeft = positionX;
                Console.CursorTop = positionY + sum;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(items[i]);
            }

            do
            {
                sum = 0;
                for (int i = 0; i <= previousIndex; i++)

                    sum += span[i];

                // Вывод предыдущего активного пункта основным цветом.
                Console.CursorLeft = positionX;
                Console.CursorTop = positionY + sum;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(items[previousIndex]);

                sum = 0;
                for (int i = 0; i <= currentIndex; i++)

                    sum += span[i];


                //Вывод активного пункта.
                Console.CursorLeft = positionX;
                Console.CursorTop = positionY + sum;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.Write(items[currentIndex]);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                previousIndex = currentIndex;
                switch (keyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        currentIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        currentIndex--;
                        break;
                    case ConsoleKey.Enter:
                        itemSelected = true;
                        break;
                }

                if (currentIndex == items.Length)
                    currentIndex = 0;
                else if (currentIndex < 0)
                    currentIndex = items.Length - 1;
            } while (!itemSelected);
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            return currentIndex + 1;
        }                                // Позволяет быстро организовать меню, введя его пункты

        static void Main(string[] args)
        {
            string text=GenerateText();          // Текст для шифровкм
            bool[,] keyMatr = GenerateKeyMatr(); // Матрица-ключ с помощью, которой шифруется текст
            
            while (true)
            {
                switch (Menu("Вывести текст", "Вывести матрицу-ключ", "Сгенерировать текст", "Сгенерировать матрицу-ключ", "Зашифровать текст, вывести шифрованную матрицу и расшифровать её", "Выход"))
                {         // 1. Вывод текста  2. Вывод матрицы-ключа   3. Генерация текста   4. Генерация матрицы-ключа    5. Показывает работы шифровки и дешифровки                          5. Выход из программы
                    case 1: Console.WriteLine(text); break;
                    case 2: PrintMatr(keyMatr); break;
                    case 3: text = GenerateText(); Console.WriteLine(text); break;
                    case 4: keyMatr = GenerateKeyMatr(); Console.WriteLine("Берётся один случайны элемент, из него следует какая четверть будет наполнена 'дырками', таким образом поставленное правило соблюдается (см.задачу)"); PrintMatr(keyMatr); break;
                    case 5: ShowCryptingAndDecrypting(text, keyMatr); break;
                    default: return;
                }
            }
        }                                              // Содержит меню для работы с программой
    }
}
