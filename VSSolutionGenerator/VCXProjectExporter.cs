using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VSSolutionGenerator
{
	enum Platform
	{
		Win32,
		x64,
		ARM,
	}

	enum ConfigType
	{
		Application,
		StaticLibrary
	}

	enum WarningLevel
	{
		Level3,
		Level4,
	}

	enum Optimization
	{
		Disabled,
		MaxSpeed,
	}

	class ProjectConfiguration
	{
		public string mConfigName;
		public Platform mPlatform;
		public ConfigType mConfigType;
		public bool mUseDebugLibraries;
		public bool mUseWholeProgramOptimisation;
		public List<string> mIncludePaths;
		public WarningLevel mWarningLevel;
		public Optimization mOptimization;
		public bool mUsePrecompiledHeader;
		public List<string> mPreprocessorDefinitions;
		public bool mFunctionLevelLinking;
		public bool mInstrinsicFunctions;
		public bool mEnableCOMDATFolding;
		public bool mOptimizeReferences;

		public string GetConfigPlatformName()
		{
			return mConfigName + "|" + mPlatform.ToString();
		}

		public string GetCondition()
		{
			return "'$(Configuration)|$(Platform)'=='" + GetConfigPlatformName() + "'";
		}

		private string BoolToString(bool inBool)
		{
			return inBool ? "true" : "false";
		}

		public string GetUseDebugLibraries() { return BoolToString(mUseDebugLibraries); }
		public string GetUseWholeProgramOptimisation() { return BoolToString(mUseWholeProgramOptimisation); }
		public string GetWarningLevel() { return mWarningLevel.ToString(); }
		public string GetOptimization() { return mOptimization.ToString(); }
		public string GetUsePrecompiledHeader() { return mUsePrecompiledHeader ? "Use" : "No?"; } // FIXME: Not sure what the falsey value is here
		public string GetFunctionLevelLinking() { return BoolToString(mFunctionLevelLinking); }
		public string GetInstrinsicFunctions() { return BoolToString(mInstrinsicFunctions); }
		public string GetEnableCOMDATFolding() { return BoolToString(mEnableCOMDATFolding); }
		public string GetOptimizeReferences() { return BoolToString(mOptimizeReferences); }
		
		private static string ConcatList(List<string> inStrings)
		{
			string outStr = "";

			foreach (var path in inStrings)
			{
				outStr += path + ";";
			}

			return outStr;
		}

		public string GetIncludePaths() { return ConcatList(mIncludePaths); }
		public string GetPreprocessorDefinitions() { return ConcatList(mPreprocessorDefinitions); }
	}

	class VCXProjectFileData
	{
		public string mProjectName;
		public List<ProjectConfiguration> mProjectConfigs;
		public List<string> mIncludeFiles;
		public List<string> mCompileFiles;
	}

	class VCXProjectExporter
	{
		public static void Export(string inTargetFilename, VCXProjectFileData inData)
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

					foreach (var projConfig in inData.mProjectConfigs)
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
			
					foreach (var projConfig in inData.mProjectConfigs)
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
			
					foreach (var projConfig in inData.mProjectConfigs)
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
			
					foreach (var projConfig in inData.mProjectConfigs)
					{
						writer.WriteStartElement("PropertyGroup");
							writer.WriteAttributeString("Condition", projConfig.GetCondition());
							writer.WriteElementString("IncludePath", projConfig.GetIncludePaths());
						writer.WriteEndElement();
					}
					
					foreach (var projConfig in inData.mProjectConfigs)
					{
						writer.WriteStartElement("ItemDefinitionGroup");
							writer.WriteAttributeString("Condition", projConfig.GetCondition());
							writer.WriteStartElement("ClCompile");
								writer.WriteElementString("PrecompiledHeader", projConfig.GetUsePrecompiledHeader());
								writer.WriteElementString("WarningLevel", projConfig.GetWarningLevel());
								writer.WriteElementString("Optimization", projConfig.GetOptimization());
								writer.WriteElementString("FunctionLevelLinking", projConfig.GetFunctionLevelLinking());
								writer.WriteElementString("InstrinsicFunctions", projConfig.GetInstrinsicFunctions());
								writer.WriteElementString("PreprocesserDefinitions", projConfig.GetPreprocessorDefinitions());
							writer.WriteEndElement();
							writer.WriteStartElement("Link");
								writer.WriteElementString("SubSystem", "Windows");
								writer.WriteElementString("EnableCOMDATFolding", projConfig.GetEnableCOMDATFolding());
								writer.WriteElementString("OptimizeReferences", projConfig.GetOptimizeReferences());
							writer.WriteEndElement();
						writer.WriteEndElement();
					}

					writer.WriteStartElement("ItemGroup");

					foreach (var includeFile in inData.mIncludeFiles)
					{
						writer.WriteStartElement("ClInclude");
							writer.WriteAttributeString("Include", includeFile);
						writer.WriteEndElement();
					}

					writer.WriteEndElement();

					writer.WriteStartElement("ItemGroup");
			
					foreach (var compileFile in inData.mCompileFiles)
					{
						writer.WriteStartElement("ClCompile");
							writer.WriteAttributeString("Include", compileFile);
							// TODO: Add check for precompiled header name.
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
