namespace FowCube.Models.HomeMenuItems
{
    using System.Collections.Generic;

    public class HomeMenuItemsGroup : List<HomeMenuItem>
    {
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public HomeMenuItemsGroup(string title, string subtitle)
        {
            this.Title = title;
            this.SubTitle = subtitle;
        }
    }
}
