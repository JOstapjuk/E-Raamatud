using E_Raamatud.Model;

namespace E_Raamatud;

public partial class BookDetailPage : ContentPage
{
    public BookDetailPage(BookWithGenre selectedBook)
    {
        InitializeComponent(); // Ensure this method is generated in the corresponding XAML file
        BindingContext = selectedBook;  // Bind the selected book's data to the page
    }
}
