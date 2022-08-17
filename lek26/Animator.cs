using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lek26
{
    public class Animator
    {
        private object commonGraphics = new();
        private object commonCircles = new();
        private Graphics _mainG;
        private List<Circle> _circ = new ();//список шариков/храним информацию обо всех шарах
        private BufferedGraphics buf_gr;
        private Thread? t;//для защиты от дурака
        private  bool stop = false;
        private SizeF _containerSize;
        private SizeF ContainerSize
        {
            get => _containerSize;
            set
            {
                _containerSize = value;
                _circ.ForEach(c => { c.ContainerSize = value; });
            }

        }

        public Graphics MainGraphics
        {
            set
            {
                lock(commonGraphics)
                {
                    _mainG = value;
                    ContainerSize = _mainG.VisibleClipBounds.Size;
                    buf_gr = BufferedGraphicsManager.Current.Allocate(_mainG, Rectangle.Ceiling(_mainG.VisibleClipBounds));

                }

            }
        }
        public Animator (Graphics g)
        {
           MainGraphics = g;
           
        }

       //постановка шариков на паузу и снятие паузы
        public void ChangePausedState()
        {
            if(Circle.IsPaused)
            {
                lock(commonCircles)
                {
                    Circle.Resume();
                    _circ.ForEach(c => { c.Start(); });
                }
            }
            else Circle.Pause();
        }
        
        private void _Start()
        {
            /* _circ = new(_mainG.VisibleClipBounds.Size);
             _circ.X = 0;//чтобы шарик ехал по новой
             Monitor.Enter(buf_gr);
             buf_gr.Graphics.Clear(Color.White);//теперь с монитор обращение синхронизировано
             Monitor.Exit(buf_gr);
             while (true)
             {
                 Monitor.Enter(buf_gr);//заходим в синхронизированный блок
                 _circ.Paint(buf_gr.Graphics);
                 buf_gr.Render(_mainG);//мэйн тоже является общим объектом
                 Monitor.Exit(buf_gr);
                 Thread.Sleep(30);//кол-во миллисекунд на которые поток приостановится/
                 lock(buf_gr)
                 {
                     buf_gr.Graphics.Clear(Color.White);
                 }
                 buf_gr.Graphics.Clear(Color.White);//общий объект в котором работает сразу мног потоков
                 if (!_circ.Move()) break;
             }
            // buf_gr.Dispose();//очистим*/
            while (!stop)//в этом цикле формируем кадры для добавления шариков
            {
                lock(commonCircles)
                {
                    _circ = _circ.FindAll(c=>c.IsAlive);
                }    
                //метод находит все элементы которые удовл-ют условиям поиска/такой элемент где живой истина/будет возвращать новую коллекцию элементов/сдохшие шарики будут выкинуты из списка
                var c = new List<Circle>(_circ);
                lock(commonGraphics)
                {
                    buf_gr.Graphics.Clear(Color.White);//сначала очищаем пространство
                    foreach (var circle in c)//прохожусь по списку шаров и у каждого беру координату
                    {//опасно в использовании цикла форич 
                        circle.Paint(buf_gr.Graphics);//отрисовываю

                    }
                    buf_gr.Render(_mainG);//отрисовываем все что получилось
                }
                Thread.Sleep(30);//чтобы анимация выглядела плавно и частота 30 кадров в сек хотя б/появляется каждые 30 миллисекунд

            }
        }

        public void Start()//делаем анимацию
        {
            if (t != null && t.IsAlive)//если вы
            {
                Stop();
                t.Join();//будет простановливать работу активного потока пока не будет завершен поток т
            }
            stop = false;
            t = new Thread(_Start);
            t.Start();//запускаю отдельно в подпроцессе,чтобы я запустила енсколько процессов одновременно
            //и параллельно могла взаимодействовать с окном помимо шарика
        }

        public void Stop()
        {
            stop = true;
           //Circle.StopAllCircles();
        }
        public void AddCircle()//метод для добавления шариков
        {
            //SizeF sz;
            //lock(_mainG)
            //{
            //    sz = _mainG.VisibleClipBounds.Size;
            //}
            var c = new Circle(ContainerSize);
            c.Start();
            _circ.Add(c);
        }
    }

}
