using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SzelessegiBejaras
{
	/// <summary>
	/// Interaction logic for Vertex.xaml
	/// </summary>
    public class GraphEventArgs : EventArgs
    {
        public Vertex CurrentVertex;
        public Edge CurrentEdge;
        public Vertex.LogicalState VertexLogicalState;
        public Edge.LogicalState EdgeLogicalState;
        public Vertex.EdgeDrawingState EdgeDrawingState;
        public bool Handled { get; set; }
    }
	public partial class Vertex : Button
	{
		public Vertex()
		{
			this.InitializeComponent();
            Number = count;
            this.Width = this.Height = 30;
            isDirected = MainWindow.myGraph.IsDirected;
            inEdges = new List<Edge>();
            outEdges = new List<Edge>();
            edges = new List<Edge>();
            //SetDistanceLabel(-1);
            Distance = maxInt;
            //prev = new Vertex();
            VertexMoved += OnVertexMoved;
            LogicalStateChanged += OnLogicalStateChanged;
            this.Expanded = ExpandedStatic;
            if(DrawingStateChanged==null)
            DrawingStateChanged += OnDrawingStateChanged;
        }
        
        #region Data
            const int maxInt = 1000000000;
            public enum InOrOut {IN,OUT};
            public enum EdgeDrawingState {idle,initiated,finished}
            public static EdgeDrawingState edgeDrawing = EdgeDrawingState.idle;
            public enum LogicalState { Idle,SelectedForEdgeDrawing,Visited,NotYetVisited,TravelStartingPoint}
            public static bool ExpandedStatic;
            private bool expanded = false;
            private LogicalState _logicalState;
            private int distance;
            public enum AlgorithmState { visited, unvisited }
            protected static int count = 1;
            public static Vertex startPoint = new Vertex();
            int number;
            int edgeCount = 0;
            int inDegree = 0;
            int outDegree = 0;
            bool isDirected = false;
            public Vertex prev;
            Grid parent = new Grid();
            List<Edge> inEdges;
            List<Edge> outEdges;
            List<Edge> edges;
            public Point LeftTop, RightTop, LeftBottom, RightBottom;
            public delegate void VertexEventHandler();
            public delegate void GraphEventHandler(GraphEventArgs e);
            public event VertexEventHandler VertexMoved;
            public event VertexEventHandler EdgeAdded;
            public event GraphEventHandler CurrentlySelectedVertexChanged;
            public event GraphEventHandler LogicalStateChanged;
            public static event GraphEventHandler DrawingStateChanged;
            public event GraphEventHandler DistanceChanged;
        #endregion

        #region Properties
            public int Number
            {
                get { return number; }
                set { number = count; this.Label.Text = number.ToString(); }
            }
            public int EdgeCount
            {
                get { return edgeCount; }
            }
            public int OutDegree
            {
                get { return outDegree; }
            }
            public int InDegree
            {
                get { return inDegree; }
            }
            public int Distance
            {
                get { return distance; }
                set { distance = value; SetDistanceLabel(distance); if (DistanceChanged != null) DistanceChanged(new GraphEventArgs() { CurrentVertex = this}); }
            }
            public bool Expanded
            {
                get { return this.expanded; }
                set { this.expanded = value; OnResized(); }
            }
            public bool IsDirected
            {
                get { return isDirected; }
                set { isDirected = value; }
            }
            public static EdgeDrawingState EdgeDrawing
            {
                get { return edgeDrawing; }
                set
                {
                    //edgeDrawing = value;
                    if(DrawingStateChanged != null)
                    DrawingStateChanged(new GraphEventArgs() { EdgeDrawingState = value, CurrentVertex = startPoint, Handled = false});
                }
            }
            public List<Edge> InEdges { get {return inEdges;} }
            public List<Edge> OutEdges { get {return outEdges;} }
            public List<Edge> Edges { get {return edges;} }
            //TODO irányítottra
            public HashSet<Vertex> Neighbours
            {
                get 
                {
                    HashSet<Vertex> list = new HashSet<Vertex>();
                    if(!isDirected)
                    {
                        foreach (Edge edge in this.Edges)
                        {
                            list.Add(edge.Start);
                            list.Add(edge.End);
                        }
                        list.Remove(this);
                    }
                    else
                    {
                        foreach(Edge edge in this.OutEdges)
                        {
                            list.Add(edge.End);
                        }
                    }
                    return list;
                }
            }
            public LogicalState logicalState
            {
                get { return this._logicalState; }
                set { _logicalState = value; LogicalStateChanged(new GraphEventArgs() {VertexLogicalState = value, CurrentVertex = this }); }
            }
            new public Thickness Margin
            {
                get { return base.Margin; }
                set { base.Margin = value; VertexMoved(); }
            }
        #endregion

        #region Methods
            public override string ToString()
            {
                return this.Number.ToString();
            }
            public static void Reset()
            {
                Vertex.count = 1;
                Vertex.startPoint = null;
                Vertex.edgeDrawing = Vertex.EdgeDrawingState.idle;
            }
            public static void SetLabel()
            {
                count++;
            }
            public void AddParent (Grid par)
            {
                parent = par;
            }
            /// <summary>
            /// Adds an edge to the Vertex in a non-directed graph. The edge therefore will be added to both InEdges and OutEdges lists
            /// </summary>
            /// <param name="edge"></param>
            public void AddEdge(Edge edge)
            {
                edges.Add(edge);
                ++edgeCount;
                Edge.count++;
                if (EdgeAdded != null)
                    EdgeAdded();
                
            }
            
            public void AddEdge(Edge edge , InOrOut direction)
            {
                if (direction == InOrOut.IN)
                {
                    inEdges.Add(edge);
                    ++inDegree;
                    ++edgeCount;
                    Edge.count++;
                }
                else
                {
                    outEdges.Add(edge);
                    ++outDegree;
                    ++edgeCount;
                    Edge.count++;
                }
                if (EdgeAdded != null)
                    EdgeAdded();
            }
            public void SetDistanceLabel(int value)
            {
                if (value == maxInt) { DistanceLabel.Text = "\u221E"; }
                else { DistanceLabel.Text = value.ToString(); }
            }
            private void OnDrawingStateChanged(GraphEventArgs e)
            {
                if(!e.Handled)
                {
                    if (e.EdgeDrawingState == EdgeDrawingState.idle && Vertex.startPoint != null)
                    {
                        //this.EdgeAdded();
                        if (e.CurrentVertex.CurrentlySelectedVertexChanged != null)
                            e.CurrentVertex.CurrentlySelectedVertexChanged(new GraphEventArgs() { CurrentVertex = null});
                        edgeDrawing = EdgeDrawingState.idle;
                        //Vertex.startPoint.logicalState = LogicalState.Idle;
                        //Vertex.startPoint = null;
                        e.CurrentVertex.logicalState = LogicalState.Idle;
                        e.CurrentVertex = null;
                        logicalState = LogicalState.Idle;
                        
                        e.Handled = true;

                    }
                }
                
            }
            private void VertexButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
            {
                if(edgeDrawing == EdgeDrawingState.idle)
                {
                    startPoint = this;
                    edgeDrawing = EdgeDrawingState.initiated;
                    logicalState = Vertex.LogicalState.SelectedForEdgeDrawing;
                   
                }
                else //if( edgeDrawing == EdgeDrawingState.initiated)
                {
                    Edge edge = new Edge(MainWindow.myGraph.IsDirected);
                    edge.AddGrid(MainWindow.DrawingGrid);
                    //Points p = GetDirection(startPoint, this);
                    edge.SetStartEnd(startPoint, this);
                    
                    edgeDrawing = EdgeDrawingState.idle;
                    startPoint.logicalState = Vertex.LogicalState.Idle;
                    startPoint = null;
                }
                CurrentlySelectedVertexChanged(new GraphEventArgs() { CurrentVertex = Vertex.startPoint });
            }
            public void OnLogicalStateChanged(GraphEventArgs e)
            {
                switch(e.VertexLogicalState )
                {
                    case LogicalState.Idle: this.Background = Brushes.LightGray; break;
                    case LogicalState.NotYetVisited: this.Background = Brushes.LightGray; break;
                    case LogicalState.SelectedForEdgeDrawing: this.Background = Brushes.Red; break;
                    case LogicalState.TravelStartingPoint: this.Background = Brushes.Blue; break;
                    case LogicalState.Visited: this.Background = Brushes.LightGreen; break;
                }
            }
            public void OnResized()
            {
                //fordítva kell csinálni, mert először beállítom, aztán jön aművelet
                if (!this.Expanded) // amikor összecsukom
                {
                    this.Label.Margin = new Thickness();
                    this.DistanceLabel.Visibility = System.Windows.Visibility.Hidden;
                    this.Height = 30;
                    SetLeftRightTopBottom();
                    refreshEdgePositions();

                }
                else//szétnyitás
                {
                    this.Label.Margin = new Thickness(-10, -24, -10, 0);
                    this.DistanceLabel.Margin = new Thickness(-10, 0, -10, -22);
                    this.DistanceLabel.Visibility = System.Windows.Visibility.Visible;
                    this.Height = 60;
                    SetLeftRightTopBottom();
                    refreshEdgePositions();
                }
            }
            private void refreshEdgePositions()
            {
                foreach (Edge edge in this.Edges)
                {
                    Points p = GetDirection(edge.Start, edge.End);
                    edge.SetCoords(p.a, p.b);
                }
                foreach (Edge edge in this.InEdges)
                {
                    Points p = GetDirection(edge.Start, edge.End);
                    edge.SetCoords(p.a, p.b);
                }
                foreach (Edge edge in this.OutEdges)
                {
                    Points p = GetDirection(edge.Start, edge.End);
                    edge.SetCoords(p.a, p.b);
                }
            }
            public struct Points
            {
                public Point a;
                public Point b;
            }
            public Points GetDirection (Vertex a, Vertex b)
            {
                Points p = new Points();
                if (a.Margin.Left < b.Margin.Left && a.Margin.Top > b.Margin.Top)
                {
                    p.a = a.RightTop;
                    p.b = b.LeftBottom;
                }
                else if (a.Margin.Left < b.Margin.Left && a.Margin.Top < b.Margin.Top)
                {
                    p.a = a.RightBottom;
                    p.b = b.LeftTop;
                }
                else if (a.Margin.Left > b.Margin.Left && a.Margin.Top < b.Margin.Top)
                {
                    p.a = a.LeftBottom;
                    p.b = b.RightTop;
                }
                else if(a.Margin.Left > b.Margin.Left && a.Margin.Top > b.Margin.Top)
                {
                    p.a = a.LeftTop;
                    p.b = b.RightBottom;
                }
                else if(a.Margin.Left < b.Margin.Left && a.Margin.Top == b.Margin.Top)
                {
                    p.a = a.RightTop; 
                    p.b = b.LeftTop;
                }
                else if (a.Margin.Left > b.Margin.Left && a.Margin.Top == b.Margin.Top)
                {
                    p.a = a.LeftTop;
                    p.b = b.RightTop;
                }
                else if(a.Margin.Left == b.Margin.Left && a.Margin.Top < b.Margin.Top)
                {
                    p.a = a.LeftBottom;
                    p.b = b.LeftTop;
                }
                else if (a.Margin.Left == b.Margin.Left && a.Margin.Top > b.Margin.Top)
                {
                    p.a = a.LeftTop;
                    p.b = b.LeftBottom;
                }
                else if (a.Margin.Left == b.Margin.Left && a.Margin.Top == b.Margin.Top)
                {
                    p.a = a.LeftTop;
                    p.b = b.RightTop;
                }
                return p;
            }
            public void SetLeftRightTopBottom()
            {
                LeftTop.X = Margin.Left;
                LeftTop.Y = Margin.Top;
                RightTop.X = Margin.Left + Width;
                RightTop.Y = Margin.Top;
                LeftBottom.X = Margin.Left;
                LeftBottom.Y = Margin.Top + Height;
                RightBottom.X = Margin.Left + Width;
                RightBottom.Y = Margin.Top + Height;
            }
            public void OnVertexMoved()
            {
                SetLeftRightTopBottom();
                if (VertexMoved != null)
                {
                    if (MainWindow.myGraph.IsDirected)
                    {
                        for (int i = 0; i < InEdges.Count; i++)
                        {

                            Edge temp = InEdges[i];
                            Points p = GetDirection(temp.Start,this);
                            temp.SetCoords(p.a,p.b);
                        }
                        for (int i = 0; i < OutEdges.Count; i++)
                        {
                            Edge temp = OutEdges[i];
                            Points p = GetDirection(this,temp.End);
                            temp.SetCoords(p.a, p.b);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Edges.Count; i++)
                        {
                            Edge temp = Edges[i];
                            temp.isDirected = false;
                            Points p = GetDirection(temp.Start, temp.End);
                            temp.SetCoords(p.a, p.b);
                        }
                    }
                }
            }
            public void Delete()
            {
                if (!isDirected) // irányított
                {
                    for (int i = 0; i < Edges.Count;)
                    {
                        Edge temp = Edges[i];
                        Edges.RemoveAt(i);
                        temp.DeleteEdge();
                        EdgeAdded();
                    }
                }
                else // nem irányított
                {
                    for (int i = 0; i < InEdges.Count; )
                    {
                        Edge temp = InEdges[i];
                        InEdges.RemoveAt(i);
                        temp.DeleteEdge();
                        EdgeAdded();
                    }
                    for (int i = 0; i < OutEdges.Count; )
                    {
                        Edge temp = OutEdges[i];
                        OutEdges.RemoveAt(i);
                        temp.DeleteEdge();
                        EdgeAdded();
                    }
                }
                this.parent.Children.Remove(this);
                this.Visibility = System.Windows.Visibility.Hidden;
            }
        #endregion


    }
   


    
}