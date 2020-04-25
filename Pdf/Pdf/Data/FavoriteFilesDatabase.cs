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
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public FavoriteFilesDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
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

        public Task<FileModel> GetItemAsync(int id)
        {
            return Database.Table<FileModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(FileModel item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(FileModel item)
        {
            return Database.DeleteAsync(item);
        }
    }
}

