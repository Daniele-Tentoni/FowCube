namespace FowCube.Models.HomeMenuItems
{
    public enum MenuItemType
    {
        Browse,
        Settings,
        About,
        Login,
        SignOut
    }
    
    public class HomeMenuItem
    {
        public int Id { get; set; }
        public MenuItemType MenuType { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Arg { get; set; }
        public bool IsInCloud { get; set; }
    }
}
