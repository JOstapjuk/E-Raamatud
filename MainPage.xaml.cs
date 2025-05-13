using E_Raamatud.Model;
using E_Raamatud.ViewModel;
using SQLite;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace E_Raamatud
{
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _database;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new BookViewModel();  // Set ViewModel

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Books.db");  // Get database path
            _database = new SQLiteAsyncConnection(dbPath);  // Create SQLite connection
        }

        // This method is triggered when a book is selected
        private async void OnBookSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                // Make sure we are working with BookWithGenre (not Raamat directly)
                var selectedBook = e.CurrentSelection[0] as BookWithGenre;

                if (selectedBook != null)
                {
                    // Debugging output
                    Console.WriteLine($"Book Selected: {selectedBook.Pealkiri}");

                    // Fetch the Raamat object from the database using Raamat_ID
                    var raamat = await _database.Table<Raamat>()
                        .Where(b => b.Raamat_ID == selectedBook.Raamat_ID)
                        .FirstOrDefaultAsync();

                    if (raamat != null)
                    {
                        // Create BookWithGenre from Raamat
                        var genre = await _database.Table<Genre>()
                            .Where(g => g.Zanr_ID == raamat.Zanr_ID)
                            .FirstOrDefaultAsync();

                        var bookWithGenre = new BookWithGenre
                        {
                            Raamat_ID = raamat.Raamat_ID,
                            Pealkiri = raamat.Pealkiri,
                            Kirjeldus = raamat.Kirjeldus,
                            Hind = raamat.Hind,
                            Zanr_Nimi = genre?.Nimetus ?? "Tundmatu",
                            Pilt = raamat.Pilt
                        };

                        // Now navigate to the BookDetailPage, passing the BookWithGenre object
                        await Navigation.PushAsync(new BookDetailPage(bookWithGenre));
                    }
                    else
                    {
                        Console.WriteLine("No Raamat found for selected book.");
                    }
                }
                else
                {
                    // Handle case if selectedBook is null
                    Console.WriteLine("Selected book is null.");
                }
            }
        }

    }
}
