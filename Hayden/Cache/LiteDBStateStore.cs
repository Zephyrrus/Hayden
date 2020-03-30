using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hayden.Contract;
using LiteDB;

namespace Hayden.Cache
{
	/// <summary>
	/// A state storage implementation using LiteDb.
	/// </summary>
	public class LiteDbStateStore : IStateStore
	{
		protected string FilePath { get; set; }

		public LiteDatabase Database { get; private set; }

		private LiteCollection<QueuedImageDownload> QueuedImageDownloads { get; set; }

		static LiteDbStateStore()
		{
			BsonMapper.Global.Entity<QueuedImageDownload>()
					  .Id(x => x.DownloadPath);
		}

		public LiteDbStateStore(string filepath)
		{
			FilePath = filepath;

			Database = new LiteDatabase(FilePath);

			QueuedImageDownloads = Database.GetCollection<QueuedImageDownload>("QueuedImageDownloads");
		}

		/// <inheritdoc/>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task WriteDownloadQueue(IList<QueuedImageDownload> imageDownloads)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			List<QueuedImageDownload> downloads = QueuedImageDownloads.FindAll().ToList();

			QueuedImageDownloads.Upsert(imageDownloads.Except(downloads));

			foreach (var removedItem in downloads.Where(x => imageDownloads.All(y => !Equals(x, y))))
				QueuedImageDownloads.Delete(removedItem.DownloadPath);
		}

		/// <inheritdoc/>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<IList<QueuedImageDownload>> GetDownloadQueue()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			return QueuedImageDownloads.FindAll().ToArray();
		}
	}
}