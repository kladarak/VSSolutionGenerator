using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VSSolutionGenerator
{
	class VCXProjectExporter
	{
		public static void Export(string inTargetFilename, VCXProjectData inData)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			XmlWriter writer = XmlWriter.Create(inTargetFilename + ".vcxproj", settings);

			writer.WriteStartDocument();
				writer.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
					writer.WriteAttributeString("ToolsVersion", "14.0");
					writer.WriteAttributeString("DefaultTargets", "Build");

					writer.WriteStartElement("ItemGroup");
					writer.WriteAttributeString("Label", "ProjectConfigurations");

					foreach (var projConfig in inData.mConfigs)
					{
						writer.WriteStartElement("ProjectConfiguration");
						writer.WriteAttributeString("Include", projConfig.GetConfigPlatformName());
							writer.WriteElementString("Configuration", projConfig.mConfigName);
							writer.WriteElementString("Platform", projConfig.mPlatform.ToString());
						writer.WriteEndElement();
					}

					writer.WriteEndElement();
			
					var projectGuid = System.Guid.NewGuid().ToString();

					writer.WriteStartElement("PropertyGroup");
						writer.WriteAttributeString("Label", "Globals");
						writer.WriteElementString("ProjectGuid", "{" + projectGuid + "}");
						writer.WriteElementString("Keyword", "Win32Proj");
						writer.WriteElementString("RootNamespace", inData.mProjectName);
					writer.WriteEndElement();

					writer.WriteStartElement("Import");
						writer.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.Default.props");
					writer.WriteEndElement();
			
					foreach (var projConfig in inData.mConfigs)
					{
						writer.WriteStartElement("PropertyGroup");
							writer.WriteAttributeString("Condition", projConfig.GetCondition());
							writer.WriteAttributeString("Label", "Configuration");

							writer.WriteElementString("ConfigurationType", projConfig.mConfigType.ToString());
							writer.WriteElementString("UseDebugLibraries", projConfig.GetUseDebugLibraries());
							writer.WriteElementString("PlatformToolset", "v140");
							writer.WriteElementString("WholeProgramOptimisation", projConfig.GetUseWholeProgramOptimisation());
							writer.WriteElementString("CharacterSet", "Unicode");
						writer.WriteEndElement();
					}
					
					writer.WriteStartElement("Import");
						writer.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.props");
					writer.WriteEndElement();
			
					writer.WriteStartElement("ImportGroup");
						writer.WriteAttributeString("Label", "ExtensionSettings");
					writer.WriteEndElement();
			
					writer.WriteStartElement("ImportGroup");
						writer.WriteAttributeString("Label", "Shared");
					writer.WriteEndElement();
			
					foreach (var projConfig in inData.mConfigs)
					{
						writer.WriteStartElement("ImportGroup");
							writer.WriteAttributeString("Label", "PropertySheets");
							writer.WriteAttributeString("Condition", projConfig.GetCondition());
				
							writer.WriteStartElement("Import");
								writer.WriteAttributeString("Project", "$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props");
								writer.WriteAttributeString("Condition", "exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')");
								writer.WriteAttributeString("Label", "LocalAppDataPlatform");
							writer.WriteEndElement();

						writer.WriteEndElement();
					}
					
					writer.WriteStartElement("PropertyGroup");
						writer.WriteAttributeString("Label", "UserMacros");
					writer.WriteEndElement();
			
					foreach (var projConfig in inData.mConfigs)
					{
						writer.WriteStartElement("PropertyGroup");
							writer.WriteAttributeString("Condition", projConfig.GetCondition());
							writer.WriteElementString("IncludePath", projConfig.GetIncludePaths());
						writer.WriteEndElement();
					}
					
					foreach (var projConfig in inData.mConfigs)
					{
						writer.WriteStartElement("ItemDefinitionGroup");
							writer.WriteAttributeString("Condition", projConfig.GetCondition());
							writer.WriteStartElement("ClCompile");
								writer.WriteElementString("PrecompiledHeader", projConfig.GetUsePrecompiledHeader());
								writer.WriteElementString("WarningLevel", projConfig.GetWarningLevel());
								writer.WriteElementString("Optimization", projConfig.GetOptimization());
								writer.WriteElementString("FunctionLevelLinking", projConfig.GetFunctionLevelLinking());
								writer.WriteElementString("InstrinsicFunctions", projConfig.GetInstrinsicFunctions());
								writer.WriteElementString("PreprocessorDefinitions", projConfig.GetPreprocessorDefinitions());
								writer.WriteElementString("TreatWarningAsError", "true");
							writer.WriteEndElement();
							writer.WriteStartElement("Link");
								writer.WriteElementString("SubSystem", "Windows");
								writer.WriteElementString("EnableCOMDATFolding", projConfig.GetEnableCOMDATFolding());
								writer.WriteElementString("OptimizeReferences", projConfig.GetOptimizeReferences());
								writer.WriteElementString("AdditionalDependencies", projConfig.GetAdditionalDependencies());
							writer.WriteEndElement();
						writer.WriteEndElement();
					}

					writer.WriteStartElement("ItemGroup");

					foreach (var includeFile in inData.mSourceFiles.mIncludeFiles)
					{
						writer.WriteStartElement("ClInclude");
							writer.WriteAttributeString("Include", includeFile);
						writer.WriteEndElement();
					}

					writer.WriteEndElement();

					writer.WriteStartElement("ItemGroup");
			
					foreach (var compileFile in inData.mSourceFiles.mCompileFiles)
					{
						writer.WriteStartElement("ClCompile");
							writer.WriteAttributeString("Include", compileFile);

							foreach (var projConfig in inData.mConfigs)
							{
								if (projConfig.mPrecompiledHeaderName.Equals(compileFile))
								{
									writer.WriteStartElement("PrecompiledHeader");
										writer.WriteAttributeString("Condition", projConfig.GetCondition());
										writer.WriteString("Create");
									writer.WriteEndElement();
								}
							}

						writer.WriteEndElement();
					}

					writer.WriteEndElement();
					
					writer.WriteStartElement("Import");
						writer.WriteAttributeString("Project", "$(VCTargetsPath)\\Microsoft.Cpp.targets");
					writer.WriteEndElement();
			
					writer.WriteStartElement("ImportGroup");
						writer.WriteAttributeString("Label", "ExtensionTargets");
					writer.WriteEndElement();

				writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Flush();
			writer.Close();
		}
	}
}
