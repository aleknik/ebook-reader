using System.Collections.ObjectModel;
using EBookReader.Model;

namespace EBookReader.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private double _maxMargin;

        public double MaxMargin
        {
            get { return _maxMargin; }
            set
            {
                _maxMargin = value * 0.3;
                OnPropertyChanged("MaxMargin");
            }
        }

        private ObservableCollection<Book> _books;

        public ObservableCollection<Book> Books
        {
            get { return _books; }
            set
            {
                _books = value;
                OnPropertyChanged("Books");
            }
        }

        private bool _nightMode;

        public bool NightMode
        {
            get { return _nightMode; }
            set
            {
                _nightMode = value;
                OnPropertyChanged("NightMode");
            }
        }

        private bool _fullScreen;

        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                _fullScreen = value;
                OnPropertyChanged("FullScreen");
            }
        }

        private bool _bookList;

        public bool BookList
        {
            get { return _bookList; }
            set
            {
                _bookList = value;
                OnPropertyChanged("BookList");
            }
        }

        public MainWindowViewModel()
        {
            Books = new ObservableCollection<Book>();
        }

        private bool _documentLoaded;

        public bool DocumentLoaded
        {
            get { return _documentLoaded; }
            set
            {
                _documentLoaded = value;
                OnPropertyChanged("DocumentLoaded");
            }
        }
    }
}