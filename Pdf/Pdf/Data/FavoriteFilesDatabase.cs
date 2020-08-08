using Pdf.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdf.Data
{
    public class FavoriteFilesDatabase
    {
        private static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = 
            new Lazy<SQLiteAsyncConnection>(() 
                => new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags));

        private static SQLiteAsyncConnection Database => lazyInitializer.Value;
        private static bool initialized;

        public FavoriteFilesDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        private async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(FileModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(FileModel)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public Task<List<FileModel>> GetItemsAsync()
        {
            return Database.Table<FileModel>().ToListAsync();
        }

        public void DeleteAllItemAsync()
        {
            Database.DeleteAllAsync<FileModel>();
        }

        public Task<FileModel> GetItemAsync(int id)
        {
            return Database.Table<FileModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(FileModel item)
        {
            return item.Id != 0 
                ? Database.UpdateAsync(item) 
                    : Database.InsertAsync(item);
        }

        public Task<int> DeleteItemAsync(FileModel item)
        {
            return Database.DeleteAsync(item);
        }
    }
}

