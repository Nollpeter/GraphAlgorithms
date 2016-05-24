using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using System.Windows.Threading;

namespace SzelessegiBejaras
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            //Button a = new Button();
            //canvasbutton.Click += new RoutedEventHandler((a, b) => { createButton(Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y); });
            canvasButton.dragCanvas.AllowDragging = true;
            canvasButton.dragCanvas.AllowDragOutOfView = false;
            DrawingGrid = LayoutRoot;
            //tempEdge = new Edge();
            myGraph = new Graph((bool)SettingsPanel.IsDirectedCheckBox.IsChecked.Value);
            //SettingsPanel.IsDirectedCheckBox.Content = "TEszt";
            CommandBinding helpBinding = new CommandBinding(ApplicationCommands.Help);
            //helpBinding.CanExecute += CanHelpExecute;
            helpBinding.Executed += new ExecutedRoutedEventHandler((a, b) => { HelpWindow h = new HelpWindow(); h.Show(); });
            CommandBindings.Add(helpBinding);
           
            
        }
        public static Grid DrawingGrid;
        
     
        public void OnEdgeAdded()
        {
            SettingsPanel.EdgeCountTextBlock.Text = myGraph.EdgeCount().ToString();
        }
        Edge tempEdge;// = new Edge();
        public static Graph myGraph;
        void createButton (double X, double Y)
        {
            Vertex mybut = new Vertex();
            mybut.EdgeAdded += OnEdgeAdded;//new Vertex.VertexEventHandler(() => { SettingsPanel.EdgeCountTextBlock.Text = myGraph.EdgeCount().ToString(); });
            mybut.CurrentlySelectedVertexChanged += OnVertexCurrentlySelectedVertexChanged;
            canvasButton.dragCanvas.Children.Add(mybut);
            mybut.Margin = new Thickness(X,Y, 0, 0);
            mybut.Padding = new Thickness(0);
            myGraph.AddVertex(mybut);
            mybut.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            mybut.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            mybut.Visibility = System.Windows.Visibility.Visible;
            mybut.AddParent(LayoutRoot);
        }
        private void OnVertexCurrentlySelectedVertexChanged(GraphEventArgs e)
        {
            if (e.CurrentVertex != null)
            {
                SettingsPanel.CurrentlySelectedVertexTextBlock.Text = (e.CurrentVertex.Number.ToString() + ". számú csúcs");
            }
            else
            {
                SettingsPanel.CurrentlySelectedVertexTextBlock.Text = "Nincs kiválasztott csúcs";
            }
            myGraph.SelectedVertex = e.CurrentVertex;
        }


        private void tempBut_Click(object sender, RoutedEventArgs e)
        {
            tempEdge.SetStartEnd(tempEdge.Start, tempEdge.End);
            
        }

        private void dragCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            createButton(e.GetPosition(this).X, e.GetPosition(this).Y);

            SettingsPanel.VertexCountTextBlock.Text = myGraph.Count.ToString();
            
        }

        private void GraphReset_Click(object sender, RoutedEventArgs e)
        {
            myGraph.Reset();
           
        }
        private void Graph_IsDirectedChange(object sender, RoutedEventArgs e)
        {
            if (myGraph.Count != 0)
            {
                if (SettingsPanel.IsDirectedCheckBox.IsChecked.Value == false)
                {
                    myGraph.ConvertToUndirected();
                }
                else
                {
                    if(myGraph.ContainsEdges())
                    {
                        MessageBoxResult result = MessageBox.Show("Csak irányított gráfot lehet irányítatlanná változtatni, fordított esetben a gráf törlődik. Törli a gráfot?", "Hiba", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            SettingsPanel_DeleteGraphClicked(sender, e);
                        }
                        else
                        {
                            SettingsPanel.IsDirectedCheckBox.IsChecked = !SettingsPanel.IsDirectedCheckBox.IsChecked;
                            return;
                        }
                    }
                    else
                    {
                       // SettingsPanel.IsDirectedCheckBox.IsChecked = !SettingsPanel.IsDirectedCheckBox.IsChecked;
                    }
                }
            }
                myGraph.IsDirected = SettingsPanel.IsDirectedCheckBox.IsChecked.Value;
        }

        private void canvasButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void canvasButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
                dragCanvas_MouseLeftButtonDown(sender, e);
            
        }

        private void SettingsPanel_DeleteEdgeClicked(object sender, RoutedEventArgs e)
        {
            if(Edge.selectedEdge != null)
            {
                Edge.selectedEdge.DeleteEdge();
                SettingsPanel.EdgeCountTextBlock.Text =myGraph.EdgeCount().ToString();
            }
        }

        private void SettingsPanel_DeleteGraphClicked(object sender, RoutedEventArgs e)
        {
            myGraph.Reset();
            SettingsPanel.VertexCountTextBlock.Text = myGraph.Count.ToString();
        }

        private void SettingsPanel_DeleteVertexClicked(object sender, RoutedEventArgs e)
        {
            if(myGraph.SelectedVertex != null)
            {
                myGraph.DeleteVertex(myGraph.SelectedVertex);
                SettingsPanel.VertexCountTextBlock.Text = myGraph.Count.ToString();
                Vertex.startPoint = null;
                Vertex.edgeDrawing = Vertex.EdgeDrawingState.idle;
            }
            
        }
        private void SettingsPanel_ResetAllVisitedStateClicked(object sender, RoutedEventArgs e)
        {
            myGraph.ResetAllVisitedState();
        }
        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.G: SettingsPanel_DeleteGraphClicked(sender, e); break;
                case Key.C: SettingsPanel_DeleteVertexClicked(sender, e); break;
                case Key.E: SettingsPanel_DeleteEdgeClicked(sender, e); break;
                case Key.R: SettingsPanel_ResetAllVisitedStateClicked(sender, e); break;
                case Key.I: SettingsPanel.IsDirectedCheckBox.IsChecked = !SettingsPanel.IsDirectedCheckBox.IsChecked; break;
                case Key.S:
                    {
                        myGraph[0].CurrentlySelectedVertexChanged += null;
                        //OnVertexCurrentlySelectedVertexChanged(new GraphEventArgs() { CurrentVertex = null });
                        Vertex.EdgeDrawing = Vertex.EdgeDrawingState.idle;
                        
                    } break;
            }
            
           
        }

        
    }
}
