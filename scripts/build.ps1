include libs\psake_ext.ps1

properties {
	## NuGet ID for the package
	$nuget_id 				= "WindowsAzure.StorageExtensions"

	## NuGet Source URL
	$nuget_source			= "https://www.nuget.org/api/v2/"

	## Version info
	$majorVersion			= 0
	$minorVersion			= 8
	
	## Path location for the project file.
	$projectPath 			= "WindowsAzure\WindowsAzure.csproj"

	## Path location for the VersionAssemblyInfo.cs file.
	$versionAssemblyPath	= "WindowsAzure\Properties\VersionAssemblyInfo.cs"
			
	## Select with Version Control System the build should use.
	## 	Use: 'git' or 'hg' for mercurial.
	$versionControlSystem	= "git"
	
	## Sources directory will be the 'src' folder 
	## on the same level of the 'build' folder.
	## Changing this to a wrong folder can brick the build script.
	$base_directory   		= resolve-path "..\."
	$source_directory 		= "$base_directory\src"	
	$nuget_directory  		= "$base_directory\builds"
}

include libs\core_build.ps1
