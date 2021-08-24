using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Utility
{



    /* 
    * Class description: Handles the table pagination, ie the numbered pages for html tables
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

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
