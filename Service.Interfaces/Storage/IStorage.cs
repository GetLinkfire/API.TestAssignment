using System.Collections.Generic;

namespace Service.Interfaces.Storage
{
	public interface IStorage
	{
		T Get<T>(string filePath);

		void Save<T>(string filePath, T content);

		List<string> GetFileList(string directoryPath, string startedWith = null);

		void Delete(string directoryPath);
	}
}
