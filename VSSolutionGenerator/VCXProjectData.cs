using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public string mConfigName = "Unnamed Config";
		public Platform mPlatform = Platform.Win32;
		public ConfigType mConfigType = ConfigType.Application;
		public bool mUseDebugLibraries = false;
		public bool mUseWholeProgramOptimisation = false;
		public List<string> mIncludePaths = new List<string>();
		public WarningLevel mWarningLevel = WarningLevel.Level4;
		public Optimization mOptimization = Optimization.Disabled;
		public List<string> mPreprocessorDefinitions = new List<string>();
		public List<string> mAdditionalDependencies = new List<string>();
		public bool mFunctionLevelLinking = false;
		public bool mInstrinsicFunctions = false;
		public bool mEnableCOMDATFolding = false;
		public bool mOptimizeReferences = false;
		public string mPrecompiledHeaderName = "";
		
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
		public string GetUsePrecompiledHeader() { return mPrecompiledHeaderName.Equals("") ? "No" : "Use"; } // FIXME: Not sure what the falsey value is here
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
		public string GetAdditionalDependencies() { return ConcatList(mAdditionalDependencies); }
	}

	class VCXProjectData
	{
		public string mProjectName;
		public string mProjectFilename; // including path relative to solution
		public string mProjectUID;
		public List<ProjectConfiguration> mConfigs = new List<ProjectConfiguration>();
		public ProjectSourceFiles mSourceFiles;
		public List<string> mProjectDependencies = new List<string>(); // {GUID}s.
	}
}
