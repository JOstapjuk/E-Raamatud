using E_Raamatud.Model;
using SQLite;
using System.Collections.ObjectModel;

namespace E_Raamatud;

public partial class PurchaseHistoryPage : ContentPage
{
    private readonly SQLiteAsyncConnection _db;

    public PurchaseHistoryPage()
    {
        InitializeComponent();
        _db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "Books.db"));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPurchaseHistoryAsync();
    }

    private async Task LoadPurchaseHistoryAsync()
    {
        if (SessionService.CurrentUser == null)
        {
            await DisplayAlert("Viga", "Kasutaja pole sisse logitud.", "OK");
            return;
        }

        var basketItems = await _db.Table<PurchaseBasket>()
                           .Where(pb => pb.Kasutaja_ID == SessionService.CurrentUser.Id)
                           .ToListAsync();

        System.Diagnostics.Debug.WriteLine($"Basket items found: {basketItems.Count}");

        var displayList = new List<PurchaseDisplay>();

        foreach (var basketItem in basketItems)
        {
            var book = await _db.Table<Raamat>()
                                .Where(r => r.Raamat_ID == basketItem.Raamat_ID)
                                .FirstOrDefaultAsync();

            System.Diagnostics.Debug.WriteLine($"BasketItem Raamat_ID={basketItem.Raamat_ID}, Book found: {book?.Pealkiri ?? "null"}");

            if (book != null)
            {
                displayList.Add(new PurchaseDisplay
                {
                    BookTitle = book.Pealkiri,
                    Quantity = basketItem.Kogus,
                    TotalPrice = basketItem.Lõppu_hind
                });
            }
            else
            {
                displayList.Add(new PurchaseDisplay
                {
                    BookTitle = $"Raamat ID {basketItem.Raamat_ID} puudub",
                    Quantity = basketItem.Kogus,
                    TotalPrice = basketItem.Lõppu_hind
                });
            }
        }

        PurchaseList.ItemsSource = displayList;
    }

    public class PurchaseDisplay
    {
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
