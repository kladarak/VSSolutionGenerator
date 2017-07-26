using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSSolutionGenerator
{
	class SolutionGenerator
	{
		private string mSolutionSettingsFilename;

		private SolutionSettingsJsonData mSolutionSettings;
		private List<ProjectSettingsJsonData> mProjectSettings;

		private SolutionData mSolutionData;
		private List<VCXProjectData> mProjectData;

		private Dictionary<string, VCXProjectData> mProjectNameDataMap;

		public SolutionGenerator(string inSolutionSettingsFilename)
		{
			mSolutionSettingsFilename = inSolutionSettingsFilename;
		}

		public bool Generate()
		{
			if (!ReadSolutionSettingsJsonFile())
				return false;

			if (!GatherProjectSettingsJsonFiles())
				return false;

			if (!GenerateProjectData())
				return false;

			if (!GenerateSolutionData())
				return false;
			
			if (!SetProjectDependencies())
				return false;

			if (!CreateDirectories())
				return false;

			if (!ExportSolutionFile())
				return false;
			
			if (!ExportProjectFiles())
				return false;

			if (!ExportFilterFiles())
				return false;

			return true;
		}

		private bool ReadSolutionSettingsJsonFile()
		{
			mSolutionSettings = FileUtils.ReadJsonFile<SolutionSettingsJsonData>(mSolutionSettingsFilename);
			return mSolutionSettings != null;
		}

		private bool GatherProjectSettingsJsonFiles()
		{
			mProjectSettings = new List<ProjectSettingsJsonData>();

			string[] projectSettingsFilenames = Directory.GetFiles(mSolutionSettings.projectSourceDirectory, "ProjectSettings.json", SearchOption.AllDirectories);

			foreach (var filename in projectSettingsFilenames)
			{
				var projectSettings = FileUtils.ReadJsonFile<ProjectSettingsJsonData>(filename);

				if (projectSettings != null)
				{
					projectSettings.mSettingsFilename = filename;
					mProjectSettings.Add(projectSettings);
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		private bool GenerateProjectData()
		{
			mProjectData = new List<VCXProjectData>();
			mProjectNameDataMap = new Dictionary<string, VCXProjectData>();

			foreach (var projSettings in mProjectSettings)
			{
				string sourceFilesDirectory = Path.GetDirectoryName(projSettings.mSettingsFilename) + "\\" + projSettings.sourceDirectory;

				ProjectSourceFiles sourceFiles = new ProjectSourceFiles(sourceFilesDirectory);
				VCXProjectData projectData = VCXProjectDataFactory.GenerateDefaultProjectData(projSettings, sourceFiles);

				mProjectData.Add(projectData);
				mProjectNameDataMap.Add(projectData.mProjectName, projectData);
			}

			return true;
		}

		private bool GenerateSolutionData()
		{
			mSolutionData = SolutionDataFactory.GenerateSolutionData(mSolutionSettings);
			mSolutionData.mProjects = mProjectData;

			return true;
		}
		
		private bool SetProjectDependencies()
		{
			foreach (var projSettings in mProjectSettings)
			{
				var projData = mProjectNameDataMap[projSettings.projectName];
				projData.mProjectDependencies = new List<string>();

				foreach (var projDep in projSettings.projectDependencies)
				{
					var depData = mProjectNameDataMap[projDep];
					if (depData == null)
					{				
						System.Console.WriteLine("Failed to find project '{0}' when matching dependencies for '{1}'", projDep, projSettings.projectName);
						return false;
					}
					else
					{
						projData.mProjectDependencies.Add(depData.mProjectUID);
					}
				}
			}

			return true;
		}
		
		private bool CreateDirectories()
		{
			Directory.CreateDirectory(mSolutionSettings.solutionDirectory);
			Directory.CreateDirectory(mSolutionSettings.binariesDirectory);

			return true;
		}
		
		private bool ExportSolutionFile()
		{
			return new SolutionExporter().Export(mSolutionSettings.solutionDirectory, mSolutionData);
		}

		private bool ExportProjectFiles()
		{	
			var exporter = new VCXProjectExporter(mSolutionSettings.solutionDirectory);

			foreach (var projectData in mProjectData)
			{
				exporter.Export(projectData);
			}

			return true;
		}

		private bool ExportFilterFiles()
		{
			foreach (var projectData in mProjectData)
			{
				FiltersExporter.Export(mSolutionSettings.solutionDirectory, projectData);
			}

			return true;
		}
	}
}
