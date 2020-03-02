namespace FowCube.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Xamarin.Forms;
    using FowCube.Authentication;
    using Xamarin.Essentials;

    public class BaseViewModel : INotifyPropertyChanged
    {
        protected IAuth AuthInfo => DependencyService.Get<IAuth>();
        // protected CardStore CardsStore => DependencyService.Get<CardStore>() ?? new CardStore();
        // protected CollectionStore CollectionsStore => DependencyService.Get<CollectionStore>() ?? new CollectionStore();

        public string DisplayName => SecureStorage.GetAsync("display_name").Result;
        public string UserId => SecureStorage.GetAsync("user_id").Result;

        bool isBusy = false;
        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.SetProperty(ref this.isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
