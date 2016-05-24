using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SzelessegiBejaras.Containers;

namespace SzelessegiBejaras.Algorithms
{
    
    
    public class BreadthFirstSearch : GraphAlgorithm
    {
        public BreadthFirstSearch(Graph graph, Vertex source, ListBox listBox, double pauseTime)
            : base(graph, source, listBox, pauseTime)
        {
            /*this.graph = graph;
            this.pauseTime = (int)pauseTime;
            this.listBox = listBox;*/
            

        }
        public override void Run()
        {
            graph.SetLabelVisibilities(false);
            if(graph.IsDirected)
            {
                DirectedGraphTraverseBFS();
            }
            else
            {
                UndirectedGraphTraverseBFS();
            }
            

        }
        
        
        public void UndirectedGraphTraverseBFS()
        {
            ExtendedQueue<Vertex> queue = new ExtendedQueue<Vertex>();
            queue.AddListBox(this.listBox);
            if(Vertex.startPoint==null)
            {
                MessageBox.Show("Válasszon ki egy kezdőpontot");
            }
            else
            {
                if(!queue.Contains(Vertex.startPoint))
                {
                    queue.Enqueue(Vertex.startPoint);
                }
            }
            
            while (queue.Count > 0)
            {
                Vertex tempNode = queue.Dequeue();
                tempNode.logicalState = Vertex.LogicalState.Visited;
                foreach (Edge edge in tempNode.Edges)
                {
                    Vertex otherVertex;
                    if (tempNode == edge.Start) { otherVertex = edge.End; }
                    else { otherVertex = edge.Start; }

                    if(otherVertex.logicalState != Vertex.LogicalState.Visited && !queue.Contains(otherVertex))
                    {
                        queue.Enqueue(otherVertex);
                    }
                    wait(edge);
                    edge.logicalState = Edge.LogicalState.Visited;
                    wait(edge);
                    otherVertex.logicalState = Vertex.LogicalState.Visited;
                    /*
                    if(ProceedOnlyOnButtonPress)
                    {
                        waitToButton();
                        
                        waitToButton();
                        otherVertex.logicalState = Vertex.LogicalState.Visited;
                    }
                    else
                    {
                        edge.logicalState = Edge.LogicalState.Visited;
                        wait(edge);
                        otherVertex.logicalState = Vertex.LogicalState.Visited;
                        wait(edge);
                    }*/
                    
                    
                    
                    
                    
                }
              
            }
            
        }
        public void DirectedGraphTraverseBFS()
        {
            ExtendedQueue<Vertex> queue = new ExtendedQueue<Vertex>();
            queue.AddListBox(this.listBox);
            if (Vertex.startPoint == null)
            {
                MessageBox.Show("Válasszon ki egy kezdőpontot");
            }
            else
            {
                if (!queue.Contains(Vertex.startPoint))
                {
                    queue.Enqueue(Vertex.startPoint);
                }
            }

            while (queue.Count > 0)
            {
                Vertex tempNode = queue.Dequeue();
                tempNode.logicalState = Vertex.LogicalState.Visited;
                foreach (Edge edge in tempNode.OutEdges)
                {
                    Vertex otherVertex;
                    if (tempNode == edge.Start) { otherVertex = edge.End; }
                    else { otherVertex = edge.Start; }

                    if (otherVertex.logicalState != Vertex.LogicalState.Visited && !queue.Contains(otherVertex))
                    {
                        queue.Enqueue(otherVertex);
                    }
                    edge.logicalState = Edge.LogicalState.Visited;
                    wait(edge);
                    otherVertex.logicalState = Vertex.LogicalState.Visited;
                    wait(edge);
                    /*
                    if (ProceedOnlyOnButtonPress)
                    {
                        waitToButton();
                        edge.logicalState = Edge.LogicalState.Visited;
                        waitToButton();
                        otherVertex.logicalState = Vertex.LogicalState.Visited;
                    }
                    else
                    {
                        
                    }*/
                }

            }
            
        }
    }
}
