using Luizanac.QueryExtensions.Abstractions.Interfaces;

namespace Luizanac.QueryExtensions.Abstractions
{
    /// <summary>
    ///  The helper class to hold paginated data
    /// </summary>
    /// <typeparam name="T">The type of paginated data</typeparam>
    public class Pagination<T> : IPagination<T>
    {
        /// <summary>
        /// The paginated data
        /// </summary>       
        public T Data { get; set; }

        /// <summary>
        /// The total pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The total data count
        /// </summary>
        public int TotalDataCount { get; set; }

        /// <summary>
        /// The current page number
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The number of datas to get
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///  The previous page number
        /// </summary>
        public int PrevPage { get; set; }

        /// <summary>
        /// The next page number
        /// </summary>
        public int NextPage { get; set; }

        public Pagination(T data, int totalPages, int currentPage, int size, int prevPage, int nextPage, int totalDataCount)
        {
            Data = data;
            TotalPages = totalPages;
            CurrentPage = currentPage;
            Size = size;
            PrevPage = prevPage;
            NextPage = nextPage;
            TotalDataCount = totalDataCount;
        }
    }
}