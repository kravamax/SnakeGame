using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Snake obj = new Snake();
            obj.Show();
            Console.ReadKey();
        }

    }
    class Snake
    {
        public Snake() { }
        public void Show()
        {
            Console.WriteLine("Hello, it's new game");
        }
    }
}
