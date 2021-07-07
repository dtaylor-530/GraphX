using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.Bayesian.WPF.Models.Vertices.Pagination
{
    public record Pagination(
        int ItemsCounts,
     int Page,
     int Size,
     int Count,
     int Start,
     int End,
     int StartIndex,
     int EndIndex,
     IReadOnlyCollection<int> Pages) : IPageRequest;


    public class PaginationHelper
    {
        public static Pagination Paginate(int totalItems, int currentPage = 1, int pageSize = 10, int maxPages = 10)
        {
            // calculate total pages
            var totalPages = (int)Math.Ceiling(totalItems / (pageSize * 1d));

            // ensure current page isn't out of range
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            else if (currentPage > totalPages)
            {
                currentPage = totalPages;
            }

            int startPage, endPage;
            if (totalPages <= maxPages)
            {
                // total pages less than max so show all pages
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // total pages more than max so calculate start and end pages
                var maxPagesBeforeCurrentPage = (int)Math.Floor(maxPages / 2d);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling(maxPages / 2d) - 1;
                if (currentPage <= maxPagesBeforeCurrentPage)
                {
                    // current page near the start
                    startPage = 1;
                    endPage = maxPages;
                }
                else if (currentPage + maxPagesAfterCurrentPage >= totalPages)
                {
                    // current page near the end
                    startPage = totalPages - maxPages + 1;
                    endPage = totalPages;
                }
                else
                {
                    // current page somewhere in the middle
                    startPage = currentPage - maxPagesBeforeCurrentPage;
                    endPage = currentPage + maxPagesAfterCurrentPage;
                }
            }

            // calculate start
            var startIndex = (currentPage - 1) * pageSize;
            // ... and end item indexes
            var endIndex = Math.Min(startIndex + pageSize - 1, totalItems - 1);

            // create an array of pages that can be looped over
            var pages = Enumerable.Range(startPage, (endPage + 1) - startPage).ToArray();

            // update object instance with all pager properties required by the view
            return new Pagination(
             totalItems,
             currentPage,
             pageSize,
             totalPages,
             startPage,
             endPage,
             startIndex,
             endIndex,
            pages);
        }
    }
}
