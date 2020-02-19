namespace Luizanac.Utils.Extensions.Interfaces
{
    public interface IPagination<T>
    {
         /// <summary>
        /// The paginated data
        /// </summary>       
        T Data { get; set; }

        /// <summary>
        /// The total pages
        /// </summary>
        int TotalPages { get; set; }

        /// <summary>
        /// The total data count
        /// </summary>
        int TotalDataCount { get; set; }

        /// <summary>
        /// The current page number
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// The number of datas to get
        /// </summary>
        int Size { get; set; }

        /// <summary>
        ///  The previous page number
        /// </summary>
        int PrevPage { get; set; }

        /// <summary>
        /// The next page number
        /// </summary>
        int NextPage { get; set; }
    }
}