using E_Raamatud.Model;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace E_Raamatud.ViewModel
{
    public class BookViewModel : INotifyPropertyChanged
    {
        private SQLiteAsyncConnection _database;
        private Genre _selectedGenre;
        private string _searchText;
        public ObservableCollection<Genre> Genres { get; set; }
        public ObservableCollection<BookWithGenre> Books { get; set; }

        public ICommand SelectGenreCommand { get; }
        public ICommand SearchCommand { get; }

        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                    FilterBooks();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }

        public BookViewModel()
        {
            Genres = new ObservableCollection<Genre>();
            Books = new ObservableCollection<BookWithGenre>();
            SelectGenreCommand = new Command<Genre>(genre =>
            {
                SelectedGenre = genre;
            });

            SearchCommand = new Command<string>(searchTerm =>
            {
                SearchText = searchTerm;
                SearchBooks(searchTerm);
            });

            InitAsync();
        }

        private async Task InitAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Books.db");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Raamat>();
            await _database.CreateTableAsync<Genre>();

            Genres.Add(new Genre { Zanr_ID = 0, Nimetus = "Kõik raamatud", Kirjeldus = "Kuva kõik raamatud" });

            if (await _database.Table<Genre>().CountAsync() == 0)
            {
                await _database.InsertAllAsync(new[]
                {
                new Genre { Zanr_ID = 1, Nimetus = "romaan", Kirjeldus = "Romaanid ja ilukirjandus" },
                new Genre { Zanr_ID = 2, Nimetus = "fantaasia", Kirjeldus = "Fantaasiakirjandus" },
                new Genre { Zanr_ID = 3, Nimetus = "klassika", Kirjeldus = "Klassikalised teosed" },
                new Genre { Zanr_ID = 4, Nimetus = "ajalugu", Kirjeldus = "Ajaloolised raamatud" },
                new Genre { Zanr_ID = 5, Nimetus = "ulme", Kirjeldus = "Ulmekirjandus" },
                new Genre { Zanr_ID = 6, Nimetus = "noorteraamat", Kirjeldus = "Noortekirjandus" },
                new Genre { Zanr_ID = 7, Nimetus = "seiklus", Kirjeldus = "Seikluslood" }
            });
            }

            if (await _database.Table<Raamat>().CountAsync() == 0)
            {
                await _database.InsertAllAsync(new[]
                {
                    new Raamat
                    {
                        Raamat_ID = 1,
                        Pealkiri = "Sipsik",
                        Kirjeldus = "Laste raamat pehme mänguasjast nimega Sipsik.",
                        Hind = 5.99m,
                        Zanr_ID = 3,
                        Pilt = "sipsik.jpg"
                    },
                    new Raamat
                    {
                        Raamat_ID = 2,
                        Pealkiri = "The Hunger Games",
                        Kirjeldus = "Noorteraamat düstoopilises ühiskonnas peetavatest ellujäämismängudest.",
                        Hind = 9.99m,
                        Zanr_ID = 6,
                        Pilt = "hungergames.jpg"
                    },
                    new Raamat
                    {
                        Raamat_ID = 3,
                        Pealkiri = "Dune",
                        Kirjeldus = "Ulmeromaan kõrbemaailmast ja poliitilisest võitlusest.",
                        Hind = 12.99m,
                        Zanr_ID = 5,
                        Pilt = "dune.jpg"
                    }
                });
            }

            var genreList = await _database.Table<Genre>().ToListAsync();
            foreach (var genre in genreList)
            {
                Genres.Add(genre);
            }

            await LoadAllBooks();
        }

        private async Task LoadAllBooks()
        {
            var genres = await _database.Table<Genre>().ToListAsync();
            var books = await _database.Table<Raamat>().ToListAsync();

            Books.Clear();
            foreach (var b in books)
            {
                var genre = genres.FirstOrDefault(g => g.Zanr_ID == b.Zanr_ID);
                Books.Add(new BookWithGenre
                {
                    Raamat_ID = b.Raamat_ID,
                    Pealkiri = b.Pealkiri,
                    Kirjeldus = b.Kirjeldus,
                    Hind = b.Hind,
                    Zanr_Nimi = genre?.Nimetus ?? "Tundmatu",
                    Pilt = b.Pilt
                });
            }
        }

        private async void FilterBooks()
        {
            if (SelectedGenre.Zanr_ID == 0)
            {
                await LoadAllBooks();
                return;
            }

            var books = await _database.Table<Raamat>()
                .Where(b => b.Zanr_ID == SelectedGenre.Zanr_ID)
                .ToListAsync();

            Books.Clear();
            foreach (var b in books)
            {
                Books.Add(new BookWithGenre
                {
                    Raamat_ID = b.Raamat_ID,
                    Pealkiri = b.Pealkiri,
                    Kirjeldus = b.Kirjeldus,
                    Hind = b.Hind,
                    Zanr_Nimi = SelectedGenre.Nimetus,
                    Pilt = b.Pilt
                });
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                SearchBooks(SearchText);
            }
        }

        private async void SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                FilterBooks();
                return;
            }

            searchTerm = searchTerm.ToLower();

            var allBooks = new ObservableCollection<BookWithGenre>();
            var genres = await _database.Table<Genre>().ToListAsync();

            var booksQuery = _database.Table<Raamat>();

            if (SelectedGenre != null && SelectedGenre.Zanr_ID != 0)
            {
                booksQuery = booksQuery.Where(b => b.Zanr_ID == SelectedGenre.Zanr_ID);
            }

            var books = await booksQuery.ToListAsync();

            foreach (var b in books)
            {
                var genre = genres.FirstOrDefault(g => g.Zanr_ID == b.Zanr_ID);
                allBooks.Add(new BookWithGenre
                {
                    Raamat_ID = b.Raamat_ID,
                    Pealkiri = b.Pealkiri,
                    Kirjeldus = b.Kirjeldus,
                    Hind = b.Hind,
                    Zanr_Nimi = genre?.Nimetus ?? "Tundmatu",
                    Pilt = b.Pilt
                });
            }

            var filteredBooks = allBooks.Where(b =>
                b.Pealkiri.ToLower().Contains(searchTerm) ||
                b.Kirjeldus.ToLower().Contains(searchTerm) ||
                b.Zanr_Nimi.ToLower().Contains(searchTerm)).ToList();

            Books.Clear();
            foreach (var book in filteredBooks)
            {
                Books.Add(book);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}