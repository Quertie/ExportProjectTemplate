using DotNetProjectParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ExportProjectTemplate
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var arguments = ParseArguments(args);

			var projectPath = TryGetArgument(arguments, "projectPath");
			var templateName = TryGetArgument(arguments, "templateName");
			var outputFolderPath = TryGetArgument(arguments, "outputFolderPath");
			var iconPath = TryGetArgument(arguments, "iconPath");
			var previewImagePath = TryGetArgument(arguments, "previewImagePath");

			var projectFileName = Path.GetFileName(projectPath);
			var project = Project.Construct(new FileInfo(projectPath));
			var templateFolderPath = Path.Combine(outputFolderPath, "Temp");
			var zipFilePath = Path.Combine(outputFolderPath, $"{templateName}.zip");

			ExportTemplateToTempFolder(templateFolderPath, project, projectFileName, iconPath, previewImagePath, templateName);
			ZipTempFolderContents(templateFolderPath, zipFilePath);
			if (Directory.Exists(templateFolderPath)) Directory.Delete(templateFolderPath, true);
		}

		private static Dictionary<string, string> ParseArguments(string[] args)
		{
			var arguments = new Dictionary<string, string>();
			foreach (var arg in args)
			{
				var parts = arg.Split('=');
				if (parts.Length == 2)
				{
					arguments[parts[0]] = parts[1];
				}
			}
			return arguments;
		}

		private static string TryGetArgument(Dictionary<string, string> arguments, string key)
		{
			if (!arguments.TryGetValue(key, out var argument))
			{
				throw new ArgumentException($"Argument {key} is required");
			}
			return argument;
		}

		private static void ExportTemplateToTempFolder(string templateFolderPath,
													   Project project,
													   string projectFileName,
													   string iconPath,
													   string previewImagePath,
													   string templateName)
		{
			if (Directory.Exists(templateFolderPath)) Directory.Delete(templateFolderPath, true);

			foreach (var item in project.Items.Where(i => i.ItemType != "Reference" && i.ItemType != "PackageReference"))
			{
				var destFilePath = Path.Combine(templateFolderPath, item.Include);
				Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
				File.Copy(item.ResolvedIncludePath, destFilePath, true);
			}

			File.Copy(project.FullPath, Path.Combine(templateFolderPath, projectFileName));

			ParametrizeInFiles(templateFolderPath, project);

			File.Copy(iconPath, Path.Combine(templateFolderPath, "__TemplateIcon.ico"));
			File.Copy(previewImagePath, Path.Combine(templateFolderPath, "__PreviewImage.png"));

			var result = VSTemplateFileHelper.GetVsTemplateText(project, projectFileName, templateName);

			File.WriteAllText(Path.Combine(templateFolderPath, "MyTemplate.vstemplate"), result);
		}

		private static void ParametrizeInFiles(string templateFolderPath, Project project)
		{
			var guid = GetProjectGuid(project);

			foreach (var file in Directory.GetFiles(templateFolderPath, "*.*", SearchOption.AllDirectories).Where(ResourceHelper.FileIsNotResource))
			{
				var content = File.ReadAllText(file);
				content = content.Replace(project.AssemblyName, "$safeprojectname$");
				content = content.Replace(guid, "{$guid1$}");
				File.WriteAllText(file, content);
			}
		}

		/// <summary>
		/// Retrieves the guid in {60dc8134-eba50-45db-8e07-4cd62e7a2f24} format
		/// </summary>
		private static string GetProjectGuid(Project project)
		{
			return project.ProjectXml.Root.Elements()
				.First(n => n.Name.LocalName == "PropertyGroup")
				.Elements()
				.Single(n => n.Name.LocalName == "ProjectGuid")
				.Value;
		}

		private static void ZipTempFolderContents(string templateFolderPath, string archiveFilePath)
		{
			if (File.Exists(archiveFilePath)) File.Delete(archiveFilePath);
			ZipFile.CreateFromDirectory(templateFolderPath, archiveFilePath);
		}
	}
}