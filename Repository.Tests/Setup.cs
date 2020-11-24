using NUnit.Framework;
using Repository.Entities;

namespace Repository.Tests
{
	[SetUpFixture]
	public class Setup
	{
		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{
			using (var context = new Context())
			{
				context.Database.Delete();
			}
		}
	}
}
