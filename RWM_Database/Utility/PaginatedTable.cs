using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Utility
{
    public class PaginatedTable
    {

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public PaginatedTable(int pageSize, int tableSize)
        {
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling(decimal.Divide(tableSize, PageSize));
        }

    }
}
