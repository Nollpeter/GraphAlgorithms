using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SzelessegiBejaras.Containers;

namespace SzelessegiBejaras.Algorithms
{
    class PrimAlgorithm : GraphAlgorithm
    {
        PriorityQueue Q;
        public PrimAlgorithm(Graph graph,Vertex source,ListBox listBox,double pauseTime):base(graph,source,listBox,pauseTime)
        {
            Q = new PriorityQueue();
            Q.QueueChanged += OnPriorityQueueChanged;
           
        }
        public override void Run()
        {
            if (graph.IsDirected)
            {
                MessageBox.Show("A prim algoritmus csak irányítatlan gráfokra működik (itt)");
            }
            else if (source == null)
            {
                MessageBox.Show("Válasszon ki kezdőpontot!");
            }
            else
            {
                graph.IsExpanded = false;
                graph.SetLabelVisibilities(true);
                prim();
            }
                
        }
        private void OnPriorityQueueChanged(object sender, PriorityQueueEventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("A feszítőfa hossza: " + getSum().ToString());
            listBox.Items.Add("Az elsőbbségi-sor állapota");

            for (int i = 0; i < e.Vertices.Count; i++)
            {
                listBox.Items.Add("Csúcs: " + e.Vertices[i].ToString());
                if(e.Priorities[i] == maxInt)
                {
                    listBox.Items.Add("Prioritása: \u221E");
                }
                else
                {
                    listBox.Items.Add("Prioritása: " + e.Priorities[i].ToString());
                }
                
            }
        }
        private int getSum()
        {
            int sum = 0;
            foreach (Vertex v in graph)
            {
                if(v.Distance != maxInt)
                sum += v.Distance;
            }
            return sum;
        }
        private void prim()
        {
            foreach (Vertex item in graph)
            {
                item.Distance = maxInt;
                item.prev = null;
            }
            source.Distance = 0;
            foreach(Vertex item in graph)
            {
                Q.Enqueue(item, item.Distance);
            }
            while (Q.Count != 0)
            {
                Vertex u = Q.Dequeue();
                foreach (Vertex v in u.Neighbours)
                {
                    if(Q.Contains(v) && length(u,v)<v.Distance)
                    {
                        Edge tempEdge = FindEdgeToPrev(v, v.prev);
                        if(tempEdge!=null)
                        {
                            tempEdge.logicalState = Edge.LogicalState.Idle;
                            wait(tempEdge);
                        }
                        
                        v.prev = u;
                        v.Distance = length(u, v);
                        Q.Repair();
                        v.logicalState = Vertex.LogicalState.Visited;
                        wait(v);
                        Edge tempEdge2 = FindEdgeToPrev(v, v.prev);
                        tempEdge2.logicalState = Edge.LogicalState.Visited;
                        wait(tempEdge2);
                    }
                }
            }
            /*foreach (Vertex v in graph)
            {
                if(v != source)
                {
                    
                    
                }
            }*/
            /*
             * 
             *  1.  Make a queue (Q) with all the vertices of G (V);
                2.  For each member of Q set the priority to INFINITY;
                3.  Only for the starting vertex (s) set the priority to 0;
                4.  The parent of (s) should be NULL;
                5.  While Q isn’t empty
                6.     Get the minimum from Q – let’s say (u); (priority queue);
                7.     For each adjacent vertex to (v) to (u)
                8.        If (v) is in Q and weight of (u, v) < priority of (v) then
                9.           The parent of (v) is set to be (u)
                10.          The priority of (v) is the weight of (u, v)*/
        }
        new protected Edge FindEdgeToPrev(Vertex vertex, Vertex prev)
        {
            //Edge edge = new Edge();
            foreach (Edge temp in vertex.Edges)
            {
                if (temp.Start == prev || temp.End == prev)
                { return temp; }
            }
            return null;
        }
        
    }
}
