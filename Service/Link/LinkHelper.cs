using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Service.Interfaces.Storage;

namespace Service.Link
{
	public class LinkHelper
	{
		public static Func<string, string, string> ShortLinkTemplate = (domain, code) => $"{domain}/{code}";
		public static Func<string, string> LinkGeneralFilenameTemplate = (shortLink) => $"{shortLink}/general.json";

		/// <summary>
		/// Generate an unique short code for link
		/// </summary>
		/// <param name="storageService"></param>
		/// <param name="domainName"></param>
		/// <returns></returns>
		public static string GetUniqueLinkShortCode(IStorage storageService, string domainName)
		{
			var randomString = Guid.NewGuid().ToString();
			
			var code = randomString.Substring(0, randomString.IndexOf('-'));
			while (!IsValidLinkCode(storageService, domainName, code))
			{
				randomString = Guid.NewGuid().ToString();
				code = randomString.Substring(0, randomString.IndexOf('-'));
			}
			return code;
		}

		public static bool IsValidLinkCode(IStorage storageService, string domainName, string code)
		{
			string shorterCode = code.Substring(0, code.Length - 2);

			List<string> keysInPath = storageService.GetFileList(domainName, shorterCode);

			foreach (string key in keysInPath)
			{
				string existingCode = key.Split(Path.DirectorySeparatorChar).Last();
				
				// check code from key against short and shorter code
				if (existingCode.Equals(code, StringComparison.InvariantCultureIgnoreCase) || existingCode.Equals(shorterCode, StringComparison.InvariantCultureIgnoreCase))
				{
					return false;
				}

				// check shorter code from key against the requested short code
				if (existingCode.Substring(0, existingCode.Length - 2).Equals(code, StringComparison.InvariantCultureIgnoreCase))
				{
					return false;
				}
			}

			return true;
		}
	}
}
