using E_Raamatud.ViewModel;
using Microsoft.Maui.Controls;

namespace E_Raamatud.View
{
    public partial class LibraryPage : ContentPage
    {
        public LibraryPage()
        {
            InitializeComponent();
            BindingContext = new LibraryViewModel();
        }
    }
}
