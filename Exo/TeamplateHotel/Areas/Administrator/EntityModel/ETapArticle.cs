using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamplateHotel.Areas.Administrator.EntityModel
{
    public class ETapArticle
    {
        public int ID { get; set; }
        [Required]
        [MaxLength]
        public string Title { get; set; }

        [MaxLength]
        public string Alias { get; set; }
        public int? Index { get; set; }

      
        [MaxLength]
        public string MetaTitle { get; set; }

       
        [MaxLength]
        public string MetaDescription { get; set; }
    }
}