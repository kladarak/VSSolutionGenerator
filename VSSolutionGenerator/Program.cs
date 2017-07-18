using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Xml;

namespace VSSolutionGenerator
{
	class JsonInputFile
	{
		public string path { get; set; }
		public string projectName { get; set; }
	}

	class SourceFiles
	{
		public List<string> mIncludeFiles = new List<string>();
		public List<string> mCompileFiles = new List<string>();
		public List<string> mFilters = new List<string>();

		private string mDirectory;
		private string mRootFiltersName;

		public SourceFiles(string inDirectory)
		{
			mDirectory = inDirectory;
			mRootFiltersName = "Source";
			mFilters.Add(mRootFiltersName);

			AddIncludeFilesWithExt(".h");
			AddIncludeFilesWithExt(".hpp");
			AddIncludeFilesWithExt(".inl");
			AddCompileFilesWithExt(".cpp");
			AddCompileFilesWithExt(".c");

			mIncludeFiles = RemoveDirectoryPath(mIncludeFiles);
			mCompileFiles = RemoveDirectoryPath(mCompileFiles);

			ExtractFilters(mIncludeFiles);
			ExtractFilters(mCompileFiles);
		}

		private void AddIncludeFilesWithExt(string inExtension)
		{
			mIncludeFiles.AddRange(GetFilesWithExt(inExtension));
		}

		private void AddCompileFilesWithExt(string inExtension)
		{
			mCompileFiles.AddRange(GetFilesWithExt(inExtension));
		}

		private string[] GetFilesWithExt(string inExtension)
		{
			return Directory.GetFiles(mDirectory, "*" + inExtension, SearchOption.AllDirectories);
		}

		private List<string> RemoveDirectoryPath(List<string> inFilenames)
		{
			List<string> newList = new List<string>();

			string directory = mDirectory + "/";

			foreach (var filename in inFilenames)
			{
				string normalised = filename.Replace("\\", "/");
				newList.Add(normalised.Replace(directory, ""));
			}

			return newList;
		}

		private void ExtractFilters(List<string> inFilenames)
		{
			foreach (var filename in inFilenames)
			{
				string filter = GetFilter(filename);	
				if (!mFilters.Contains(filter))
				{
					mFilters.Add(filter);
				}
			}
		}

		public string GetFilter(string inFilename)
		{
			int lastIndex = inFilename.LastIndexOf("/");
			if (lastIndex >= 0)
			{
				return mRootFiltersName + "/" + inFilename.Substring(0, lastIndex);	
			}
			else
			{
				return mRootFiltersName;
			}
		}
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

			SourceFiles sourceFiles = new SourceFiles(jsonInput.path);

			ExportFilters(jsonInput.projectName, sourceFiles);
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

		static void ExportFilters(string inTargetFilename, SourceFiles inSourceFiles)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			XmlWriter writer = XmlWriter.Create(inTargetFilename + ".vcxproj.filters", settings);

			writer.WriteStartDocument();
				//writer.WriteStartElement("ElemName");
				//writer.WriteAttributeString("Attr", "AttrVal");
				//writer.WriteElementString("Elem", "ElemVal");
				//writer.WriteEndElement();

				writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
					writer.WriteAttributeString("ToolsVersion", "4.0");
					writer.WriteStartElement("ItemGroup");

						foreach (var filter in inSourceFiles.mFilters)
						{
							writer.WriteStartElement("Filter");
								writer.WriteAttributeString("Include", filter);
								writer.WriteElementString("UniqueIdentifier", System.Guid.NewGuid().ToString());
							writer.WriteEndElement();
						}

					writer.WriteEndElement();
			
					writer.WriteStartElement("ItemGroup");

						foreach (var filename in inSourceFiles.mIncludeFiles)
						{
							writer.WriteStartElement("ClInclude");
								writer.WriteAttributeString("Include", filename);
								writer.WriteElementString("Filter", inSourceFiles.GetFilter(filename));
							writer.WriteEndElement();
						}

					writer.WriteEndElement();
			
					writer.WriteStartElement("ItemGroup");

						foreach (var filename in inSourceFiles.mCompileFiles)
						{
							writer.WriteStartElement("ClCompile");
								writer.WriteAttributeString("Include", filename);
								writer.WriteElementString("Filter", inSourceFiles.GetFilter(filename));
							writer.WriteEndElement();
						}

					writer.WriteEndElement();

				writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Flush();
			writer.Close();
		}
	}
}
