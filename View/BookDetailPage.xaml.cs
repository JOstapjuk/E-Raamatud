using E_Raamatud.Model;
using E_Raamatud.ViewModel;

namespace E_Raamatud;

public partial class BookDetailPage : ContentPage
{
    public BookDetailPage(Raamat selectedBook, string zanrNimi, int userId)
    {
        InitializeComponent();
        BindingContext = new BookDetailViewModel(selectedBook, zanrNimi, userId);
    }
}
