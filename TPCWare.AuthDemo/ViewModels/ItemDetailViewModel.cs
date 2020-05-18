using System;

using TPCWare.AuthDemo.Models;

namespace TPCWare.AuthDemo.ViewModels
{
    public class ItemDetailViewModel : ViewModelBase
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
