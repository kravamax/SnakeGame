using System;
using System.IO;
using static System.Console;
using System.Diagnostics;
using System.Threading;             // класс запускает метод Loop
using System.Collections.Generic;   //обобщенные или типизированные классы коллекций (IEnumerable <T> name = new IEnumerable <T>() {};)
using System.Linq;                  //пространство имен предоставляет классы и интерфейсы, которые поддерживают запросы                 

namespace SnakeGame
{
    class Game
    {
        static readonly int x = 80;                     // длина игровой области по горизонтали
        static readonly int y = 36;                     // длина игровой области по вертикали

        static Walls walls;                              //объявляем объект внешних стен
        static Snake snake;
        static FoodFactory foodFactory;                 // создает объект еды
        static Timer time;
        static int scores = 0;
        static int level = 0;
        static int diflevel;                            // уровни сложности
        static int snakeSpeed = 150;                    // начальная скорость змеи
        static readonly Stopwatch stopWatch = new Stopwatch();
        static void Main()
        {            
            SetWindowSize(x + 1, y + 1);        // устанавливает размер окна консоли
            SetBufferSize(x + 1, y + 1);        // устанавливает размер буфера (убирает панель прокрутки)
            CursorVisible = false;              // скрываем курсор                                          

            TittlePage();

            GameMenu();

            StartGame();            
        }
        static void Loop(object obj)
        {            
            if (walls.IsHit(snake.GetHead()) || snake.IsHit(snake.GetHead()))
            {
                time.Change(0, Timeout.Infinite);
                GameOver();
                GameOverMenu();
            }
            else if (snake.Eat(foodFactory.Food))
            {
                foodFactory.CreateFood(diflevel);
                if (diflevel == 0)                                                           // система начисления очков для легкого уровня
                {
                    scores++;
                    if (scores % 3 == 0)
                    {
                        if (scores <= 6) { time.Change(0, snakeSpeed -= 40); }               // увеличиваем скорость змеи, если съели до 6 яблок
                        else if (snakeSpeed >= 0) { time.Change(0, snakeSpeed -= 10); }      // немного увеличиваем после 6 яблок
                        level++;
                    }
                }
                else if (diflevel == 1)                                                           // система начисления очков для среднего уровня
                {
                    scores += 2;
                    if (scores % 4 == 0)
                    {
                        if (scores <= 6) { time.Change(0, snakeSpeed -= 30); }               // увеличиваем скорость змеи, если съели до 6 яблок
                        else if (snakeSpeed >= 0) { time.Change(0, snakeSpeed -= 10); }      // немного увеличиваем после 6 яблок
                        level++;
                    }
                }
                else if (diflevel == 2)                                                           // система начисления очков для тяжелого уровня
                {
                    scores += 4;
                    if (scores % 8 == 0)
                    {
                        if (scores <= 12) { time.Change(0, snakeSpeed -= 20); }               // увеличиваем скорость змеи, если съели до 12 яблок
                        else if (snakeSpeed >= 0) { time.Change(0, snakeSpeed -= 10); }       // еще ускоряемся после 12 яблок
                        level++;
                    }
                }
            }
            else
            {
                snake.Move();
                SetCursorPosition(0, 28);
                GameInfo();                                                             // вывод игрового табло с инфой
            }
        }
        static void GameMenu()
        {
            int selectedMenuClass = ConsoleHelper.MultipleChoice(true, "start", "statistics", "read rules", "exit");

            switch (selectedMenuClass)
            {
                case 0: { Clear(); DificultyLevel(); /*StartGame();*/ break; }
                case 1: { GameStatistics(); GameMenu(); break; }
                case 2: { GameRules(); break; }
                case 3: { GameOver(); break; }
            }
        }
        public static void GameRules()
        {
            string text = @"    Snake game - the main task of the game is to eat fruits

            that appear on the field and move away from all obstacles.

            The player maneuvers a growing snake, if the snake crashes 

            into a wall or into itself - the game ends.


                                      Goog game!";
            Clear();
            SetCursorPosition(15, 13);
            WriteLine(text);
            ReadKey();
            GameMenu();
        }
        static void GameOver()
        {
            Clear();
            stopWatch.Stop();

            string writePath = @"C:\Users\maksc\source\repos\SnakeGame\SnakeGame\Records.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine("Player\t\t" + scores + "\t{0:mm\\:ss\\.ff}", stopWatch.Elapsed);
                }
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            SetCursorPosition(37, 18);
            WriteLine("GAME OVER, " + "Noname");
            SetCursorPosition(13, 28);
            WriteLine("Your scores: " + scores);
            SetCursorPosition(43, 28);
            Write("\t\tTime: {0:mm\\:ss\\.ff}", stopWatch.Elapsed);

