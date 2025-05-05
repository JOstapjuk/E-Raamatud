using E_Raamatud.ViewModel;

namespace E_Raamatud;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new BookViewModel();
    }
}