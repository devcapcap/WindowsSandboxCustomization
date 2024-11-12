using System.Runtime.CompilerServices;
using System.Text;

namespace FileWatcher
{
	public sealed class Program
	{

		static FileStream log = null;
		public static async Task Main(string[] args)
		{
			try
			{
#if DEBUG

				args = new string[] { "c:\\" };
#endif
				if (args.Length == 0)
					throw new Exception("set path value argument");

				string path = args[0];
				if (string.IsNullOrWhiteSpace(path))
					throw new Exception("set path value");

				
				string date = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
				string fileTrace = $"Trace_{Guid.NewGuid().ToString()}_{date}.txt";
				using (log = File.Create(Path.Combine(path, fileTrace), 1024, FileOptions.Asynchronous))
				{
					LOGAsync("Create file trace", $"File trace create at {DateTime.Now} and monitorin path {path}");

					path = args[0];
					using var watcher = new FileSystemWatcher($"{path}");

					watcher.NotifyFilter = NotifyFilters.Attributes
										 | NotifyFilters.CreationTime
										 | NotifyFilters.DirectoryName
										 | NotifyFilters.FileName
										 | NotifyFilters.LastAccess
										 | NotifyFilters.LastWrite
										 | NotifyFilters.Security
										 | NotifyFilters.Size;

					watcher.Changed += async (s, e) => await OnChanged(s, e);
					watcher.Created += async (s, e) => await OnCreated(s, e);
					watcher.Deleted += async (s, e) => await OnDeleted(s, e);
					watcher.Renamed += async (s, e) => await OnRenamed(s, e);
					watcher.Error += OnError;

					watcher.Filter = "*.*";
					watcher.IncludeSubdirectories = true;
					watcher.EnableRaisingEvents = true;

					Console.WriteLine("Press enter to exit.");
					Console.ReadLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine(ex.StackTrace);
			}
		}
		static byte[] ConvertStringToUTF8Bytes(string message)
		{
			return Encoding.UTF8.GetBytes(message + "\r\n");
		}

		static  async Task LOGAsync(string @event, string message)
		{
			byte[] s = ConvertStringToUTF8Bytes($"{@event}->\t{message}");
			await log.WriteAsync(s, 0, s.Length);
		}
		private async static Task OnChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType != WatcherChangeTypes.Changed)
			{
				await LOGAsync("Changed", e.FullPath);
				Console.WriteLine($"Changed: {e.FullPath}");
			}
			
		}

		private async static Task OnCreated(object sender, FileSystemEventArgs e)
		{
			await LOGAsync("Created", e.FullPath);
			Console.WriteLine("Created " + e.FullPath);
		}

		private async static Task OnDeleted(object sender, FileSystemEventArgs e)
		{
			await LOGAsync("Created", e.FullPath);
			Console.WriteLine($"Deleted: {e.FullPath}");
		}

		private async static Task OnRenamed(object sender, RenamedEventArgs e)
		{
			await LOGAsync("Renamed", $"old={e.OldFullPath}, new ={e.FullPath}");
			Console.WriteLine($"Renamed:");
			Console.WriteLine($"    Old: {e.OldFullPath}");
			Console.WriteLine($"    New: {e.FullPath}");
		}

		private  static void OnError(object sender, ErrorEventArgs e)
		{
			PrintException(e.GetException());
		}

		private static void PrintException(Exception? ex)
		{
			if (ex != null)
			{
				Console.WriteLine($"Message: {ex.Message}");
				Console.WriteLine("Stacktrace:");
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine();
				PrintException(ex.InnerException);
			}
		}
	}
}