            WriteLine("\n\n\n\n\t\t\t\t\tPress Enter");
            ReadKey();
        }
        static void GameOverMenu()
        {
            int selectedMenuClass = ConsoleHelper.MultipleChoice(true, "game again", "exit");

            switch (selectedMenuClass)
            {
                case 0: { GameMenu(); break; }
                case 1: { break; }
            }
        }
        static void GameInfo()
        {
            Write("\n\n\n\tPlayer: " + "Noname");
            Write("\t\t\t\t\tLevel: " + level);
            Write("\n\n\n\tScores: " + scores);
            Write("\t\t\t\t\tTime:  {0:mm\\:ss\\.ff}", stopWatch.Elapsed);
        }
        public static void TittlePage()
        {
            ForegroundColor = ConsoleColor.Green;
            string SnakePicture = @"
                           /^\/^\
                         _|__|  O|
                \/     /~     \_/ \
                 \____|__________/  \
                        \_______      \
                                `\     \                 \
                                  |     |                  \
                                 /      /                    \
                                /     /                       \\
                              /      /                         \ \
                             /     /                            \  \
                           /     /             _----_            \   \
                          /     /           _-~      ~-_         |   |
                         (      (        _-~    _--_    ~-_     _/   |
                          \      ~-____-~    _-~    ~-_    ~-_-~    /
                            ~-_           _-~          ~-_       _-~
                               ~--______-~                ~-___-~              
                            ";            
            Write(SnakePicture + "\n");
            ResetColor();         
        }
        public static void DificultyLevel()
        {
            //WriteLine("Dificulty level: ");
            int selectedDificultyClass = ConsoleHelper.MultipleChoice(true, "ease", "normal", "hard");

            switch (selectedDificultyClass)
            {
                case 0: { Clear(); walls = new Walls(x, y, '.'); break; }                           // создание стен легкого уровня
                case 1: { Clear(); walls = new Walls(x, y, '.', 70, 5, 10, 21); break; }            // создание стен среднего уровня
                case 2: { Clear(); walls = new Walls(x, y, '.', 70, 5, 10, 21, 60, 12); break; }    // создание стен сложного уровня
            }
            diflevel = selectedDificultyClass;
        }
        static void StartGame()
        {
            snake = new Snake(x / 2, 3, 2);         // создаем объект змейки в центре поля и в длину 3 элемента

            stopWatch.Start();                          // включаем секундомер

            if (diflevel == 0) foodFactory = new FoodFactory(x, y, 'o');   // инициализация объекта еды координатами и символом
            else if (diflevel == 1) foodFactory = new FoodFactory(x, y, 'o', 70, 5, 10, 21);
            else if (diflevel == 2) foodFactory = new FoodFactory(x, y, 'o', 70, 5, 10, 21, 60, 12);

            foodFactory.CreateFood(diflevel);                   // добавляет еду для змейки

            time = new Timer(Loop, null, 0, snakeSpeed);

            while (true)
            {
                if (KeyAvailable)
                {
                    ConsoleKeyInfo key = ReadKey(true);
                    snake.Rotation(key.Key);
                }
            }
        }
        static void GameStatistics()
        {
            Clear();

            string text = "\n\n\t\t\tТаблица рекордов игры Змейка\n\n\t\t\tPlayerName\tScores\tTime";
            WriteLine(text);

            string path = @"C:\Users\maksc\source\repos\SnakeGame\SnakeGame\Records.txt";
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    WriteLine("\n\t\t\t" + line);
                }
            }
            ReadKey();
        }
    }
    struct Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public char ch { get; set; }

        public static implicit operator Point((int, int, char) value) => new Point { x = value.Item1, y = value.Item2, ch = value.Item3 };        // => лямбда-оператор, определяет анонимные лямба выражения, заменяет return.
                                                                                                                                                  // (int, int, char) - кортеж
                                                                                                                                                  //implicit(неявнове преобразование) - типы объектов int, int, char неявно преобразовуются в тип объекта Point

        public static bool operator ==(Point a, Point b) => a.x == b.x && a.y == b.y;      // сравниваем две точки совпадения координат. Головы с едой, стенками и хвостом
        public static bool operator !=(Point a, Point b) => a.x != b.x && a.y != b.y;      // переопределяем оператор равно и не равно
        public void Draw()              // метод вывода точки
        {
            DrawPoint(ch);
        }
        public void Clear()             // метод стирания точки
        {
            DrawPoint(' ');             // "затирает" хвост при движении
        }
        private void DrawPoint(char _ch)            //метод рисует символы границ
        {
            SetCursorPosition(x, y);        // получает позицию курсора
            Write(_ch);                     // выводит символ
        }
    }
    class Walls                                     // отрисовка стен
    {
        public char ch;
        public List<Point> wall = new List<Point>();

        public Walls(int x, int y, char ch)         // конструктор принимает 3 параметра для отображения всех 4 стен заданным символом
        {
            this.ch = ch;
            DrawHorizontal(x, 0);                   //верхняя граница
            DrawHorizontal(x, y - 10);              //нижняя граница, добавили место для вывода игровой информации
            DrawVertical(0, y);                     //левая граница
            DrawVertical(x, y);                     //правая граница
        }
        public Walls(int x, int y, char ch, int x2, int y2, int x3, int y3) : this(x, y, ch)        // уровень normal
        {
            DrawHorizontalUp(x2, y2);
            DrawHorizontalDown(x2, y2);
            DrawVerticalLeft(x3, y3);
            DrawVerticalRight(x3, y3);
        }
        public Walls(int x, int y, char ch, int x2, int y2, int x3, int y3, int x4, int y4) : this(x, y, ch, x2, y2, x3, y3)        // уровень HARD
        {
            DrawHorizontalCenter(x4, y4);
            DrawVerticalCenter(x4, y4);
        }
        private void DrawHorizontal(int x, int y)       // метод рисует горизонтальные стены
        {            
            for (int i = 0; i < x; i++)
            {
                Point p = (i, y, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawVertical(int x, int y)         // метод рисует вертикальные стены
        {
            for (int i = 0; i < y - 10; i++)
            {
                Point p = (x, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawHorizontalUp(int x2, int y2)       // метод рисует горизонтальные внутренние верхние стены
        {
            for (int i = 10; i < x2; i++)
            {
                if (i > 20 && i < 60) continue;
                Point p = (i, y2, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawHorizontalDown(int x2, int y2)       // метод рисует горизонтальные внутренние нижние стены
        {
            for (int i = 10; i < x2; i++)
            {
                if (i > 20 && i < 60) continue;
                Point p = (i, y2+15, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawVerticalLeft(int x3, int y3)         // метод рисует вертикальные правые внутренние стены
        {
            for (int i = 5; i < y3; i++)
            {
                if (i > 10 && i < 15) continue;
                Point p = (x3, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawVerticalRight(int x3, int y3)         // метод рисует вертикальные левые внутренние стены
        {
            for (int i = 5; i < y3; i++)
            {
                if (i > 10 && i < 15) continue;
                Point p = (x3+60, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawHorizontalCenter(int x4, int y4)       // метод рисует горизонтальные внутренние верхние стены
        {
            for (int i = 20; i < x4; i++)
            {
                Point p = (i, y4, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        private void DrawVerticalCenter(int x4, int y4)         // метод рисует вертикальные правые внутренние стены
        {
            for (int i = 5; i < y4+9; i++)
            {
                Point p = (x4-20, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }
        public bool IsHit(Point p)                  // метод проверяет на совпадение точки со стеной
        {
            foreach (var w in wall)
            {
                if (p == w) { return true; }
            }
            return false;
        }
    }
    enum Direction
    {
        LEFT, RIGHT, UP, DOWN                        // определяем перечисление направлений движения
    }
    class Snake
    {
        private readonly List<Point> snake;                                              // список точек тела змеи

        private Direction direction;                                            // переменная наших перечислений
        private readonly int step = 1;                                                   // переменная величины шага            
        private Point tail;                                                     // переменная для хвоста типа Point 
        private Point head;                                                     // переменная для головы типа Point                        
        bool rotate = true;

        public Snake(int x, int y, int length)                                  // конструктор с прорисовкой змейки на старте
        {
            direction = Direction.RIGHT;
            snake = new List<Point>();
            for (int i = x - length; i < x; i++)
            {
                Point p = (i, y, '.');
                snake.Add(p);
                p.Draw();
            }
        }
        public Point GetHead() => snake.Last();                  // голова принимает последний элемент последовательности
        public void Move()                                       // метод движения      
        {
            ForegroundColor = ConsoleColor.Green;        // раскрас тела змейки   
            head = GetNextPoint();                               // голова принимает значение следующей точки координат
            
            snake.Add(head);                                     // добавление символа головы

            tail = snake.First();                                // хвост принимает первый элемент последовательности
            snake.Remove(tail);                                  // удаление элемента хвоста                     

            tail.Clear();                                        // очистка хвоста  
            head.Draw();                                         // новая отрисовка головы

            rotate = true;
        }
        public bool Eat(Point p)                                // метод проверяет, если змейка съела еду - увеличивает её длину
        {
            head = GetNextPoint();
            if (head == p)
            {
                snake.Add(head);
                head.Draw();
                return true;
            }
            return false;
        }
        public Point GetNextPoint()			// отклик на клавиши направления змеи
        {
            Point p = GetHead();            // объект принимает координаты головы
            switch (direction)
            {
                case Direction.LEFT: p.x -= step; break;
                case Direction.RIGHT: p.x += step; break;
                case Direction.UP: p.y -= step; break;
                case Direction.DOWN: p.y += step; break;
            }
            return p;
        }
        public void Rotation(ConsoleKey key)                                                                // метод поворота
        {
            if (rotate)
            {
                switch (direction)
                {
                    case Direction.LEFT:
                    case Direction.RIGHT:
                        if (key == ConsoleKey.DownArrow) { direction = Direction.DOWN; }                    // указываем, что можем повернуть только в две стороны
                        else if (key == ConsoleKey.UpArrow) { direction = Direction.UP; }
                        break;
                    case Direction.UP:
                    case Direction.DOWN:
                        if (key == ConsoleKey.LeftArrow) { direction = Direction.LEFT; }
                        else if (key == ConsoleKey.RightArrow) { direction = Direction.RIGHT; }
                        break;
                }
                rotate = false;                                                                             // отключаем возможность поворачивать после первого нажатия
            }
        }
        public bool IsHit(Point p)                              // метод проверяет на совпадение точки с хвостом
        {
            for (int i = snake.Count - 2; i > 0; i--)
            {
                if (snake[i] == p) { return true; }
            }
            return false;
        }
    }
    class FoodFactory                                   // генерация еды
    {
        readonly int x, y, x2, y2, x3, y3, x4, y4;
        readonly char ch;
        public Point Food { get; private set; }

        readonly Random random = new Random();

        public FoodFactory(int x, int y, char ch)
        {
            this.x = x;
            this.y = y;
            this.ch = ch;
        }
        public FoodFactory(int x, int y, char ch, int x2, int y2, int x3, int y3) : this(x, y, ch)                                                // нельзя ставить при среднем уровне (правильно??)
        {
            this.x2 = x2;
            this.y2 = y2;
            this.x3 = x3;
            this.y3 = y3;
        }
        public FoodFactory(int x, int y, char ch, int x2, int y2, int x3, int y3, int x4, int y4) : this(x, y, ch, x2, y2, x3, y3)              // нельзя ставить при сложном уровне (правильно??)
        {
            this.x4 = x4;
            this.y4 = y4;
        }

        public void CreateFood(int DifLevel)                                                // рисует еду в рандомных точках
        {
            if (DifLevel == 0) { Food = (random.Next(1, 79), random.Next(1, 25), ch); }
            if (DifLevel == 1)
            {
                Food = (random.Next(1, 79), random.Next(1, 25), ch);
                //for (int i = 10; i < 70; i++)
                //{                    
                //    if (i > 20 && i < 60) { continue; }
                //    else if(Food.x == i && Food.y == 5 || Food.x == i && Food.y == 21) { Food = (random.Next(1, 79), random.Next(1, 25), ch); }
                //}

            }
            if (DifLevel == 2) { Food = (random.Next(1, 79), random.Next(1, 25), ch); }

            ForegroundColor = ConsoleColor.Red;                         // меняем цвет еды
            Food.Draw();
        }
    }
    public class ConsoleHelper
    {
        public static int MultipleChoice(bool canCancel, params string[] options)
        {
            int optionsPerLine = options.Length;

            int currentSelection = 0;

            ConsoleKey keymenu;

            do
            {
                Clear();
                Game.TittlePage();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == currentSelection)
                        ForegroundColor = ConsoleColor.Red;

                    WriteLine("\t\t\t\t\t" + options[i]);
                    ResetColor();
                }

                keymenu = ReadKey(true).Key;

                switch (keymenu)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection > 0)
                                currentSelection--;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection < optionsPerLine - 1)
                                currentSelection++;
                            break;

                        }
                    case ConsoleKey.Escape:
                        {
                            if (canCancel)
                                return -1;
                            break;
                        }
                }
            } while (keymenu != ConsoleKey.Enter);

            return currentSelection;
        }
    }
}