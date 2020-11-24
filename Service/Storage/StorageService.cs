using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Service.Interfaces.Storage;

namespace Service.Storage
{
	public class StorageService : IStorage
	{
		private string _tempFolder = "data";

		private static string SolutionFolder
		{
			get
			{
				var basePath = AppDomain.CurrentDomain.BaseDirectory;
				if (basePath.Contains("bin"))
					return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
				return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.FullName;
			}
		}

		public T Get<T>(string filePath)
		{
			var absolutePath = Path.Combine(SolutionFolder, _tempFolder, filePath);
			var content = File.ReadAllText(absolutePath);
			if (string.IsNullOrEmpty(content))
			{
				throw new Exception($"File {absolutePath} not found.");
			}

			return JsonConvert.DeserializeObject<T>(content);
		}

		public void Save<T>(string filePath, T content)
		{
			var text = JsonConvert.SerializeObject(content);
			var absolutePath = Path.Combine(SolutionFolder, _tempFolder, filePath);
			Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
			File.WriteAllText(absolutePath, text);
		}

		public List<string> GetFileList(string directoryPath, string startedWith = null)
		{
			var absolutePath = Path.Combine(SolutionFolder, _tempFolder, directoryPath);

			if (!Directory.Exists(absolutePath))
			{
				return Enumerable.Empty<string>().ToList();
			}

			FileAttributes attr = File.GetAttributes(absolutePath);

			if (!attr.HasFlag(FileAttributes.Directory))
			{
				throw new ArgumentException($"Path {directoryPath} should point to directory");
			}

			string[] filePaths = Directory.GetFileSystemEntries(absolutePath,
				String.IsNullOrEmpty(startedWith) ? $"*.json" : $"{startedWith}*", SearchOption.AllDirectories);

			return filePaths.ToList();
		}

		public void Delete(string directoryPath)
		{
			var absolutePath = Path.Combine(SolutionFolder, _tempFolder, directoryPath);
			FileAttributes attr = File.GetAttributes(absolutePath);

			if (!attr.HasFlag(FileAttributes.Directory))
			{
				throw new ArgumentException($"Path {directoryPath} should point to directory");
			}
			
			Directory.Delete(absolutePath, true);
		}
	}
}
