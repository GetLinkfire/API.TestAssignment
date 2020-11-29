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
				string basePath = AppDomain.CurrentDomain.BaseDirectory;
				
				if (basePath.Contains("bin"))
                {
					return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
				}
				
				return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.FullName;
			}
		}

		public T Get<T>(string filePath)
		{
			string absolutePath = AbsolutePath(filePath);
			
			string content = File.ReadAllText(absolutePath);
			
			if (string.IsNullOrEmpty(content))
			{
				throw new Exception($"File {absolutePath} not found.");
			}

			return JsonConvert.DeserializeObject<T>(content);
		}

		public void Save<T>(string filePath, T content)
		{
			string text = JsonConvert.SerializeObject(content);
			
			string absolutePath = AbsolutePath(filePath);
			
			Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
			
			File.WriteAllText(absolutePath, text);
		}

		public List<string> GetFileList(string directoryPath, string startedWith = null)
		{
			string absolutePath = AbsolutePath(directoryPath);

			if (!Directory.Exists(absolutePath))
			{
				return Enumerable.Empty<string>().ToList();
			}

			ValidateDirectory(directoryPath);

			string[] filePaths = Directory.GetFileSystemEntries(
				absolutePath,
				string.IsNullOrEmpty(startedWith) ? $"*.json" : $"{startedWith}*",
				SearchOption.AllDirectories
			);

			return filePaths.ToList();
		}

		public void Delete(string directoryPath)
		{
			string absolutePath = ValidateDirectory(directoryPath);

			Directory.Delete(absolutePath, true);
		}

		public void Move(string originPath, string destinationPath)
		{
			string absoluteOriginPath = Path.Combine(SolutionFolder, _tempFolder, originPath);
			string absoluteDestinationPath = Path.Combine(SolutionFolder, _tempFolder, destinationPath);

			Directory.Move(absoluteOriginPath, absoluteDestinationPath);
		}

		private	string AbsolutePath(string path)
        {
			return Path.Combine(SolutionFolder, _tempFolder, path);
		}

		private string ValidateDirectory(string path)
        {
			string absolutePath = AbsolutePath(path);

			FileAttributes attr = File.GetAttributes(absolutePath);

			if (!attr.HasFlag(FileAttributes.Directory))
			{
				throw new ArgumentException($"Path {path} should point to directory");
			}

			return absolutePath;
		}
	}
}