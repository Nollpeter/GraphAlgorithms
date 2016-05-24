using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SzelessegiBejaras.Containers
{
    public class ExtendedQueue<T> : Queue<T> where T : class
    {
        private ListBox listBox;
        public event EventHandler QueueChanged;
        public ExtendedQueue()
            : base()
        {
            this.QueueChanged += MyQueue_QueueChanged;
        }

        void MyQueue_QueueChanged(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("A sor állapota");

            foreach (T item in this)
            {
                listBox.Items.Add(item.ToString());
            }
        }
        public void AddListBox(ListBox listBox)
        {
            this.listBox = listBox;
        }
        new public void Enqueue(T item)
        {
            base.Enqueue(item);
            QueueChanged(this, new EventArgs());
        }
        new public T Dequeue()
        {
            T item = base.Dequeue();
            QueueChanged(this, new EventArgs());
            return item;

        }

    }
}
