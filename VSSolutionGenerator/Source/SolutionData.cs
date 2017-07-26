using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionGenerator
{
	enum BuildType
	{
		Debug,
		Release
	}

	enum Architecture
	{
		x86,
		x64,
		ARM
	}

	class SolutionConfig
	{
		public string mName;
		public Architecture mArchitecture;
		public Platform mPlatform;
		public BuildType mBuildType;
	}

	class SolutionData
	{
		public string mSolutionName;
		public string mSolutionGuid; // must be surrounded with {}
		public List<SolutionConfig> mSolutionConfigs;
		public List<VCXProjectData> mProjects;
	}
}
