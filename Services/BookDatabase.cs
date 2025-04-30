using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Raamatud.Model;

namespace E_Raamatud.Services
{
    class BookDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public BookDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Raamat>().Wait();
        }

        public Task<List<Raamat>> GetBooksAsync()
        {
            return _database.Table<Raamat>().ToListAsync();
        }

        public Task<Raamat> GetBookAsync(int id)
        {
            return _database.Table<Raamat>()
                            .Where(b => b.Raamat_ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveBookAsync(Raamat book)
        {
            if (book.Raamat_ID != 0)
                return _database.UpdateAsync(book);
            else
                return _database.InsertAsync(book);
        }

        public Task<int> DeleteBookAsync(Raamat book)
        {
            return _database.DeleteAsync(book);
        }
    }
}
