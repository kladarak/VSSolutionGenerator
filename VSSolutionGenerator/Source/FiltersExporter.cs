using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VSSolutionGenerator
{
	class FiltersExporter
	{
		public static void Export(string inTargetFilename, ProjectSourceFiles inSourceFiles)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			XmlWriter writer = XmlWriter.Create(inTargetFilename + ".vcxproj.filters", settings);

			writer.WriteStartDocument();

				writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
					writer.WriteAttributeString("ToolsVersion", "4.0");
					writer.WriteStartElement("ItemGroup");

					foreach (var filter in inSourceFiles.mFilters)
					{
						var guid = System.Guid.NewGuid().ToString();
							
						writer.WriteStartElement("Filter");
							writer.WriteAttributeString("Include", filter);
							writer.WriteElementString("UniqueIdentifier", "{" + guid + "}");
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
