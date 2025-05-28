using E_Raamatud.Model;
using SQLite;
using System.Collections.ObjectModel;

namespace E_Raamatud.View;

public partial class AvaldajaReportPage : ContentPage
{
    private SQLiteAsyncConnection _db;

    public class BookReport
    {
        public string Pealkiri { get; set; }
        public int KokkuM��dud { get; set; }
        public decimal KokkuTulu { get; set; }

        public string M��dudTekst => $"M��dud: {KokkuM��dud} tk";
        public string TuluTekst => $"Tulu: {KokkuTulu} �";
    }

    public AvaldajaReportPage(int avaldajaId)
    {
        InitializeComponent();
        _db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "Books.db"));
        LoadReport(avaldajaId);
    }

    private async void LoadReport(int avaldajaId)
    {
        await _db.CreateTableAsync<PurchaseBasket>();
        await _db.CreateTableAsync<Raamat>();

        var books = await _db.Table<Raamat>()
            .Where(b => b.Avaldaja_ID == avaldajaId)
            .ToListAsync();

        var purchases = await _db.Table<PurchaseBasket>()
            .Where(p => p.Status == "Purchased")
            .ToListAsync();

        var report = books.Select(book =>
        {
            var bookPurchases = purchases.Where(p => p.Raamat_ID == book.Raamat_ID);
            int totalSold = bookPurchases.Sum(p => p.Kogus);
            decimal totalRevenue = bookPurchases.Sum(p => p.L�ppu_hind);

            return new BookReport
            {
                Pealkiri = book.Pealkiri,
                KokkuM��dud = totalSold,
                KokkuTulu = totalRevenue
            };
        })
        .Where(r => r.KokkuM��dud > 0)
        .ToList();

        ReportListView.ItemsSource = new ObservableCollection<BookReport>(report);
    }
}
