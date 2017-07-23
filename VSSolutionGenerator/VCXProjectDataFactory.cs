using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionGenerator
{
	class VCXProjectDataFactory
	{
		public static ProjectConfiguration CreateDefaultDebugProjectConfig()
		{
			ProjectConfiguration c = new ProjectConfiguration();

			c.mConfigName = "Debug";
			c.mPlatform = Platform.Win32;
			c.mConfigType = ConfigType.Application;
			c.mUseDebugLibraries = true;
			c.mUseWholeProgramOptimisation = false;
			c.mWarningLevel = WarningLevel.Level4;
			c.mOptimization = Optimization.Disabled;
			c.mFunctionLevelLinking = false;
			c.mInstrinsicFunctions = false;
			c.mEnableCOMDATFolding = false;
			c.mOptimizeReferences = false;
				
			return c;
		}
		
		public static ProjectConfiguration CreateDefaultReleaseProjectConfig()
		{
			ProjectConfiguration c = new ProjectConfiguration();

			c.mConfigName = "Release";
			c.mPlatform = Platform.Win32;
			c.mConfigType = ConfigType.Application;
			c.mUseDebugLibraries = false;
			c.mUseWholeProgramOptimisation = true;
			c.mWarningLevel = WarningLevel.Level4;
			c.mOptimization = Optimization.MaxSpeed;
			c.mFunctionLevelLinking = true;
			c.mInstrinsicFunctions = true;
			c.mEnableCOMDATFolding = true;
			c.mOptimizeReferences = true;

			return c;
		}

		public static void AddDefaultPreprocessorDefinitions(ProjectConfiguration inProjConfig)
		{
			var defs = inProjConfig.mPreprocessorDefinitions;

			if (inProjConfig.mPlatform == Platform.Win32)
			{
				defs.Add("WIN32");
			}

			if (inProjConfig.mUseDebugLibraries)
			{
				defs.Add("_DEBUG");
			}
			else
			{
				defs.Add("NDEBUG");
			}

			// _CONSOLE if console subsystem?
			// _LIB if static library? Maybe also if console?

			defs.Add("%(PreprocessorDefinitions)");
		}

		public static void AddDefaultAdditionalDependencies(ProjectConfiguration inProjConfig)
		{
			var deps = inProjConfig.mAdditionalDependencies;

			deps.Add("%(AdditionalDependencies)");
		}

		public static void AddDefaultIncludePaths(ProjectConfiguration inProjConfig)
		{
			var incs = inProjConfig.mIncludePaths;

			incs.Add("$(ProjectDir)");
			incs.Add("$(IncludePath)");
		}

		public static void AddDefaultDefsDepsIncs(ProjectConfiguration inProjConfig)
		{
			AddDefaultPreprocessorDefinitions(inProjConfig);
			AddDefaultAdditionalDependencies(inProjConfig);
			AddDefaultIncludePaths(inProjConfig);
		}

		public static void AppendIncludePaths(ProjectConfiguration inProjConfig, List<string> inIncludePaths)
		{
			foreach (var path in inIncludePaths)
			{
				string normalised = FileUtils.NormaliseFilename(path);
				inProjConfig.mIncludePaths.Add("$(ProjectDir)" + normalised);
			}
		}

		public static VCXProjectData GenerateDefaultProjectData(ProjectFileJsonData inProjectData, ProjectSourceFiles inSourceFiles)
		{
			VCXProjectData projData = new VCXProjectData();
			projData.mProjectName = inProjectData.projectName;
			projData.mSourceFiles = inSourceFiles;

			{
				var c = CreateDefaultDebugProjectConfig();
				AppendIncludePaths(c, inProjectData.includePaths);
				AddDefaultDefsDepsIncs(c);
				projData.mConfigs.Add(c);
			}
			
			{
				var c = CreateDefaultDebugProjectConfig();
				c.mPlatform = Platform.x64;
				AppendIncludePaths(c, inProjectData.includePaths);
				AddDefaultDefsDepsIncs(c);
				projData.mConfigs.Add(c);
			}
			
			{
				var c = CreateDefaultReleaseProjectConfig();
				AppendIncludePaths(c, inProjectData.includePaths);
				AddDefaultDefsDepsIncs(c);
				projData.mConfigs.Add(c);
			}
			
			{
				var c = CreateDefaultReleaseProjectConfig();
				c.mPlatform = Platform.x64;
				AppendIncludePaths(c, inProjectData.includePaths);
				AddDefaultDefsDepsIncs(c);
				projData.mConfigs.Add(c);
			}

			return projData;
		}
	}
}
