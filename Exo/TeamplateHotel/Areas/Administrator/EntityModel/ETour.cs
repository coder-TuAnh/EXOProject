using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ProjectLibrary.Database;
using System;

namespace TeamplateHotel.Areas.Administrator.EntityModel
{
    public class ETour
    {
        public int ID { get; set; }

        [DisplayName("Menu")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Menu")]
        public int MenuID { get; set; }

        [DisplayName("ActivitisID")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Activities")]
        public int ActivitisID { get; set; }

        [DisplayName("Title")]
        [MaxLength(250)]
        [Required]
        public string Title { get; set; }

        [DisplayName("Alias")]
        public string Alias { get; set; }

        [MaxLength]
        [Required]
        public string Image { get; set; }

        public string Description { get; set; }

        public int? Index { get; set; }

        [DisplayName("Meta Title")]
        [MaxLength(250)]
        public string MetaTitle { get; set; }

        [MaxLength(250)]
        public string MetaDescription { get; set; }

        public bool Status { get; set; }

        public bool Hot { get; set; }

        public bool Deal { get; set; }

        public bool Like { get; set; }

        public bool ViewAll { get; set; }

        public decimal PriceSale { get; set; }

        public decimal Price { get; set; }

        public string Location { get; set; }
        public string Time { get; set; }
        public bool TourOther { get; set; }
        public string Content { get; set; }
        public string TourIncluded { get; set; }
        public string TourExcluded { get; set; }

        public string[] Theme { get; set; }

        public List<EGalleryITem> EGalleryITems { get; set; }

        public List<TabTour> TabTours { get; set; }

        public List<TabHotel> TabHotels { get; set; }

        public List<ThemesMenu> ThemesMenus { get; set; }
    }
}