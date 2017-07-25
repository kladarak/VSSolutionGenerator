using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionGenerator
{
	class SolutionDataFactory
	{
		private static List<SolutionConfig> GenerateDefaultSolutionConfigs()
		{
			var configs = new List<SolutionConfig>();

			{
				var c = new SolutionConfig();
				c.mName = "Debug";
				c.mArchitecture = Architecture.x86;
				c.mPlatform = Platform.Win32;
				c.mBuildType = BuildType.Debug;
				configs.Add(c);
			}
			
			{
				var c = new SolutionConfig();
				c.mName = "Debug";
				c.mArchitecture = Architecture.x64;
				c.mPlatform = Platform.x64;
				c.mBuildType = BuildType.Debug;
				configs.Add(c);
			}

			{
				var c = new SolutionConfig();
				c.mName = "Release";
				c.mArchitecture = Architecture.x86;
				c.mPlatform = Platform.Win32;
				c.mBuildType = BuildType.Release;
				configs.Add(c);
			}

			{
				var c = new SolutionConfig();
				c.mName = "Release";
				c.mArchitecture = Architecture.x64;
				c.mPlatform = Platform.x64;
				c.mBuildType = BuildType.Release;
				configs.Add(c);
			}

			return configs;
		}

		public static SolutionData GenerateSolutionData(SolutionSettingsJsonData inSolutionData)
		{
			SolutionData solutionData = new SolutionData();

			solutionData.mSolutionName = inSolutionData.solutionName;
			solutionData.mSolutionGuid = "{" + Guid.NewGuid().ToString() + "}";
			solutionData.mSolutionConfigs = GenerateDefaultSolutionConfigs();

			return solutionData;
		}
	}
}
