using System.Linq;
using System.Text;
using DotNetProjectParser;

namespace ExportProjectTemplate
{
	/// <summary>
	/// Helper class for generating the .vstemplate file
	/// </summary>
	internal class VSTemplateFileHelper
	{
		public static string GetVsTemplateText(Project project, string projectFileName, string templateName)
		{
			var stringBuilder = new StringBuilder();
			AddTemplateOpeningTag(stringBuilder);
			AddTemplateData(stringBuilder, templateName, projectFileName);
			AddTemplateContent(stringBuilder, project, projectFileName);
			AddTemplateClosingTag(stringBuilder);

			var result = stringBuilder.ToString();
			return result;
		}

		private static void AddTemplateClosingTag(StringBuilder stringBuilder)
		{
			stringBuilder.AppendLine("</VSTemplate>");
		}

		private static void AddTemplateOpeningTag(StringBuilder stringBuilder)
		{
			stringBuilder.AppendLine("<VSTemplate Version=\"3.0.0\" xmlns=\"http://schemas.microsoft.com/developer/vstemplate/2005\" Type=\"Project\">");
		}

		private static void AddTemplateData(StringBuilder stringBuilder, string templateName, string projectFileName)
		{
			var templateDescription = "Template Description";
			var projectType = projectFileName.EndsWith("csproj") ? "CSharp" : "VisualBasic";
			var iconFileName = "__TemplateIcon.ico";
			var previewImageFileName = "__PreviewImage.png";
			
			stringBuilder.AppendLine("<TemplateData>");
			stringBuilder.AppendLine($"<Name>{templateName}</Name>");
			stringBuilder.AppendLine($"<Description>{templateDescription}</Description>");
			stringBuilder.AppendLine($"<ProjectType>{projectType}</ProjectType>");
			stringBuilder.AppendLine("<ProjectSubType>");
			stringBuilder.AppendLine("</ProjectSubType>");
			stringBuilder.AppendLine("<SortOrder>1000</SortOrder>");
			stringBuilder.AppendLine($"<CreateNewFolder>true</CreateNewFolder>");
			stringBuilder.AppendLine($"<DefaultName>{templateName}</DefaultName>");
			stringBuilder.AppendLine($"<ProvideDefaultName>true</ProvideDefaultName>");
			stringBuilder.AppendLine($"<LocationField>Enabled</LocationField>");
			stringBuilder.AppendLine($"<EnableLocationBrowseButton>true</EnableLocationBrowseButton>");
			stringBuilder.AppendLine($"<Icon>{iconFileName}</Icon>");
			stringBuilder.AppendLine($"<PreviewImage>{previewImageFileName}</PreviewImage>");
			stringBuilder.AppendLine("</TemplateData>");
		}

		private static void AddTemplateContent(StringBuilder stringBuilder, Project project, string projectFileName)
		{
			stringBuilder.AppendLine("<TemplateContent>");
			stringBuilder.AppendLine($"<Project TargetFileName=\"{projectFileName}\" File=\"{projectFileName}\" ReplaceParameters=\"true\">");

			var tree = FolderTreeHelper.GetFoldersFromProjectItems(project.Items
				.Where(i => i.ItemType != "Reference")
				.ToList());

			foreach (var folder in tree)
			{
				if (folder.Name == "")
				{
					foreach (var fileItem in folder.Files)
					{
						AddFileItem(stringBuilder, fileItem);
					}
				}
				else
				{
					AddFolderItem(stringBuilder, folder);
				}
			}
			stringBuilder.AppendLine("</Project>");
			stringBuilder.AppendLine("</TemplateContent>");
		}

		private static void AddFolderItem(StringBuilder stringBuilder, Folder folder)
		{
			stringBuilder.AppendLine($"<Folder Name=\"{folder.Name}\" TargetFolderName=\"{folder.Name}\">");
			foreach (var file in folder.Files)
			{
				AddFileItem(stringBuilder, file);
			}

			foreach (var subFolder in folder.Folders)
			{
				AddFolderItem(stringBuilder, subFolder);
			}
			stringBuilder.AppendLine("</Folder>");
		}

		private static void AddFileItem(StringBuilder stringBuilder, ProjectItem fileItem)
		{
			stringBuilder.AppendLine($"<ProjectItem ReplaceParameters=\"{ResourceHelper.FileIsNotResource(fileItem.ItemName).ToString().ToLower()}\" TargetFileName=\"{fileItem.ItemName}\">{fileItem.ItemName}</ProjectItem>");
		}
	}
}