using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using EBookReader.Model;
using Microsoft.Win32;
using EBookReader.ViewModels;
using EBookReader.Util;

namespace EBookReader
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private State _state;

        public MainWindow()
        {
            _mainWindowViewModel = new MainWindowViewModel
            {
                FullScreen = false,
                BookList = true
            };
            DataContext = _mainWindowViewModel;
            InitializeComponent();

            RestoreState();
        }

        private void SetFullScreen(bool fullScreen)
        {
            if (fullScreen)
            {
                _state.WindowWidth = Width;
                _state.WindowHeight = Height;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Left = 0;
                Top = 0;
                Width = SystemParameters.VirtualScreenWidth;
                Height = SystemParameters.VirtualScreenHeight;
                Topmost = true;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                Width = _state.WindowWidth;
                Height = _state.WindowHeight;
            }
        }

        private void RestoreState()
        {
            _state = Serializer.Deserialize();
            if (_state == null)
            {
                _state = new State();
                return;
            }

            _mainWindowViewModel.Books = new ObservableCollection<Book>(_state.Books);

            SetupDocument();

            var window = GetWindow(DocumentReader);
            if (window == null)
            {
                return;
            }

            window.Height = _state.WindowHeight;
            window.Width = _state.WindowWidth;
            MarginSlider.Value = _state.Margin.Left;

            if (DocumentReader.FlowDocument != null)
            {
                DocumentReader.FlowDocument.PagePadding = _state.Margin;
            }

            DocumentReader.Zoom = _state.Zoom;
            _mainWindowViewModel.BookList = _state.ShowBookList;
            ShowBookList(_state.ShowBookList, true);

            if (_state.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            var thread = new Thread(LoadPreviousState);
            thread.Start();
        }

        private void FileOpen()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                OpenBook(dialog.FileName, null);
            }
        }

        private void OpenBook(string filePath, Book book)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"The book {book.Title} was deleted!", "Error");
                _state.Books.Remove(book);
                _mainWindowViewModel.Books.Remove(book);
                return;
            }

            if (_state.CurrentBook != null)
            {
                _state.CurrentBook.PageCount = DocumentReader.PageCount;
                _state.CurrentBook.CurrentPage = DocumentReader.MasterPageNumber;
            }

            if (book == null)
            {
                var b = _state.Books.FirstOrDefault(x => x.FilePath == filePath);
                if (b != null)
                {
                    _state.Books.Remove(b);
                    _state.Books.Insert(0, b);
                    _mainWindowViewModel.Books.Remove(b);
                }
                else
                {
                    _state.Books.Insert(0, new Book());
                }
            }
            else
            {
                _state.Books.Remove(book);
                _state.Books.Insert(0, book);
                _mainWindowViewModel.Books.Remove(book);
            }

            _state.CurrentBook.FilePath = filePath;
            _state.CurrentBook.Title = Path.GetFileNameWithoutExtension(filePath);
            FinishLoadingDocument();
            _mainWindowViewModel.Books.Insert(0, _state.CurrentBook);
            SetupDocument();

            if (book != null)
            {
                var thread = new Thread(LoadPreviousState);
                thread.Start();
            }
        }

        private void SetupDocument()
        {
            if (_state.CurrentBook == null)
            {
                return;
            }

            var text = File.ReadAllText(_state.CurrentBook.FilePath);
            var bytes = Encoding.UTF8.GetBytes(text);
            text = Encoding.UTF8.GetString(bytes);

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(text);

            var document = new FlowDocument(paragraph);
            DocumentReader.SetDocument(document);
            DocumentReader.FlowDocument.PagePadding = _state.Margin;
            document.ColumnWidth = 2000;
            document.TextAlignment = TextAlignment.Center;

            if (_state.NightMode)
            {
                DocumentReader.SetNightMode();
            }
            else
            {
                DocumentReader.SetDayMode();
            }
            _mainWindowViewModel.NightMode = _state.NightMode;

            foreach (var mark in _state.CurrentBook.Marks)
            {
                DocumentReader.RestoreMark(mark, _state.NightMode);
            }
        }

        private void LoadPreviousState()
        {
            Dispatcher.Invoke(() => { ResizeMode = ResizeMode.NoResize; });
            var pageCount = -1;
            var done = false;

            while (!done)
            {
                Dispatcher.Invoke(() =>
                    {
                        if (DocumentReader.PageCount != pageCount)
                        {
                            pageCount = DocumentReader.PageCount;
                        }
                        else
                        {
                            done = true;
                        }
                    }
                );
                Thread.Sleep(100);
            }
            Dispatcher.Invoke(() =>
            {
                if (_state.CurrentBook != null)
                {
                    FinishLoadingDocument();
                    double newPageDouble = _state.CurrentBook.CurrentPage * DocumentReader.PageCount /
                                           _state.CurrentBook.PageCount;
                    var newPage = (int) Math.Ceiling(newPageDouble);
                    DocumentReader.GoToPage(newPage);
                }
            });

            Thread.Sleep(300);
            Dispatcher.Invoke(() => { ResizeMode = ResizeMode.CanResize; });
        }

        private void MarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DocumentReader?.SetMargin(e.NewValue);
            if (_state != null)
            {
                _state.Margin = DocumentReader.FlowDocument.PagePadding;
            }
        }

        private void PageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DocumentReader?.PageCount > e.NewValue)
                DocumentReader.GoToPage((int) e.NewValue);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _mainWindowViewModel.MaxMargin = ((Window) sender).Width;
        }

        private void PageNumberTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            int pageNumber;
            if (int.TryParse(PageNumberTextBox.Text, out pageNumber))
            {
                DocumentReader.GoToPage(pageNumber);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_state != null)
            {
                if (DocumentReader.FlowDocument != null)
                {
                    _state.Margin = DocumentReader.FlowDocument.PagePadding;
                }
                var window = GetWindow(DocumentReader);
                _state.WindowHeight = window.Height;
                _state.WindowWidth = window.Width;
                _state.Zoom = DocumentReader.Zoom;
                _state.Maximized = WindowState == WindowState.Maximized;
                if (BookListColumn.ActualWidth > 0)
                {
                    _state.BookListWidth = BookListColumn.ActualWidth;
                }

                if (_state.CurrentBook != null)
                {
                    _state.CurrentBook.CurrentPage = DocumentReader.MasterPageNumber;
                    _state.CurrentBook.PageCount = DocumentReader.PageCount;
                }

                Serializer.Serialize(_state);
            }
        }

        private void FullScreen()
        {
            _mainWindowViewModel.FullScreen = !_mainWindowViewModel.FullScreen;
            SetFullScreen(_mainWindowViewModel.FullScreen);
        }

        private void FileOpen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileOpen();
        }

        private void NightMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NightMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentReader == null) return;

            _state.NightMode = !_state.NightMode;
            _mainWindowViewModel.NightMode = _state.NightMode;
            if (_state.NightMode)
            {
                DocumentReader.SetNightMode();
            }
            else
            {
                DocumentReader.SetDayMode();
            }
        }

        private void FullScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FullScreen();
        }

        private void BookList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                var item = (TextBlock) e.OriginalSource;
                var book = (Book) item.DataContext;
                OpenBook(book.FilePath, book);
            }
        }

        private void Mark_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DocumentReader != null)
            {
                if (DocumentReader.Selection != null) e.CanExecute = DocumentReader.Selection.Text.Length > 0;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Mark_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentReader?.MarkText(true, _state);
        }

        private void UnMark_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DocumentReader != null)
            {
                if (DocumentReader.Selection != null) e.CanExecute = DocumentReader.Selection.Text.Length > 0;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void UnMark_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentReader?.MarkText(false, _state);
        }


        private void ShowBookList(bool show, bool first)
        {
            if (show)
            {
                BookListColumn.Width = new GridLength(_state.BookListWidth);
                BookListGridSplitter.Width = 5;
            }
            else
            {
                if (!first)
                {
                    _state.BookListWidth = BookListColumn.ActualWidth;
                }
                BookListColumn.Width = new GridLength(0);
                BookListGridSplitter.Width = 0;
            }
        }

        private void ShowBooks_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowBooks_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mainWindowViewModel.BookList = !_mainWindowViewModel.BookList;
            _state.ShowBookList = _mainWindowViewModel.BookList;
            ShowBookList(_mainWindowViewModel.BookList, false);
        }

        private void FinishLoadingDocument()
        {
            var title = _state.CurrentBook.Title;
            if (title.Length > 20)
            {
                title = _state.CurrentBook.Title.Substring(0, 20);
            }
            Title = $"Ebook Reader - {title}...";
            _mainWindowViewModel.DocumentLoaded = true;
        }
    }
}