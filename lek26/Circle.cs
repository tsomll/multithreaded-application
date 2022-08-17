using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lek26
{
    public class Circle
    {
        public Color Color { get; set; }
        public float Radius { get; set; }
        public float X{ get; set; }
        public float Y{ get; set;}
        public float direction = 1;
        public SizeF ContainerSize { get; set; }
        private Thread? t;

        private static bool stop = false;
        //поле и свойство для присотановки работы потока шариков
        private static bool pause = false;
        public static bool IsPaused => pause;

        //чтобы шарики на паузе не удалились из массива "живых"
        //добавляем в свойство IsAlive значение паузы
        public bool IsAlive => pause || (t?.IsAlive ?? false);
        private Random _r = new Random();
        private float _dx = 1;
        
        public Circle(SizeF containerSize)//конструктор для случайного цвета и радиуса
        {
            Color = Color.FromArgb(
                _r.Next(235),
                _r.Next(235),
                _r.Next(235)
                );
            Radius = _r.Next(30, 100);
            ContainerSize = containerSize;
        }
        public void Paint(Graphics g)//метод для рисовки шарика
        {
            var b = new SolidBrush(Color);
            g.FillEllipse(
                b,
                X, 
                Y,
                Radius,
                Radius
                );
        }
        private bool Move()//меняем координату на какую то величину чтобы шарик двигался
        {
           
            while (!stop)
            {
                if (X < ContainerSize.Width - Radius + _dx/2 && X>ContainerSize.Width - Radius - _dx/2)
                    direction = 2;
                if(X>- _dx/2&&X< _dx/2)
                    direction = 1;
                while (direction==1)
                {
                    X += _dx;
                    return true;
                }
                while (direction == 2)
                {
                    X -= _dx;
                    return true;
                }
                //if (X < ContainerSize.Width - Radius - _dx)//чтобы шарик не выходил за пределы панели
                //{
                //    while (X < ContainerSize.Width - Radius - _dx)
                //    {
                //        X += _dx;
                //        direction = 1;
                //        return true;
                //    }
                //}
                //else
                //{
                //    direction = 2;
                //    while ((X > Radius + _dx))
                //    {
                //        X -= _dx;
                //        direction = 2;
                //        return true;
                //    }
                //}
            }
            if(stop)
                return false;
            return false;
        }
        public void Start()//метод для заупска шарика
        {
            if (t == null || !t.IsAlive)//проверка что поток дохлый(поток жив пока выполняется метод за который он отвечает
            {
                t = new Thread(() =>//функция прям на месте вызова/называется лямбда выражения/нет возвращ-го знач-я
                {
                    while (!stop && Move() && !IsPaused)//подождать чтобы смещение плавно происходило
                    {
                        Thread.Sleep(30);
                    }
                });
                t.IsBackground = true;//тогда поток будет фоновым, он прервется при завершении всех основных потоков
                t.Start();
            }
        }

        public static void StopAllCircles()
        {
            stop = true;
        }

        //метод присотанавливающий все шарики
        public static void Pause()
        {
            pause = true;
;        }

        //сетод возобновляющих движение всех шариков
        public static void Resume()
        {
            pause = false;
        }

    }
}
