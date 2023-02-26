using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceParameters
{
    public class AuthorsResourceParameters
    {

        // Prevent the client asking for a too large number of pages
        const int maxPageSize = 20;

        public string? MainCategory { get; set; }

        public string? SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {

            get => _pageSize;
            // value = the value the client required
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }


    }
}