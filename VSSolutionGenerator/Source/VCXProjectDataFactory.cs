﻿using System;
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
			c.mFunctionLevelLinking = true;
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
		
		public static void AddDefaultDefsDepsIncs(ProjectConfiguration inProjConfig)
		{
			AddDefaultPreprocessorDefinitions(inProjConfig);
			AddDefaultAdditionalDependencies(inProjConfig);
		}
		
		public static ConfigType StrToConfigType(string inConfigType)
		{
			if (inConfigType.Equals("Application"))
			{
				return ConfigType.Application;
			}
			
			if (inConfigType.Equals("StaticLibrary"))
			{
				return ConfigType.StaticLibrary;
			}

			return ConfigType.Application;
		}

		public static VCXProjectData GenerateDefaultProjectData(ProjectSettingsJsonData inProjectData, ProjectSourceFiles inSourceFiles)
		{
			VCXProjectData projData = new VCXProjectData();
			projData.mProjectName = inProjectData.projectName;
			projData.mProjectUID = "{" + Guid.NewGuid().ToString() + "}";
			projData.mSourceFiles = inSourceFiles;
			
			ConfigType configType = StrToConfigType(inProjectData.configType);

			string precompiledHeader = "";
			if (!inProjectData.precompiledHeaderName.Equals(""))
			{
				precompiledHeader = inProjectData.precompiledHeaderName + ".cpp";
			}

			{
				var c = CreateDefaultDebugProjectConfig();
				c.mConfigType = configType;
				c.mPrecompiledHeaderName = precompiledHeader;
				c.mIncludePaths.AddRange(inProjectData.includePaths);

				AddDefaultDefsDepsIncs(c);

				projData.mConfigs.Add(c);
			}
			
			{
				var c = CreateDefaultDebugProjectConfig();
				c.mPlatform = Platform.x64;
				c.mConfigType = configType;
				c.mPrecompiledHeaderName = precompiledHeader;
				c.mIncludePaths.AddRange(inProjectData.includePaths);

				AddDefaultDefsDepsIncs(c);

				projData.mConfigs.Add(c);
			}
			
			{
				var c = CreateDefaultReleaseProjectConfig();
				c.mConfigType = configType;
				c.mPrecompiledHeaderName = precompiledHeader;
				c.mIncludePaths.AddRange(inProjectData.includePaths);

				AddDefaultDefsDepsIncs(c);

				projData.mConfigs.Add(c);
			}
			
			{
				var c = CreateDefaultReleaseProjectConfig();
				c.mPlatform = Platform.x64;
				c.mConfigType = configType;
				c.mPrecompiledHeaderName = precompiledHeader;
				c.mIncludePaths.AddRange(inProjectData.includePaths);

				AddDefaultDefsDepsIncs(c);

				projData.mConfigs.Add(c);
			}

			return projData;
		}
	}
}
