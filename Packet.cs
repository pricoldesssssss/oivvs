using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab1
{
    public class Packet
    {
        public int Number { get; set; }              // номер пакета
        public int From { get; set; }                // откуда
        public int To { get; set; }                  // куда
        public int CurrentVertex { get; set; }       // где сейчас (индекс вершины)
        public int NextVertex { get; set; }          // куда движется
        public int Size { get; set; }                // размер (байт)
        public int TimeToLive { get; set; }          // время жизни (в тактах)
        public int TicksAlive { get; set; }          // сколько уже живёт
        public List<int> Route { get; set; }         // назначенный маршрут
        public int RoutePosition { get; set; }       // позиция в маршруте
        public PointF Position { get; set; }         // текущая позиция на экране
        public PointF StartPos { get; set; }         // откуда движется (для интерполяции)
        public PointF TargetPos { get; set; }        // куда движется
        public float Progress { get; set; }          // прогресс движения 0..1
        public bool Delivered { get; set; }          // доставлен?
        public bool IsAlive { get; set; }            // жив?
        public Color Color { get; set; }             // цвет пакета

        public Packet(int number, int from, int to)
        {
            Number = number;
            From = from;
            To = to;
            CurrentVertex = from;
            NextVertex = -1;
            Size = 1024;         // по умолчанию 1 КБ
            TimeToLive = 20;     // 20 тактов
            TicksAlive = 0;
            Route = new List<int>();
            RoutePosition = 0;
            Delivered = false;
            IsAlive = true;
            Progress = 0;
            Color = Color.Orange;
        }

        public Packet Copy()
        {
            return new Packet(Number, From, To)
            {
                CurrentVertex = CurrentVertex,
                NextVertex = NextVertex,
                Size = Size,
                TimeToLive = TimeToLive,
                TicksAlive = TicksAlive,
                Route = new List<int>(Route),
                RoutePosition = RoutePosition,
                Position = Position,
                StartPos = StartPos,
                TargetPos = TargetPos,
                Progress = Progress,
                Delivered = Delivered,
                IsAlive = IsAlive,
                Color = Color
            };
        }

        public string GetRouteString()
        {
            if (Route == null || Route.Count == 0)
                return "не назначен";

            var parts = new List<string>();
            for (int i = 0; i <= RoutePosition && i < Route.Count; i++)
                parts.Add($"v{Route[i]}");

            return string.Join(" → ", parts);
        }
    }
}