using System;

namespace nFact.viewModels.Controls
{
    public class PaginationViewModel
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public int PageMin { get; set; }
        public int PageMax { get; set; }
        public int PageCurrent { get; set; }
        public int NumOfPages { get; set; }
        public bool NextVisible { get; set; }
        public bool PrevVisible { get; set; }
        public string Spec { get; set; }

        public PaginationViewModel(string spec, int pages)
        {
            Spec = spec;
            NumOfPages = pages;
        }

        public void Set(int min, int current, int max)
        {
            Max = max;
            Min = min;

            if (current - NumOfPages < min)
            {
                PageMin = min;
                PageMax = min + NumOfPages - 1;
            }
            else if ((current + NumOfPages) > max)
            {
                PageMin = max - NumOfPages + 1;
                PageMax = max;
            }
            else
            {
                PageMin = current - Convert.ToInt32(Math.Floor((double)NumOfPages / 2));
                PageMax = PageMin + NumOfPages - 1;
            }

            if (min < PageMin)
                PrevVisible = true;

            if (max > PageMax)
                NextVisible = true;


            if (PageMax > max)
                PageMax = max;

            if (PageMin < min)
                PageMin = min;

            PageCurrent = current;
        }
    }
}