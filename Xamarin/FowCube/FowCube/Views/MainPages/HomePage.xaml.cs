namespace FowCube.Views.MainPages
{
    using FowCube.Models.Cards;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<Models.Collection.Collection> Collections = new ObservableCollection<Models.Collection.Collection>(App.Database.GetCollectionsByUserAsync("11").Result);
        public ObservableCollection<Card> Cards = new ObservableCollection<Card>(App.Database.GetCollectionByIdAsync("1").Result.CardsIn);

        public HomePage()
        {
            this.Title = "Homepage";
            this.InitializeComponent();
        }
    }
}