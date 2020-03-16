using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamplateHotel.Areas.Administrator.ModelShow
{
    public class ShowTab
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int? Index { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
    }
}