using E_Raamatud.ViewModel;

namespace E_Raamatud;

public partial class UserProfilePage : ContentPage
{
    public UserProfilePage()
    {
        InitializeComponent();
        BindingContext = new UserProfileViewModel();
    }

    private async void OnPurchaseHistoryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PurchaseHistoryPage());
    }
}
