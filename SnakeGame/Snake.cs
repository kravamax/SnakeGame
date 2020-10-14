using System;
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
        static void Main(string[] args)
        {

            Console.SetWindowSize(x + 1, y + 1);        // устанавливает размер окна консоли
            Console.SetBufferSize(x + 1, y + 1);        // устанавливает размер буфера (убирает панель прокрутки)
            Console.CursorVisible = false;              // скрываем курсор

            walls = new Walls(x, y, '#');
            snake = new Snake(x / 2, y / 3, 3);         // создаем объект змейки в центре поля и в длину 3 элемента

            foodFactory = new FoodFactory(x, y, '+');   // инициализация объекта еды координатами и символом
            foodFactory.CreateFood();                   // добавляет еду для змейки


            time = new Timer(Loop, null, 0, 200);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    snake.Rotation(key.Key);
                }
            }
        }
        static void Loop(object obj)
        {
            if (walls.IsHit(snake.GetHead()) || snake.IsHit(snake.GetHead()))
            {
                time.Change(0, Timeout.Infinite);
            }
            else if (snake.Eat(foodFactory.food))
            {
                foodFactory.CreateFood();
            }
            else { snake.Move(); }
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
            Console.SetCursorPosition(x, y);        // получает позицию курсора
            Console.Write(_ch);                     // выводит символ
        }
    }
    class Walls                                     // отрисовка стен
    {
        private char ch;
        private List<Point> wall = new List<Point>();

        public Walls(int x, int y, char ch)         // конструктор принимает 3 параметра для отображения всех 4 стен заданным символом
        {
            this.ch = ch;
            DrawHorizontal(x, 0);                   //верхняя граница
            DrawHorizontal(x, y - 10);              //нижняя граница, добавили место для вывода игровой информации
            DrawVertical(0, y);                     //левая граница
            DrawVertical(x, y);                     //правая граница
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
        private List<Point> snake;                                              // список точек тела змеи

        private Direction direction;                                            // переменная наших перечислений
        private int step = 1;                                                   // переменная величины шага            
        private Point tail;                                                     // переменная для хвоста типа Point 
        private Point head;                                                     // переменная для головы типа Point                        
        bool rotate = true;
        public Snake(int x, int y, int length)                                  // конструктор с прорисовкой змейки на старте
        {
            direction = Direction.RIGHT;
            snake = new List<Point>();
            for (int i = x - length; i < x; i++)
            {
                Point p = (i, y, '*');
                snake.Add(p);
                p.Draw();
            }
        }
        public Point GetHead() => snake.Last();
        public void Move()
        {
            head = GetNextPoint();
            snake.Add(head);

            tail = snake.First();
            snake.Remove(tail);

            tail.Clear();
            head.Draw();

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
        public Point GetNextPoint()
        {
            Point p = GetHead();
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
        int x, y;
        char ch;
        public Point food { get; private set; }

        Random random = new Random();

        public FoodFactory(int x, int y, char ch)       // Использовать лямбада-функции?
        {
            this.x = x;
            this.y = y;
            this.ch = ch;
        }

        public void CreateFood()                                                // рисует еду в рандомных точках
        {
            food = (random.Next(2, x - 2), random.Next(2, y - 2), ch);
            food.Draw();
        }
    }
}
