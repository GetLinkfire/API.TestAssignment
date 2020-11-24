using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.Interfaces.Storage;
using Service.Storage;

namespace Service.Tests.Storage
{
	[TestFixture]
	[ExcludeFromCodeCoverage]
	public class StorageServiceTest
	{
		private class TestObject
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public int Value { get; set; }
		}

		private static string SolutionFolder => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;

		[OneTimeSetUp]
		public void Init()
		{
			Directory.CreateDirectory(Path.Combine(SolutionFolder, "data", "unittest"));
		}

		[OneTimeTearDown]
		public void Cleanup()
		{
			Directory.Delete(Path.Combine(SolutionFolder, "data", "unittest"), true);
		}

		[Test]
		public void Get()
		{
			var content = Builder<TestObject>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.Build();

			var path = $"unittest/{content.Id}/general.json";
			var absolutePath = Path.Combine(SolutionFolder, "data", path);
			Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
			File.WriteAllText(absolutePath, JsonConvert.SerializeObject(content));

			IStorage storageService = new StorageService();
			var fromFile = storageService.Get<TestObject>(path);

			Assert.AreEqual(content.Id, fromFile.Id);
			Assert.AreEqual(content.Name, fromFile.Name);
			Assert.AreEqual(content.Value, fromFile.Value);
		}

		[Test]
		public void Get_DirectoryNotFoundException()
		{
			var path = $"unittest/notExists/general.json";
			IStorage storageService = new StorageService();
			Assert.Throws<DirectoryNotFoundException>(() => { storageService.Get<TestObject>(path); });
		}

		[Test]
		public void Get_Exception()
		{
			var emptyContentId = Guid.NewGuid();
			var path = $"unittest/{emptyContentId}/general.json";
			var absolutePath = Path.Combine(SolutionFolder, "data", path);
			Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
			File.WriteAllText(absolutePath, "");

			IStorage storageService = new StorageService();
			Assert.Throws<Exception>(() => { storageService.Get<TestObject>(path); });
		}

		[Test]
		public void Save()
		{
			var content = Builder<TestObject>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.Build();
			var path = $"unittest/{content.Id}/general.json";

			IStorage storageService = new StorageService();
			storageService.Save(path, content);

			var absolutePath = Path.Combine(SolutionFolder, "data", path);
			var text = File.ReadAllText(absolutePath);
			var fromFile = JsonConvert.DeserializeObject<TestObject>(text);

			Assert.AreEqual(content.Id, fromFile.Id);
			Assert.AreEqual(content.Name, fromFile.Name);
			Assert.AreEqual(content.Value, fromFile.Value);
		}


		[Test]
		public void GetFileList()
		{
			var unique = Guid.NewGuid();

			var contents = Builder<TestObject>.CreateListOfSize(3)
				.All()
				.Do(x => x.Id =  Guid.NewGuid())
				.Build();

			foreach (var content in contents)
			{
				var path = $"unittest/getFiles/{unique}/{content.Id}/general.json";
				var absolutePath = Path.Combine(SolutionFolder, "data", path);
				Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
				File.WriteAllText(absolutePath, JsonConvert.SerializeObject(content));
			}

			IStorage storageService = new StorageService();
			var paths = storageService.GetFileList($"unittest/getFiles/{unique}");

			Assert.AreEqual(3, paths.Count);
			foreach (var content in contents)
			{
				Assert.IsTrue(paths.Any(path => path.Contains(content.Id.ToString())));
			}
		}

		[Test]
		public void GetFileList_SameCodes()
		{
			var contents = Builder<TestObject>.CreateListOfSize(2)
				.All()
				.Do(x => x.Id = Guid.NewGuid())
				.TheFirst(1)
				.With(x=>x.Name, "code")
				.TheNext(1)
				.With(x => x.Name, "codeup")
				.Build();

			foreach (var content in contents)
			{
				var path = $"unittest/getFiles/{content.Name}/general.json";
				var absolutePath = Path.Combine(SolutionFolder, "data", path);
				Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
				File.WriteAllText(absolutePath, JsonConvert.SerializeObject(content));
			}

			IStorage storageService = new StorageService();
			var paths = storageService.GetFileList("unittest/getFiles", "code");

			Assert.AreEqual(2, paths.Count);
		}

		[Test]
		public void GetFileList_NoDirectory()
		{
			IStorage storageService = new StorageService();
			var paths = storageService.GetFileList("unittest/getFilesTest");

			Assert.AreEqual(0, paths.Count);
			
		}

		[Test]
		public void Delete()
		{
			var content = Builder<TestObject>.CreateNew()
				.With(x => x.Id, Guid.NewGuid())
				.Build();

			var path = $"unittest/{content.Id}/general.json";
			var absolutePath = Path.Combine(SolutionFolder, "data", path);
			var directoryPath = Path.GetDirectoryName(absolutePath);
			Directory.CreateDirectory(directoryPath);
			File.WriteAllText(absolutePath, JsonConvert.SerializeObject(content));

			IStorage storageService = new StorageService();
			storageService.Delete(directoryPath);

			Assert.Throws<DirectoryNotFoundException>(() =>
			{
				Directory.GetFiles(Path.Combine(SolutionFolder, "data", $"unittest/{content.Id}"), "*.json", SearchOption.AllDirectories);
			});
		}
	}


}
