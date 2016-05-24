using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzelessegiBejaras
{
    public class Graph : List<Vertex>
    {
        public Graph(bool _isDirected)
        {
           isDirected = _isDirected;
        }
        public static bool isDirected = false;
        private bool isExpanded = false;
        private Vertex currentlySelectedVertex;
        public event Vertex.GraphEventHandler CurrentlySelectedVertexChanged;

        #region Properties
            public bool IsSimple
            {
                // TODO unfinished, multiple edges
                get
                {
                    bool containsLoop = false, containsMultipleEdges = false;
                    Parallel.Invoke(
                        () =>
                        {
                            foreach (Vertex vertex in this)
                            {
                                foreach (Edge edge in vertex.Edges)
                                {
                                    if (edge.IsLoop)
                                    {
                                        containsLoop = true;
                                        return;
                                    }
                                }
                            }

                        },
                        () => { containsMultipleEdges = false; }

                        );
                    return !(containsLoop || containsMultipleEdges);
                }
            }
            public bool IsDirected
            {
                get { return isDirected; }
                set
                {
                    isDirected = value;
                    foreach (Vertex v in this)
                    { v.IsDirected = value; }
                }
            }
            public bool IsExpanded
            {
                get { return isExpanded; }
                set { isExpanded = value; Vertex.ExpandedStatic = value; foreach (Vertex vertex in this) { vertex.Expanded = value; } }
            }
            public Vertex SelectedVertex
            {
                get
                {
                    /*foreach (Vertex item in this)
                    {
                        if (item.logicalState == Vertex.LogicalState.SelectedForEdgeDrawing)
                        {
                            return item;
                        }
                    }
                    return null;*/
                    return this.currentlySelectedVertex;
                    
                }
                set
                {
                    this.currentlySelectedVertex = value;
                    if (CurrentlySelectedVertexChanged != null)
                    {
                        CurrentlySelectedVertexChanged(new GraphEventArgs() { CurrentVertex = currentlySelectedVertex });
                    }
                }
            }
            
        #endregion

        public void SetLabelVisibilities(bool visibility)
        {
            Edge.labelVisible = visibility;
            foreach (Edge edge in this.Edges())
            {
                edge.LabelVisible = visibility;
            }
            /*Vertex.ExpandedStatic = true;
            foreach ( Vertex vertex in this)
            {
                vertex.Expanded = true;
            }*/

        }
        public void ConvertToUndirected()
        {
                this.IsDirected = false;
                for (int i = 0; i < this.Count; i++)
                {
                    for (int j = 0; j < this[i].InEdges.Count; )
                    {
                        Vertex a = this[i].InEdges[j].Start, b = this[i].InEdges[j].End;
                        
                        this[i].InEdges[j].DeleteEdge();
                        Edge temp = new Edge(false);
                        temp.AddGrid(MainWindow.DrawingGrid);
                        temp.SetStartEnd(a,b);
                      
                    }
                    for (int j = 0; j < this[i].OutEdges.Count; )
                    {
                        Vertex a = this[i].OutEdges[j].Start, b = this[i].OutEdges[j].End;

                        this[i].OutEdges[j].DeleteEdge();
                        Edge temp = new Edge(false);
                        temp.AddGrid(MainWindow.DrawingGrid);
                        temp.SetStartEnd(a, b);
                       
                        /*Edge temp = new Edge(false);
                        temp.AddGrid(MainWindow.DrawingGrid);
                        temp.SetStartEnd(this[i].OutEdges[j].Start, this[i].OutEdges[j].End);
                        this[i].OutEdges[j].DeleteEdge();*/
                    }
                }
            
        }
        public List<Edge> Edges()
        {
            List<Edge> list = new List<Edge>();
            foreach (Vertex vertex in this)
            {
                if(IsDirected)
                {
                    foreach (Edge edge in vertex.OutEdges)
                    {
                        list.Add(edge);
                    }
                }
                else
                {
                    foreach (Edge edge in vertex.Edges)
                    {
                        if(!list.Contains(edge))
                        {
                            list.Add(edge);
                        }
                    }
                }
            }
            return list;
        }
        public void AddVertex(Vertex vertex)
        {
            this.Add(vertex);
            Vertex.SetLabel();
        }
        public void DeleteVertex(Vertex vertex)
        {
            this.Remove(vertex);
            vertex.Delete();

        }
        public void Reset()
        {
            for (int i = 0; i < this.Count; i++)
            {
                /*Vertex temp = this[i];

                for (int j = 0; j < temp.Edges.Count; j++)
                {
                    temp.Edges[j].DeleteEdge();    
                }
                
                this.RemoveAt(i);
                temp = null;*/
                this[i].Delete();

            }
            this.RemoveRange(0, this.Count);
            Vertex.Reset();
            
            
        }
        public void ResetAllVisitedState()
        {
            foreach (Vertex item in this)
            {
                foreach (Edge edge in item.OutEdges)
                {
                    edge.logicalState = Edge.LogicalState.Idle;
                    edge.PrevArrow.Visibility = System.Windows.Visibility.Hidden;
                }
                foreach (Edge edge in item.InEdges)
                {
                    edge.logicalState = Edge.LogicalState.Idle;
                    edge.PrevArrow.Visibility = System.Windows.Visibility.Hidden;
                }
                foreach (Edge edge in item.Edges)
                {
                    edge.logicalState = Edge.LogicalState.Idle;
                    edge.PrevArrow.Visibility = System.Windows.Visibility.Hidden;
                }
                item.logicalState = Vertex.LogicalState.Idle;
            }
            //Vertex.startPoint = null;
            Vertex.EdgeDrawing = Vertex.EdgeDrawingState.idle;
            currentlySelectedVertex = null;
        }
        public int EdgeCount()
        {
            
            int count = 0;
            if(IsDirected)
            {
                foreach (Vertex item in this)
                {
                    count += item.InEdges.Count;
                    count += item.OutEdges.Count;
                }
            }
            else
            {
                foreach (Vertex item in this)
                {
                    count += item.Edges.Count;
                    
                }
            }
            return count/2;
            //Settings.EdgeCountTxtBlock.Text = count.ToString();
        }
        public bool ContainsEdges()
        {
            foreach (Vertex vertex in this)
            {
                if(vertex.Edges.Count >0 || vertex.InEdges.Count>0 || vertex.OutEdges.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
