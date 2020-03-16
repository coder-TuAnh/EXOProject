using System.Collections.Generic;

namespace ProjectLibrary.Config
{
    public class SystemMenuType
    {
        public const int Home = 1;
        public const int Article = 2;     
        public const int Tour = 6;
        public const int Activities = 3;
        public const int Contact = 7;
        public const int OutSite = 17;
        public const int Aboutus = 4;



        public static Dictionary<int, string> CategoryType = new Dictionary<int, string>()
                                                                 {
                                                                     {Home, "Home page"},
                                                                     {Article, "Article"},
                                                                     {Tour, "Tour"},
                                                                     {Contact, "Contact"},
                                                                     {OutSite, "Out Site"},
                                                                     {Activities, "Activities"},
                                                                     {Aboutus, "Aboutus"},
                                                                   
                                                                 };
    }
}