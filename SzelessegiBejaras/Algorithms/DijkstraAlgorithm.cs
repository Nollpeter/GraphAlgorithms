using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SzelessegiBejaras.Containers;

namespace SzelessegiBejaras.Algorithms
{
    public class DijkstraAlgorithm : GraphAlgorithm
    {
        public DijkstraAlgorithm(Graph graph, Vertex source, ListBox listBox, double pauseTime)
            : base(graph, source, listBox, pauseTime)
        {
            /*this.graph = graph;
            this.source = source;
            this.listBox = listBox;
            this.PauseTime = (int)pauseTime;*/
            

        }
        private void OnPriorityQueueChanged(object sender, PriorityQueueEventArgs e)
        {
            listBox.Items.Clear();
            //listBox.Items.Add("A feszítőfa hossza: " + getSum().ToString());
            listBox.Items.Add("Az elsőbbségi-sor állapota");

            for (int i = 0; i < e.Vertices.Count; i++)
            {
                listBox.Items.Add("Csúcs: " + e.Vertices[i].ToString());
                if (e.Priorities[i] == maxInt)
                {
                    listBox.Items.Add("Prioritása: \u221E");
                }
                else
                {
                    listBox.Items.Add("Prioritása: " + e.Priorities[i].ToString());
                }

            }
        }
        public override void Run()
        {
            if(!graph.IsDirected)
            {
                MessageBox.Show("A dijkstra algoritmus csak irányított gráfokra működik (itt)");
            }
            else if(source == null)
            {
                MessageBox.Show("Válasszon ki kezdőpontot!");
            }
            else
            dijkstra();
        }
        
        
        private void dijkstra()
        {
           
            PriorityQueue Q = new PriorityQueue();
            Q.QueueChanged += OnPriorityQueueChanged;
            source.Distance = 0;
            //source.prev.Distance = -1;
            graph.IsExpanded = true;
            graph.SetLabelVisibilities(true);
            foreach (Vertex v in graph)
            {
                v.prev = new Vertex();
                if(v != source)
                {
                    v.Distance = maxInt;
                    v.prev.Distance = maxInt;
                }
                Q.Enqueue(v, v.Distance);
            }
            while (!(Q.Count == 0))
            {
                Vertex u = Q.Dequeue();
                u.logicalState = Vertex.LogicalState.Visited;
                System.Threading.Thread.Sleep(pauseTime);
                u.Dispatcher.Invoke(DispatcherPriority.Render,EmptyDelegate);
                foreach(Vertex v in u.Neighbours)
                {
                    int alt = u.Distance + length(u, v);
#region huladék 
                    /*if(v.Distance == -1)
                    {
                        alt = length(u, v) + u.Distance;
                    }
                    else
                    {
                        alt = 
                    }*/
#endregion
                    if (alt < v.Distance)//v.Distance == -1 || alt < v.Distance) //ha bejáratlan van rövidebb az út
                    {
                        v.Distance = alt;

                        if(v.prev.Distance != maxInt)//-1) // ha már van rendes prev-je, nem undefined
                        {
                            Edge temp = FindEdgeToPrev(v, v.prev);
                            temp.PrevArrow.Visibility = Visibility.Hidden;

                            v.prev = u;

                            Edge temp2 = FindEdgeToPrev(v, v.prev);
                            temp2.DrawPrevArrow();
                        }
                        else
                        {
                            v.prev = u;


                            Edge temp = FindEdgeToPrev(v, v.prev);
                            temp.DrawPrevArrow();
                        }
                    }
                    wait(v);
                    /*if(ProceedOnlyOnButtonPress)
                    {
                        waitToButton();
                        
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(pauseTime);
                        v.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                    }*/
                    v.logicalState = Vertex.LogicalState.Visited;
                }
            }

        }
        
        private Vertex ExtractSmallest()
        {
            Vertex min = graph[0];
            
             
            foreach (Vertex v in graph)
            {
                if (v.Distance<min.Distance)
                {
                    min = v;
                }
            }
            return min;
        }
        //private static Action EmptyDelegate = delegate() { };
        #region PseudoCode
        /*
         *  dist[source] ← 0                       // Distance from source to source
     4      prev[source] ← undefined               // Previous node in optimal path initialization
     5
     6      for each vertex v in Graph:  // Initialization
     7          if v ≠ source            // Where v has not yet been removed from Q (unvisited nodes)
     8              dist[v] ← infinity             // Unknown distance function from source to v
     9              prev[v] ← undefined            // Previous node in optimal path from source
    10          end if 
    11          add v to Q                     // All nodes initially in Q (unvisited nodes)
    12      end for
    13      
    14      while Q is not empty:
    15          u ← vertex in Q with min dist[u]  // Source node in first case
    16          remove u from Q 
    17          
    18          for each neighbor v of u:           // where v is still in Q.
    19              alt ← dist[u] + length(u, v)
    20              if alt < dist[v]:               // A shorter path to v has been found
    21                  dist[v] ← alt 
    22                  prev[v] ← u 
    23              end if
    24          end for
    25      end while
26
27          return dist[], prev[]
         */
        #endregion
    }
    
}
