using System;
using System.Collections.Generic;   //обобщенные или типизированные классы коллекций (IEnumerable <T> name = new IEnumerable <T>() {};)
using System.Linq;                  //пространство имен предоставляет классы и интерфейсы, которые поддерживают запросы                 

namespace SnakeGame
{
    class Game
    {
        static readonly int x = 80;                     // длина игровой области по горизонтали
        static readonly int y = 36;                     // длина игровой области по вертикали
        static void Main(string[] args)
        {
            Console.SetWindowSize(x + 1, y + 1);        // устанавливает размер окна консоли
            Console.SetBufferSize(x + 1, y + 1);        // устанавливает размер буфера (убирает панель прокрутки)
            Console.CursorVisible = false;              // скрываем курсор

            walls = new Walls(x, y, '#');

            Console.ReadKey();
        }
        static Walls walls;                              //объявляем объект внешних стен
    }
    class Point
    {
        public int x { get; set; }                  
        public int y { get; set; }
        public char ch { get; set; }

        public static implicit operator Point((int, int, char) value) //=>        // => лямбда-оператор, определяет анонимные лямба выражения, заменяет return.
                                                                                  // (int, int, char) - кортеж
                                                                                  //implicit(неявнове преобразование) - типы объектов int, int, char неявно преобразовуются в тип объекта Point
        {
            return new Point { x = value.Item1, y = value.Item2, ch = value.Item3 };
        }
        public void Draw()              // метод вывода точки
        {
            DrawPoint(ch);
        }
        public void Clear()             // метод стирания точки
        {
            DrawPoint(' ');
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
    }
}

// This is Additional brach!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! =)
