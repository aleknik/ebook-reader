using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using EBookReader.Model;

namespace EBookReader.CustomControls
{
    class CustomDocumentReader : FlowDocumentPageViewer
    {
        public FlowDocument FlowDocument;

        public void SetDocument(FlowDocument flowDocument)
        {
            Document = flowDocument;
            FlowDocument = flowDocument;
        }

        public void SetMargin(double margin)
        {
            if (FlowDocument != null)
            {
                FlowDocument.PagePadding = new Thickness(margin, 0, margin, 0);
            }
        }

        public void SetNightMode()
        {
            var flowDocument = Document as FlowDocument;
            if (flowDocument != null)
            {
                flowDocument.Background = Brushes.Bisque;
            }

            Foreground = Brushes.SaddleBrown;
        }

        public void SetDayMode()
        {
            var flowDocument = Document as FlowDocument;
            if (flowDocument != null)
            {
                flowDocument.Background = Brushes.White;
            }

            Foreground = Brushes.Black;
        }

        public void MarkText(bool setMarked, State state)
        {
            if (Selection == null) return;

            var range = new TextRange(Selection.Start, Selection.End);
            var documentStart = ((FlowDocument) Document).ContentStart;

            var start = documentStart.GetOffsetToPosition(Selection.Start);
            var end = documentStart.GetOffsetToPosition(Selection.End);

            if (setMarked)
            {
                state.CurrentBook.AddMark(start, end);
            }
            else
            {
                state.CurrentBook.Unmark(start, end);
            }
            ApplyMark(range, setMarked, state.NightMode);
        }

        public void RestoreMark(Mark mark, bool nightMode)
        {
            var documentStart = ((FlowDocument) Document).ContentStart;

            var start = documentStart.GetPositionAtOffset(mark.Start);
            var end = documentStart.GetPositionAtOffset(mark.End);

            var range = new TextRange(start, end);
            ApplyMark(range, mark.Marked, nightMode);
        }

        private void ApplyMark(TextRange range, bool setMarked, bool nightMode)
        {
            if (setMarked)
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
            }
            else
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty,
                    nightMode ? Brushes.SaddleBrown : Brushes.Black);
            }
        }
    }
}