using DotNetProjectParser;
using System.Collections.Generic;

namespace ExportProjectTemplate
{
	internal class FolderTreeHelper
	{
		public static List<Folder> GetFoldersFromProjectItems(List<ProjectItem> projectItems)
		{
			var folders = new List<Folder>();
			projectItems.Sort((item1, item2) => item1.Include.CompareTo(item2.Include));
			var folderByPath = new Dictionary<string, Folder>();
			foreach (var item in projectItems)
			{
				var str = item.Include;
				if (str.EndsWith("\\")) // we have a folder
				{
					EnsureFolder(folders, folderByPath, str);
				}
				else // we have a file
				{
					var lastSlashPosition = str.LastIndexOf("\\");
					var parentFolderPath = str.Substring(0, lastSlashPosition + 1);
					var parentFolder = EnsureFolder(folders, folderByPath, parentFolderPath);
					var fileName = str.Substring(lastSlashPosition + 1);
					parentFolder.Files.Add(item);
				}
			}
			return folders;
		}

		private static Folder EnsureFolder(List<Folder> rootFolders, Dictionary<string, Folder> folderByPath, string folderPath)
		{
			if (!folderByPath.TryGetValue(folderPath, out var folder))
			{
				var folderPathWithoutEndSlash = folderPath.TrimEnd('\\');
				var lastSlashPosition = folderPathWithoutEndSlash.LastIndexOf("\\");
				List<Folder> folders;
				string folderName;
				if (lastSlashPosition < 0) // it's a first level folder
				{
					folderName = folderPathWithoutEndSlash;
					folders = rootFolders;
				}
				else
				{
					var parentFolderPath = folderPath.Substring(0, lastSlashPosition + 1);
					folders = folderByPath[parentFolderPath].Folders;
					folderName = folderPathWithoutEndSlash.Substring(lastSlashPosition + 1);
				}
				folder = new Folder
				{
					Name = folderName
				};
				folders.Add(folder);
				folderByPath.Add(folderPath, folder);
			}
			return folder;
		}
	}
}