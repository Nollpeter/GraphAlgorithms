using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace SzelessegiBejaras
{
    public partial class Edge : UserControl
    {
        public Edge()
        {
            isDirected = false;
            initialize();
            ++count;
            LabelVisible = Edge.labelVisible;
            //Settings.EdgeCountTxtBlock.Text = (++count).ToString() ;

        }
        public Edge(bool isDirectedEdge)
        {
            isDirected = isDirectedEdge;
            initialize();
            ++count;
            LabelVisible = Edge.labelVisible;
            
            //Settings.EdgeCountTxtBlock.Text = (++count).ToString();
            
        }

        void Edge_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if( this.logicalState == LogicalState.Idle)
            {
                this.logicalState = LogicalState.SelectedForEdgeDrawing;
                selectedEdge = this;
            }
            else
            {
                this.logicalState = LogicalState.Idle;
                selectedEdge = null;
            }
            
        }
        public Edge(Edge other)
        {
            isDirected = other.isDirected;
            DirectedLine = other.DirectedLine;
            line = other.line;
            Start = other.Start;
            End = other.End;
            ParentGrid = other.ParentGrid;
        }
        #region Variables

        public double length=0;
        public bool isDirected;
        public static int count = 0;
        public static bool labelVisible = false;
        public static Edge selectedEdge;
        public enum LogicalState { Idle, SelectedForEdgeDrawing, Visited, NotYetVisited }
        private LogicalState _logicalState;
        public event Vertex.GraphEventHandler LogicalStateChanged;

        #endregion

        #region Properties

            public bool IsLoop { get { return Start == End; } }
            public Arrow DirectedLine { get; set; }
            public Vertex Start { get; set; }
            public Vertex End { get; set; }
            public Arrow PrevArrow { get; set; }
            public Grid ParentGrid { get; set; }
            public TextBox LabelBlock { get; set; }

            public double Length
            {
                get { return length; }
                set { length = value; }
            }
            public bool LabelVisible
            {
                get { return labelVisible; }
                set
                {
                    labelVisible = value; if (value == false) { LabelBlock.Visibility = System.Windows.Visibility.Hidden; }
                    else
                    { LabelBlock.Visibility = System.Windows.Visibility.Visible; } } }
            public bool IsDirected { get { return isDirected; } }
            public Edge.LogicalState logicalState
            {
                get { return this._logicalState; }
                set { _logicalState = value; LogicalStateChanged(new GraphEventArgs() {EdgeLogicalState = value}); }
            }
            
        #endregion
            public void OnLogicalStateChanged(GraphEventArgs e)
            {
                if(!IsDirected)
                {
                    switch (e.EdgeLogicalState)
                    {
                        case Edge.LogicalState.Idle: line.Stroke = Brushes.Black; break;
                        case Edge.LogicalState.NotYetVisited: line.Stroke = Brushes.Black; break;
                        case Edge.LogicalState.Visited: line.Stroke = Brushes.Green; break;
                        case Edge.LogicalState.SelectedForEdgeDrawing: line.Stroke = Brushes.Red; break;
                    }
                }
                else
                {
                    switch (e.EdgeLogicalState)
                    {
                            
                        case Edge.LogicalState.Idle: DirectedLine.Stroke = Brushes.Black; break;
                        case Edge.LogicalState.NotYetVisited: DirectedLine.Stroke = Brushes.Black; break;
                        case Edge.LogicalState.Visited: DirectedLine.Stroke = Brushes.Green; break;
                        case Edge.LogicalState.SelectedForEdgeDrawing: DirectedLine.Stroke = Brushes.Red; break;
                    }
                }
                
            }
        public void SetStartEnd( Vertex start, Vertex end)
        {
            Start = start;
            End = end;
            if(start != end)
            {
                Vertex.Points p = start.GetDirection(start, end);
                //SetCoords(new Point(start.Margin.Left, start.Margin.Top), new Point(end.Margin.Left, end.Margin.Top));
                SetCoords(p.a, p.b);
                if (isDirected)
                {
                    Start.AddEdge(this, Vertex.InOrOut.OUT);
                    End.AddEdge(this, Vertex.InOrOut.IN);
                    DrawDirectedLine();
                }
                else
                {
                    Start.AddEdge(this);
                    End.AddEdge(this);
                    DrawLine();
                }
                DrawLabel();
            }
            else
            {
                MessageBox.Show("Hurokél nem megengedett!");
                this.DeleteEdge();
            }
            
        }
        public void SetCoords(Point A, Point B)
        {
            if (isDirected)
            {
                DirectedLine.X1 = A.X;
                DirectedLine.X2 = B.X;
                DirectedLine.Y1 = A.Y;
                DirectedLine.Y2 = B.Y;
                PrevArrow.X2 = (DirectedLine.X1 *1.5 + DirectedLine.X2) / 2.5;
                PrevArrow.X1 = (DirectedLine.X1 + DirectedLine.X2 * 1.5) / 2.5;
                PrevArrow.Y2 = (DirectedLine.Y1 * 1.5 + DirectedLine.Y2) / 2.5;
                PrevArrow.Y1 = (DirectedLine.Y1 + DirectedLine.Y2 * 1.5) / 2.5;
                TranslateTransform trans = new TranslateTransform();
                trans.X = -10;
                trans.Y = -10;
                PrevArrow.RenderTransform = trans;
                /*PrevArrow.X1 = (DirectedLine.X1 + DirectedLine.X2) / 2 + 20;
                PrevArrow.X2 = (DirectedLine.X1 + DirectedLine.X2) / 2 - 20;
                PrevArrow.Y1 = (DirectedLine.Y1 + DirectedLine.Y2) / 2+20;
                PrevArrow.Y2 = (DirectedLine.Y2 + DirectedLine.Y2) / 2+20;*/
               /* TransformGroup myTransformGroup = new TransformGroup();
                ScaleTransform scale = new ScaleTransform(0.3,0.3);
                PrevArrow.RenderTransform = scale;
                TranslateTransform trans = new TranslateTransform();
                trans.X = (PrevArrow.X1);
                trans.Y = (PrevArrow.Y1);
                myTransformGroup.Children.Add(scale);
                myTransformGroup.Children.Add(trans);
                //PrevArrow.RenderTransform = myTransformGroup;
                //PrevArrow.StrokeThickness = 10;*/
                /*
                 * van az x1y1 koordináta ami a hosszú nyíl. tehát akkor (x1y1)/2 + x eltolás - ((x1y1)/4)/2**/
                
            }
            else
            {
                line.X1 = A.X;
                line.X2 = B.X;
                line.Y1 = A.Y;
                line.Y2 = B.Y;
            }
            LabelBlock.Margin = new Thickness(Math.Abs((B.X + A.X) / 2), Math.Abs((B.Y + A.Y) / 2), 0, 0);

            

        }
        public void SetPrevArrowCoords()//Point A, Point B)
        {
            

        }
        public void AddGrid(Grid grid)
        {
            ParentGrid = grid;
        }
        public void DrawLine()
        {
            if (!ParentGrid.Children.Contains(line))
            {
                ParentGrid.Children.Add(line);
            }

            line.Visibility = Visibility.Visible;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;//StaticResources.LineStrokeThickness;
        }
        public void DrawPrevArrow()
        {
            if (!ParentGrid.Children.Contains(PrevArrow))
            {
                ParentGrid.Children.Add(PrevArrow);
            }
            PrevArrow.Visibility = Visibility.Visible;
            PrevArrow.HeadWidth = 10;
            PrevArrow.HeadHeight = 5;
            PrevArrow.Stroke = Brushes.Red;
            PrevArrow.StrokeThickness = 4;
        }
        public void DrawDirectedLine()
        {
            if (!ParentGrid.Children.Contains(DirectedLine))
            {
                ParentGrid.Children.Add(DirectedLine);
            }
            
            DirectedLine.Visibility = Visibility.Visible;
            DirectedLine.HeadWidth = 10;
            DirectedLine.HeadHeight = 5;
            DirectedLine.Stroke = Brushes.Black;
            DirectedLine.StrokeThickness = 2;//StaticResources.LineStrokeThickness;
        }
        private void DrawLabel()
        {
            if (!ParentGrid.Children.Contains(LabelBlock))
            {
                ParentGrid.Children.Add(LabelBlock);
            }
            LabelVisible = labelVisible;
            LabelBlock.Height = 20; LabelBlock.Width = 30;
            LabelBlock.MaxWidth = 100;
            LabelBlock.Text = Length.ToString();
            LabelBlock.TextChanged += new TextChangedEventHandler((s, e) => 
            {
                if(!Double.TryParse(LabelBlock.Text, out length))
                {
                   this.Length = 0;
                   this.LabelBlock.Text = "0";
                }
                //this.length = Convert.ToInt16(LabelBlock.Text); 
            });
            LabelBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            LabelBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            LabelBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
        public Line line { get; set; }
        private void initialize ()
        {
            DirectedLine = new Arrow();
            line = new Line();
            Start = new Vertex();
            End = new Vertex();
            LabelBlock = new TextBox();
            PrevArrow = new Arrow();
            //this.ParentGrid.Children.Add(LabelBlock);
            LogicalStateChanged += new Vertex.GraphEventHandler(OnLogicalStateChanged);
            this.logicalState = LogicalState.Idle;
            this.MouseRightButtonDown += Edge_MouseRightButtonDown;
            this.line.MouseRightButtonDown += Edge_MouseRightButtonDown;
            this.DirectedLine.MouseRightButtonDown += Edge_MouseRightButtonDown;
            
        }
        public void DeleteEdge()
        {
            
            this.Visibility = System.Windows.Visibility.Hidden;
            
            
            if(IsDirected)
            {
                this.Start.OutEdges.Remove(this);
                this.End.InEdges.Remove(this);
                this.DirectedLine.Visibility = System.Windows.Visibility.Hidden;
                this.PrevArrow.Visibility = System.Windows.Visibility.Hidden;
                ParentGrid.Children.Remove(DirectedLine);
                ParentGrid.Children.Remove(PrevArrow);
                //this = null;
            }
            else
            {
                this.Start.Edges.Remove(this);
                this.End.Edges.Remove(this);
                this.line.Visibility = System.Windows.Visibility.Hidden;
                ParentGrid.Children.Remove(line);
                //this = null;
            }
            this.LabelBlock.Visibility = System.Windows.Visibility.Hidden;
            ParentGrid.Children.Remove(LabelBlock);
            ParentGrid.Children.Remove(this);
            
        }

    }
}
