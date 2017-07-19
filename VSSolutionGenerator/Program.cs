using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace VSSolutionGenerator
{
	class JsonInputFile
	{
		public string path { get; set; }
		public string projectName { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				System.Console.WriteLine("Please pass json input file.");
				return;
			}

			JsonInputFile jsonInput = ReadJSONInputFile(args[0]);

			if (!Directory.Exists(jsonInput.path))
			{
				System.Console.WriteLine("Directory {0} does not exist.", jsonInput.path);
				return;
			}

			// TODO: Also need to validate that the name is valid to be used as a filename.
			if (jsonInput.projectName.Length == 0)
			{
				System.Console.WriteLine("No project name specified.");
				return;
			}

			ProjectSourceFiles sourceFiles = new ProjectSourceFiles(jsonInput.path);

			FiltersExporter.Export(jsonInput.projectName, sourceFiles);
		}

		static JsonInputFile ReadJSONInputFile(string inFilename)
		{
			string contents = File.ReadAllText(inFilename);

			if (contents.Length == 0)
			{
				System.Console.WriteLine("Failed to read json input file \"" + inFilename + "\" (or it was empty)");
				return new JsonInputFile();
			}

			try
			{
				return JsonConvert.DeserializeObject<JsonInputFile>(contents);
			}
			catch
			{
				System.Console.Write("Error parsing json file " + inFilename);
				return new JsonInputFile();
			}
		}
	}
}
