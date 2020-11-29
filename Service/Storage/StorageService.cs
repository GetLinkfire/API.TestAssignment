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
			var absolutePath = GetAbsolutePath(directoryPath);

			if (!Directory.Exists(absolutePath))
			{
				return Enumerable.Empty<string>().ToList();
			}

			ValidateDirectory(absolutePath);

			string[] filePaths = Directory.GetFileSystemEntries(absolutePath,
				String.IsNullOrEmpty(startedWith) ? $"*.json" : $"{startedWith}*", SearchOption.AllDirectories);

			return filePaths.ToList();
		}

		public void Delete(string directoryPath)
		{
			var absolutePath = GetAbsolutePath(directoryPath);
			ValidateDirectory(absolutePath);
			
			Directory.Delete(absolutePath, true);
		}

		public void RenameDirectory(string directoryPath, string newName)
		{
			var absolutePath = GetAbsolutePath(directoryPath);
			var newAbsolutePath = GetAbsolutePath(newName);
			Directory.Move(absolutePath, newAbsolutePath);
		}

		private void ValidateDirectory(string absolutePath)
		{
			FileAttributes attr = File.GetAttributes(absolutePath);

			if (!attr.HasFlag(FileAttributes.Directory))
			{
				throw new ArgumentException($"Path {absolutePath} should point to directory");
			}
		}

		private string GetAbsolutePath(string relativePath)
		{
			return Path.Combine(SolutionFolder, _tempFolder, relativePath);
		}
	}
}
