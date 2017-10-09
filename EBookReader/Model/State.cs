using System;
using System.Collections.Generic;
using System.Windows;

namespace EBookReader.Model
{
    [Serializable]
    public class State
    {
        public List<Book> Books { get; set; }

        public Book CurrentBook
        {
            get { return Books.Count != 0 ? Books[0] : null; }
            set { }
        }

        public Thickness Margin { get; set; }

        public double Zoom { get; set; }

        public double WindowHeight { get; set; }

        public double WindowWidth { get; set; }

        public bool NightMode { get; set; }

        public bool Maximized { get; set; }

        public bool ShowBookList { get; set; }

        public double BookListWidth { get; set; }

        public State()
        {
            if (Books == null)
            {
                Books = new List<Book>();
            }
        }
    }
}