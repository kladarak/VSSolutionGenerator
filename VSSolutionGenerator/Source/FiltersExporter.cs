using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace VSSolutionGenerator
{
	class FiltersExporter
	{
		public static void Export(string inTargetDirectory, VCXProjectData inProjectData)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			string filtersFilename = Path.GetFullPath(Path.Combine(inTargetDirectory, inProjectData.mProjectName + ".vcxproj.filters"));
			XmlWriter writer = XmlWriter.Create(filtersFilename, settings);

			var sourceFiles = inProjectData.mSourceFiles;
			var sourceFilePrefix = FileUtils.GetRelativePath(inTargetDirectory, sourceFiles.mSearchDirectory);

			writer.WriteStartDocument();

				writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
					writer.WriteAttributeString("ToolsVersion", "4.0");
					writer.WriteStartElement("ItemGroup");

					foreach (var filter in sourceFiles.mFilters)
					{
						var guid = System.Guid.NewGuid().ToString();
							
						writer.WriteStartElement("Filter");
							writer.WriteAttributeString("Include", filter);
							writer.WriteElementString("UniqueIdentifier", "{" + guid + "}");
						writer.WriteEndElement();
					}

					writer.WriteEndElement();
			
					writer.WriteStartElement("ItemGroup");

					foreach (var filename in sourceFiles.mIncludeFiles)
					{
						writer.WriteStartElement("ClInclude");
							writer.WriteAttributeString("Include", Path.Combine(sourceFilePrefix, filename));
							writer.WriteElementString("Filter", sourceFiles.GetFilter(filename));
						writer.WriteEndElement();
					}

					writer.WriteEndElement();
			
					writer.WriteStartElement("ItemGroup");

					foreach (var filename in sourceFiles.mCompileFiles)
					{
						writer.WriteStartElement("ClCompile");
							writer.WriteAttributeString("Include", Path.Combine(sourceFilePrefix, filename));
							writer.WriteElementString("Filter", sourceFiles.GetFilter(filename));
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
