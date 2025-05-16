using E_Raamatud.ViewModel;

namespace E_Raamatud;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is UserViewModel vm)
        {
            var user = await vm.LoginAsync();

            if (user != null)
            {
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                await DisplayAlert("Login", vm.LoginStatus, "OK");
            }
        }
    }

    private async void GoToRegister_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}