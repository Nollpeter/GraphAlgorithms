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

namespace SzelessegiBejaras
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : UserControl
	{
		public Settings()
		{
			this.InitializeComponent();
            EdgeCountTxtBlock = EdgeCountTextBlock;
            DataContext = new StaticResource();
		}
        public static bool Proceed = false;
        public static TextBlock EdgeCountTxtBlock;
        //public event Vertex.VertexEventHandler IsDirectedChange;
        //public static readonly DependencyProperty IsDirectedChangeProperty;
        
        public static readonly RoutedEvent IsDirectedChangeEvent = EventManager.RegisterRoutedEvent("IsDirectedChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Settings));
        public static readonly RoutedEvent DeleteGraph = EventManager.RegisterRoutedEvent("DeleteGraph", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Settings));
        public static readonly RoutedEvent DeleteVertex = EventManager.RegisterRoutedEvent("DeleteVertex", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Settings));
        public static readonly RoutedEvent DeleteEdge = EventManager.RegisterRoutedEvent("DeleteEdge", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Settings));
        public static readonly RoutedEvent ResetAllVisitedState = EventManager.RegisterRoutedEvent("ResetGraph", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Settings));
        /*private void IsDirectedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(IsDirectedChange != null)
            {
                IsDirectedChange();
                //EventManager.RegisterRoutedEvent()
            }
        }*/
        public Algorithms.GraphAlgorithm SelectedAlgorithm { get; set; }
        public event RoutedEventHandler IsDirectedChange
        {
            add { AddHandler(IsDirectedChangeEvent, value); }
            remove { RemoveHandler(IsDirectedChangeEvent, value); }
        }
        public event RoutedEventHandler DeleteGraphClicked
        {
            add { AddHandler(DeleteGraph, value); }
            remove { RemoveHandler(DeleteGraph, value); }
        }
        public event RoutedEventHandler DeleteVertexClicked
        {
            add { AddHandler(DeleteVertex, value); }
            remove { RemoveHandler(DeleteVertex, value); }
        }
        public event RoutedEventHandler DeleteEdgeClicked
        {
            add { AddHandler(DeleteEdge, value); }
            remove { RemoveHandler(DeleteEdge, value); }
        }
        public event RoutedEventHandler ResetAllVisitedStateClicked
        {
            add { AddHandler(ResetAllVisitedState, value); }
            remove { RemoveHandler(ResetAllVisitedState, value); }
        }
        static Settings()
        {
            /*IsDirectedChangeProperty = DependencyProperty.Register(
                "IsDirectedChange",
                typeof(Vertex.VertexEventHandler),
                typeof(Settings),
                new PropertyMetadata(null));*/
        }
        private void DeleteGraphButton_Clicked(object sender,RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DeleteGraph);
            RaiseEvent(newEventArgs);
        }
        private void DeleteVertexButton_Clicked(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DeleteVertex);
            RaiseEvent(newEventArgs);
        }
        private void DeleteEdgeButton_Clicked(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DeleteEdge);
            RaiseEvent(newEventArgs);
        }
        private void IsDirectedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(IsDirectedChangeEvent);
            RaiseEvent(newEventArgs);
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnimationPauseTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(SelectedAlgorithm != null)
            //AnimationPauseTimeLabel.Content = ((int)AnimationPauseTime.Value).ToString();
            this.SelectedAlgorithm.PauseTime = (int)this.AnimationPauseTime.Value;
        }

        private void AlgorithmChooseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (AlgorithmChooseComboBox.SelectedIndex)
            {
                case 0:
                    {
                        this.SelectedAlgorithm = new Algorithms.BreadthFirstSearch(MainWindow.myGraph, Vertex.startPoint, AlgorithmStateListBox, this.AnimationPauseTime.Value);
                        MainWindow.myGraph.SetLabelVisibilities(false);
                        MainWindow.myGraph.IsExpanded = false;
                    } break;
                case 1:
                    {
                        this.SelectedAlgorithm = new Algorithms.DijkstraAlgorithm(MainWindow.myGraph, Vertex.startPoint, AlgorithmStateListBox, this.AnimationPauseTime.Value);
                        MainWindow.myGraph.SetLabelVisibilities(true);
                        MainWindow.myGraph.IsExpanded = true;
                    } break;
                case 2:
                    {
                        this.SelectedAlgorithm = new Algorithms.PrimAlgorithm(MainWindow.myGraph, Vertex.startPoint, AlgorithmStateListBox, this.AnimationPauseTime.Value);
                        MainWindow.myGraph.SetLabelVisibilities(true);
                        MainWindow.myGraph.IsExpanded = false;
                    }break;
            }
            
        }

        private void ResetAllVisitedState_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.myGraph.ResetAllVisitedState();
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ResetAllVisitedState);
            RaiseEvent(newEventArgs);
        }
        private void RunAlgorithm(object sender, RoutedEventArgs e)
        {
            if (this.SelectedAlgorithm != null)
                this.SelectedAlgorithm.Run();
            else
                MessageBox.Show("Válasszon ki egy algoritmust!");
        }
        private void VisibleLabelsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myGraph != null) 
            MainWindow.myGraph.SetLabelVisibilities(true);
        }

        private void VisibleLabelsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myGraph != null) 
            MainWindow.myGraph.SetLabelVisibilities(false);
        }

        private void VisibleDistanceLabelsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myGraph != null)
                MainWindow.myGraph.IsExpanded = true;
        }

        private void VisibleDistanceLabelsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.myGraph != null)
                MainWindow.myGraph.IsExpanded = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(this.SelectedAlgorithm != null)
            this.SelectedAlgorithm.ProceedOnlyOnButtonPress = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.SelectedAlgorithm != null)
            this.SelectedAlgorithm.ProceedOnlyOnButtonPress = false;
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            Proceed = true;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        
	}
}