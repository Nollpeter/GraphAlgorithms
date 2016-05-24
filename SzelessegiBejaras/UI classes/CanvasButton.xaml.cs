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
	/// Interaction logic for CanvasButton.xaml
	/// </summary>
	public partial class CanvasButton : Button
	{
		public CanvasButton()
		{
			this.InitializeComponent();
            
		}
        
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = Brushes.White;
            //this.MouseMove += (a, b) => { a = e.GetPosition(this); MessageBox.Show(a.ToString()); };
            
            
        }

        

        private void canvasbutton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.Content = (e.GetPosition(this).ToString());
            //createButton(e.GetPosition(this).X, e.GetPosition(this).Y);
            
        }
        void createButton (double X, double Y)
        {
            
            
        }
	}
}