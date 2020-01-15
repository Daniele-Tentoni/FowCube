namespace FowCube.Models
{
    public enum MenuItemType
    {
        Browse,
        Login,
        About
    }
    public class HomeMenuItem
    {
        public int Id { get; set; }
        public MenuItemType MenuType { get; set; }
        public string Title { get; set; }
        public string Arg { get; set; }
    }
}
