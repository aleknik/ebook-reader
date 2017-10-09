using System;
using System.Collections.Generic;

namespace EBookReader.Model
{
    [Serializable]
    public class Book
    {
        public string FilePath { get; set; }

        public string Title { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public List<Mark> Marks { get; set; }

        public Book()
        {
            Marks = new List<Mark>();
        }

        public void AddMark(int start, int end)
        {
            Marks.Add(new Mark(start, end, true));
            /*
            Marks.Add(new Mark(start, end));
            Marks = Marks.OrderBy(m => -m.Start).ToList();

            var index = 0;

            for (var i = 0; i < Marks.Count; i++)
            {
                if (index != 0 && Marks[index - 1].Start <= Marks[i].End)
                {
                    while (index != 0 && Marks[index - 1].Start <= Marks[i].End)
                    {
                        Marks[index - 1].End = Math.Max(Marks[index - 1].End, Marks[i].End);
                        Marks[index - 1].Start = Math.Min(Marks[index - 1].Start, Marks[i].Start);
                        index--;
                    }
                }
                else
                    Marks[index] = Marks[i];

                index++;
            }


            Marks = Marks.GetRange(0, index);
            */
        }

        public void Unmark(int start, int end)
        {
            Marks.Add(new Mark(start, end, false));
            /*
            if (Marks.Count == 0) return;

            Marks = Marks.OrderBy(m => m.Start).ToList();

            var i = 0;
            while (start < Marks[i].Start && i < Marks.Count)
            {
                i++;
            }

            if (i > 0 && start < Marks[i - 1].End)
            {
                Marks[i - 1].End = start;
            }

            while (i < Marks.Count && Marks[i].End < end)
            {
                Marks.RemoveAt(i);
                i++;
            }

            if (i < Marks.Count && Marks[i].Start < end)
            {
                Marks[i].Start = end;
            }
            */
        }
    }
}