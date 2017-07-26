using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSSolutionGenerator
{
	class FileUtils
	{
		public static string NormaliseFilename(string inFilename)
		{
			return inFilename.Replace("/", "\\");
		}

		public static T ReadJsonFile<T>(string inFilename)
		{
			if (!File.Exists(inFilename))
			{
				System.Console.WriteLine("Directory {0} does not exist.", inFilename);
				return default(T);
			}

			string contents = File.ReadAllText(inFilename);

			if (contents.Length == 0)
			{
				System.Console.WriteLine("Failed to read json input file '{0}' (or it was empty)", inFilename);
			}

			try
			{
				return JsonConvert.DeserializeObject<T>(contents);
			}
			catch (Exception e)
			{
				System.Console.Write("Error parsing json file '{0}': {1}", inFilename, e.Message);
				return default(T);
			}
		}

		public static string GetRelativePath(string inFromPath, string inToPath)
		{
			var fromPath = Path.GetFullPath(inFromPath);
			var toPath = Path.GetFullPath(inToPath);

			char[] delimiter = { '\\' };
			var fromPathDirs = fromPath.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
			var toPathDirs = toPath.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

			int skipDirCount = 0;

			for (skipDirCount = 0; skipDirCount < fromPathDirs.Count(); ++skipDirCount)
			{
				if (skipDirCount == toPathDirs.Count())
					break;

				if (fromPathDirs[skipDirCount] != toPathDirs[skipDirCount])
					break;
			}

			string outPath = "";

			for (int i = skipDirCount; i < fromPathDirs.Count(); ++i)
			{
				outPath = Path.Combine(outPath, "..");
			}

			for (int i = skipDirCount; i < toPathDirs.Count(); ++i)
			{
				outPath = Path.Combine(outPath, toPathDirs[i]);
			}

			return outPath;
		}
	}
}
