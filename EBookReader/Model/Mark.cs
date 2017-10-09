using System;

namespace EBookReader.Model
{
    [Serializable]
    public class Mark
    {
        public int Start { get; set; }

        public int End { get; set; }

        public bool Marked { get; set; }

        public Mark()
        {
        }

        public Mark(int start, int end)
        {
            Start = start;
            End = end;
        }

        public Mark(int start, int end, bool marked)
        {
            Start = start;
            End = end;
            Marked = marked;
        }
    }
}