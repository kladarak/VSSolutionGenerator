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
	}
}
