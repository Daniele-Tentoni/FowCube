namespace FowCube.ViewModels
{
    using FowCube.Models;

    public class ItemDetailViewModel : BaseViewModel
    {
        public Card Item { get; set; }
        public ItemDetailViewModel(Card item = null)
        {
            this.Title = item?.Name;
            this.Item = item;
        }
    }
}
