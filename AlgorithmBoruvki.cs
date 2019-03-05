using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Graph
{
    /* 
     * 30 Вариант
     * Постановка задачи: задан неориентированный взвешенный граф. Найдите остовное дерево минимальной стоимости
     * посредством влгоритма Борувки.
     * 
     * Описание алгоритма:
     * На вход подается связный взвешенный неориентированный граф.
     * Шаг 1: Создается список списков, представляющий собой лес деревьев (сегменты). В него помещаются все вершины
     * как отдельные сегменты.
     * Шаг 2: Для каждого сегмента находится минимальное по стоимости ребро, соединяющее его с другим сегментом.
     * Ребро подсвечивается зеленым. Эти сегменты объединяются.
     * Повторять Шаг 2 до тех пор, пока количество ребер в дереве не будет равно NV - 1, где NV - количество вершин в графе.
     * Когда количество ребер будет равно NV - 1, это означает, что остовное дерево найдено.
     * Выводим сообщение о том, что остовное дерево найдено.
     */
    class AlgorithmBoruvki
    {
        public async Task Start(CGraph graph, Grid grid)
        {
            List<Vertex> lv = graph.Vertices;
            List<Edge> le = graph.Edges;
            List<Edge> tredges = new List<Edge>(); //Список с ребрами остовного дерева
            List<List<Vertex>> segments = new List<List<Vertex>>(); //Список сегментов

            foreach (Edge e in le)
                e.Line.Stroke = Brushes.Black;
            foreach (Vertex v in lv)
                v.Ellipse.Stroke = Brushes.Black;

            foreach (Vertex v in lv) //Шаг 1
            {
                List<Vertex> segment = new List<Vertex>();
                segment.Add(v);
                segments.Add(segment);
            }

            while (tredges.Count != lv.Count - 1)//Шаг 2
            {
                List<Edge> edgelist = new List<Edge>();//Список выбранных ребер минимальной стоимости

                foreach (List<Vertex> segment in segments)
                {
                    List<Edge> edges = new List<Edge>();//Список ребер, инцидентных сегменту

                    foreach (Vertex v1 in segment)//Заполнение списка edges
                        foreach (Edge e in le)
                            if ((e.End == v1 && !segment.Contains(e.Start)) || (e.Start == v1 && !segment.Contains(e.End)))
                                if (!edges.Contains(e))
                                    edges.Add(e);

                    List<Line> ls = new List<Line>();//Список линий, представляющих собой ребра в графе
                    foreach (Edge e in edges)
                        ls.Add(e.Line);

                    await Blink(ls.ToArray());//Моргание линий 

                    int min = int.MaxValue;
                    Edge emin = null;
                    foreach (Edge e in edges)//Поиск минимального по стоимости ребра
                        if (e.Weight < min)
                        {
                            emin = e;
                            min = e.Weight;
                        }

                    emin.Line.Stroke = Brushes.Green;//Подсветка ребра в зеленый

                    if (!tredges.Contains(emin))//Добавление ребра в список ребер остовного дерева
                        tredges.Add(emin);

                    if (!edgelist.Contains(emin))//Добавление ребра в список минимальных ребер
                        edgelist.Add(emin);

                }

                foreach (Edge e in edgelist)//Объединение сегментов по выбранным ребрам минимальной стоимости 
                {
                    List<Vertex> segment1 = null;
                    List<Vertex> segment2 = null;

                    foreach (List<Vertex> s in segments)
                        if (s.Contains(e.Start))
                            segment1 = s;
                        else if (s.Contains(e.End))
                            segment2 = s;

                    if (segment2 == null || segment1 == null)
                        throw new Exception("One of segments is references null");

                    Merge(segments, segment1, segment2);
                }

                await ShowSegments(segments, le, tredges);
            }

            MessageBox.Show("Остовное дерево найдено!");
        }

        private void Merge(List<List<Vertex>> segments, List<Vertex> segment1, List<Vertex> segment2) //Объединение двух сегментов в один
        {
            List<Vertex> segment = new List<Vertex>();

            foreach (Vertex v in segment1)
                segment.Add(v);

            foreach (Vertex v in segment2)
                segment.Add(v);

            segments.Remove(segment1);
            segments.Remove(segment2);
            segments.Add(segment);
        }

        private async Task ShowSegments(List<List<Vertex>> segments, List<Edge> edges, List<Edge> tredges) //Выделение отдельных сегментов для наглядности в GUI
        {
            List<Brush> brushes = new List<Brush>();

            foreach (Edge e in edges)
                brushes.Add(e.Line.Stroke);


            foreach (List<Vertex> segment in segments)
            {
                foreach (Edge e in edges)
                    e.Line.Stroke = Brushes.LightGray;

                foreach (Vertex v1 in segment)
                    foreach (Vertex v2 in segment)
                        if (v1 != v2)
                            foreach (Edge e in tredges)
                                if ((e.Start == v1 && e.End == v2) || (e.Start == v2 && e.End == v1))
                                    e.Line.Stroke = Brushes.Blue;

                foreach (Vertex v in segment)
                    v.Ellipse.Stroke = Brushes.Blue;

                foreach (List<Vertex> other in segments)
                    if (other != segment)
                        foreach (Vertex v in other)
                            v.Ellipse.Stroke = Brushes.LightGray;

                await Task.Delay(2000);
            }

            if (segments.Count != 1)
            {
                for (int i = 0; i < edges.Count; i++)
                    edges[i].Line.Stroke = brushes[i];

                foreach (List<Vertex> segment in segments)
                    foreach (Vertex v in segment)
                        v.Ellipse.Stroke = Brushes.Black;
            }
        }

        private async Task Blink(params Line[] ls) //Моргание линий
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (Line l in ls)
                {
                    if (l.Stroke == Brushes.Black)
                        l.Stroke = Brushes.Red;
                    else if (l.Stroke == Brushes.Red)
                        l.Stroke = Brushes.Black;

                    if (l.Stroke == Brushes.Green)
                        l.Stroke = Brushes.Blue;
                    else if (l.Stroke == Brushes.Blue)
                        l.Stroke = Brushes.Green;
                }
                await Task.Delay(200);
            }
        }

    }
}
