using gbajax.Models;
using gbajax.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace gbajax.Views
{
    public class GuestbooksViewModel
    {
        [DisplayName("搜尋：")]
        public string Search { get; set; }
        public List<Guestbooks> DataList { get; set; }
        public ForPaging Paging { get; set; }
    }
}