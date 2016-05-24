using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SzelessegiBejaras.Containers
{
    public class PriorityQueueEventArgs:EventArgs
    {
        public List<Vertex> Vertices;
        public List<int> Priorities;
    }
    public class PriorityQueue : IEnumerable
    {
        List<Vertex> items;
        List<int> priorities;
        private ListBox listBox;
        public delegate void ListBoxEventHandler (object sender, PriorityQueueEventArgs e);
        public event ListBoxEventHandler QueueChanged;
        public PriorityQueue()
        {
            items = new List<Vertex>();
            priorities = new List<int>();
        }

        public IEnumerator GetEnumerator() { return items.GetEnumerator(); }
        public int Count { get { return items.Count; } }

        /// <returns>Index of new element</returns>
        public int Enqueue(Vertex item, int priority)
        {
            for (int i = 0; i < priorities.Count; i++) //go through all elements...
            {

                if (priorities[i] > priority)// && priority != -1) //...as long as they have a lower priority.    low priority = low index
                {
                    items.Insert(i, item);
                    priorities.Insert(i, item.Distance);
                    if (QueueChanged != null)
                    QueueChanged(this, new PriorityQueueEventArgs() { Vertices = items, Priorities = priorities });
                    //Vertex temp = item as Vertex;
                    item.DistanceChanged += func;
                    return i;
                }

            }

            items.Add(item);
            //Vertex temp2 = item as Vertex;
            item.DistanceChanged += func;
            priorities.Add(priority);
            if(QueueChanged!=null)
            QueueChanged(this, new PriorityQueueEventArgs() { Vertices = items, Priorities = priorities });
            return items.Count - 1;
        }
        public bool Contains(Vertex element)
        {

            foreach (Vertex item in this)
            {
                if (EqualityComparer<Vertex>.Default.Equals(item, element))
                {
                    return true;
                }
            }
            return false;
        }
        private void func(GraphEventArgs e)
        {
            if (items.Contains(e.CurrentVertex))
            {
                
                //int j = GetIndexOfElement(e.CurrentVertex);
                priorities[items.IndexOf(e.CurrentVertex)] = e.CurrentVertex.Distance;
            }


        }
      
        
        public void AddListBox(ListBox listBox)
        {
            this.listBox = listBox;

        }
        private int GetIndexOfElement(Vertex item)
        {
            int i = 0;
            while (item != items[i])//while (EqualityComparer<Vertex>.Default.Equals(item, items[i]))
            {
                i++;
            }
            return i;
        }
        public Vertex Dequeue()
        {
            Vertex item = items[0];
            priorities.RemoveAt(0);
            items.RemoveAt(0);
            if (QueueChanged != null)
            QueueChanged(this, new PriorityQueueEventArgs() { Vertices = items, Priorities = priorities });
            return item;
        }

        public Vertex Peek()
        {
            return items[0];
        }

        public int PeekPriority()
        {
            return priorities[0];
        }
        public void Repair()
        {
            Quicksort(0, items.Count - 1);
        }
        private void Quicksort(int left, int right)
        {
            int i = left, j = right;
            int pivot = priorities[(left + right) / 2];

            while (i <= j)
            {
                while (priorities[i] < pivot)
                {
                    i++;
                }

                while (priorities[j] > pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    int tmp = priorities[i];
                    priorities[i] = priorities[j];
                    priorities[j] = tmp;
                    Vertex tmpT = items[i];
                    items[i] = items[j];
                    items[j] = tmpT;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(left, j);
            }

            if (i < right)
            {
                Quicksort(i, right);
            }
        }

    }
}
