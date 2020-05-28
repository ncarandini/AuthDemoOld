using System;
using System.Collections.Generic;

using Xamarin.Forms;
using TPCWare.AuthDemo.ViewModels;

namespace TPCWare.AuthDemo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        AppShellViewModel appShellViewModel = new AppShellViewModel();
        public AppShell()
        {
            InitializeComponent();
            this.BindingContext = appShellViewModel;


            MessagingCenter.Subscribe<AppShellViewModel, string>(this, "changeTitle", (sender, arg) =>
             {

                 this.tabLogin.Title = arg;
             });

        }


    }
}
