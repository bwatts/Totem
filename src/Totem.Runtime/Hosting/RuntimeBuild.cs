using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO.Compression;
using System.Linq;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// A deployable build of the Totem runtime
	/// </summary>
	[Export(typeof(IRuntimeBuild))]
	public class RuntimeBuild : Notion, IRuntimeBuild
	{
		public void Deploy(IOLink location)
		{
			if(location is FolderLink)
			{
				Deploy(location as FolderLink);
			}
			else
			{
				Deploy(location as FileLink);
			}
		}

		private void Deploy(FolderLink location)
		{
			Log.Info("[deploy] {Location:l}", location);

			Deploy(new LocalFolder(location));
		}

		private void Deploy(FileLink zipLocation)
		{
			Log.Info("[deploy] {Location:l}", zipLocation);

			var zipFolder = new LocalFolder(zipLocation.Folder);

			var outputResource = FolderResource.From("_deploy_");
			var outputFolder = zipFolder.Then(outputResource);

			try
			{
				Deploy(outputFolder);

				zipFolder.Delete(zipLocation.Name, strict: false);

				Log.Info("[deploy] Zipping to {Location:l}", zipLocation);

				ZipFile.CreateFromDirectory(
					outputFolder.Link.ToString(),
					zipLocation.ToString(),
					default(CompressionLevel),
					includeBaseDirectory: false);
			}
			finally
			{
				outputFolder.Delete(strict: false);

				Log.Info("[deploy] Deleted temporary folder {Output:l}", outputResource);
			}
		}

		private void Deploy(IFolder location)
		{
			location.Delete(FolderResource.Root, strict: false);

			foreach(var package in
				from region in Runtime.Regions
				from package in region.Packages
				where package.Name != "Totem" && package.Name != "Totem.Runtime" 
				select package)
			{
				Log.Info("[deploy] Package {Package:l}", package);

				Deploy(location, package);
			}
		}

		private void Deploy(IFolder location, RuntimePackage package)
		{
			var packageLocation = location.Then(FolderResource.From(package.Name));

			foreach(var buildFile in package.BuildFolder.ReadFiles(recursive: true))
			{
				var data = package.BuildFolder.ReadFile(buildFile);

				packageLocation.Write(buildFile, data, createFolders: true);

				Log.Verbose("[deploy] {Location:l}", packageLocation.Link.Then(buildFile));
			}

			foreach(var deployedResource in package.Areas.SelectMany(area => area.DeployedResources))
			{
				if(deployedResource is FolderResource)
				{
					var deployedFolder = (FolderResource) deployedResource;

					var files = package.DeploymentFolder.ReadFiles(deployedFolder, recursive: true);

					foreach(var file in files)
					{
						var data = package.DeploymentFolder.ReadFile(file);

						packageLocation.Write(file, data, createFolders: true);
					}
				}
				else
				{
					var deployedFile = (FileResource) deployedResource;

					var data = package.DeploymentFolder.ReadFile(deployedFile);

					packageLocation.Write(deployedFile, data, createFolders: true);

					Log.Verbose("[deploy] {Location:l}", packageLocation.Link.Then(deployedFile));
				}
			}
		}
	}
}