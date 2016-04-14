using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// A deployable build of the Totem runtime
	/// </summary>
	internal sealed class RuntimeBuild : Notion, IRuntimeBuild
	{
		public void Deploy(IOLink location)
		{
			Log.Info("[deploy] {Location:l}", location);

			if(location is FolderLink)
			{
				Deploy(location as FolderLink);
			}
			else
			{
				Deploy(location as FileLink);
			}

			Log.Info("[deploy] Finished");
		}

		private void Deploy(FolderLink location)
		{
			Deploy(new LocalFolder(location));
		}

		private void Deploy(FileLink zipLocation)
		{
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

		private void Deploy(IFolder output)
		{
			foreach(var packageBuild in
				from region in Runtime.Regions
				from package in region.Packages
				where package.Name != "Totem" && package.Name != "Totem.Runtime"
				select new PackageBuild(package, output))
			{
				packageBuild.Deploy();
			}
		}

		private sealed class PackageBuild : Notion
		{
			private readonly RuntimePackage _package;
			private readonly IFolder _output;

			internal PackageBuild(RuntimePackage package, IFolder deploymentOutput)
			{
				_package = package;
				_output = deploymentOutput.Then(FolderResource.From(package.Name));
			}

			internal void Deploy()
			{
				Log.Info("[deploy] Package {Package:l}", _package);

				DeployBuildFiles();

				DeployAreaFiles();
			}

			private void DeployBuildFiles()
			{
				foreach(var file in _package.BuildFolder.ReadFiles(recursive: true))
				{
					DeployFile(_package.BuildFolder, file);
				}
			}

			private void DeployAreaFiles()
			{
				foreach(var deployedResource in _package.Areas.SelectMany(area => area.DeployedResources))
				{
					if(deployedResource is FolderResource)
					{
						DeployFolder(_package.DeploymentFolder, (FolderResource) deployedResource);
					}
					else
					{
						DeployFile(_package.DeploymentFolder, (FileResource) deployedResource);
					}
				}
			}

			private void DeployFolder(IFolder source, FolderResource resource)
			{
				foreach(var file in source.ReadFiles(resource, recursive: true))
				{
					DeployFile(source, file);
				}
			}

			private void DeployFile(IFolder source, FileResource resource)
			{
				var data = source.ReadFile(resource);

				_output.Write(resource, data, createFolders: true);

				Log.Verbose("[deploy] {Location:l}", _output.Link.Then(resource));
			}
		}
	}
}