using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineFertilizerWeb.Models
{
    public class Search
    {
        private int categoryID;
        private string txtSearch;
        private int pageIndex;

        public int CategoryID { get => categoryID; set => categoryID = value; }
        public string TxtSearch { get => txtSearch; set => txtSearch = value; }
        public int PageIndex { get => pageIndex; set => pageIndex = value; }
    }
}