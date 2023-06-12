using System;

namespace OCM.TFI.OCM.Utilities
{
    public class PagingModel
    {
        public int PageSize { get; set; } = 5;

        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public int TotalItemCount { get; set; }
    }
}
