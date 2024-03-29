﻿using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TPCWare.AuthDemo.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
        }

        public ICommand OpenWebCommand { get; }
    }
}