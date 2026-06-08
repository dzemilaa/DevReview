using System.Collections.Generic;

namespace DevReview.Application.DTOs
{
    public class PagedResultDto<T>
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public IList<T> Items { get; set; } = new List<T>();
    }
}
