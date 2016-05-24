using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SzelessegiBejaras.Algorithms
{
    public abstract class GraphAlgorithm
    {
        protected Graph graph;
        protected int pauseTime;
        protected ListBox listBox;
        protected Vertex source;
        protected const int maxInt = 1000000000;
        protected static Action EmptyDelegate = delegate() { };
        protected void wait(UIElement item)
        {
            if(!ProceedOnlyOnButtonPress)
            {
                System.Threading.Thread.Sleep(pauseTime);
                item.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
            else
            {
                while (!Settings.Proceed)
                {
                    if (item.Dispatcher.HasShutdownStarted ||
                        item.Dispatcher.HasShutdownFinished)
                    {
                        break;
                    }

                    item.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new ThreadStart(delegate { }));
                    Thread.Sleep(20);
                }
                Settings.Proceed = false;
            }
            
        }
        protected GraphAlgorithm(Graph graph, Vertex source, ListBox listBox, double pauseTime)
        {
            this.graph = graph;
            this.source = source;
            this.listBox = listBox;
            this.PauseTime = (int)pauseTime;
            graph.CurrentlySelectedVertexChanged += changeSource; //new Vertex.GraphEventHandler((e) => {  });
        }
        protected void changeSource(GraphEventArgs e)
        {
            source = e.CurrentVertex;
        }
        public bool ProceedOnlyOnButtonPress { get; set; }
        public int PauseTime { get { return pauseTime; } set { pauseTime = value; } }
        public abstract void Run();
        protected int length(Vertex u, Vertex v)
        {
            int length = 0;
            if (graph.IsDirected)
            {
                foreach (Edge edge in u.OutEdges)
                {
                    if (edge.Start == u && edge.End == v)
                    {
                        edge.logicalState = Edge.LogicalState.Visited;
                        length = (int)edge.Length;
                        edge.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                        System.Threading.Thread.Sleep(pauseTime);
                    }
                }
                return length;
            }
            else
            {
                foreach (Edge edge in u.Edges)
                {
                    if ((edge.Start == u && edge.End == v) || (edge.End == u && edge.Start == v))
                    {
                        //edge.logicalState = Edge.LogicalState.Visited;
                        length = (int)edge.Length;
                        edge.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                        System.Threading.Thread.Sleep(pauseTime);
                    }
                }
                return length;
            }


        }
        protected Edge FindEdgeToPrev(Vertex vertex, Vertex prev)
        {
            //Edge edge = new Edge();
            foreach (Edge temp in vertex.InEdges)
            {
                if (temp.Start == prev)
                { return temp; }
            }
            return null;
        }
        protected void waitToButton()
        {
            while (!Settings.Proceed)
            {
                if (listBox.Dispatcher.HasShutdownStarted ||
                    listBox.Dispatcher.HasShutdownFinished)
                {
                    break;
                }

                listBox.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }
            Settings.Proceed = false;
        }
    }
}
