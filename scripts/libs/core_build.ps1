formatTaskName "-------------------------------- {0} --------------------------------"

# includes the NuGet API Key file.
include build.properties.ps1

properties {
	## After compiling and publishing the package 
	## the build script commits and tags it using the $logTitle variable.
	## 		Example of commit message "BUILD: New version of $logTitle"
	## 		Example of tag "$logTitle-v$commitVersion"
	$logTitle				= $nuget_id

	$nuget_symbolsPack		= $true
	
	# git version
	if ($versionControlSystem -eq "git") {
		$revisionNumber		= exec { git rev-list --count HEAD } 
	}
	# mercurial version
	if ($versionControlSystem -eq "hg") {
		$revisionNumber		= exec { hg identify -n } 
	}
	
	## Do Not Change
	$buildVersion 			= $revisionNumber.Replace("+", "")
	$version        		= "$majorVersion.$minorVersion.$buildVersion"
	$fileVersion          	= $version
	$commitVersion			= "$majorVersion.$minorVersion.$buildVersion"
	$nuget_packPath			= "$nuget_directory\$nuget_id.$version.nupkg"
	$nuget_packSymbolsPath	= "$nuget_directory\$nuget_id.$version.symbols.nupkg"
	$build_directory  		= "$nuget_directory\$version"
	$build_assemblyPath		= "$source_directory\$versionAssemblyPath"
	$build_appPath			= "$source_directory\$projectPath"	
}

#
# default task, this task starts the build
#
task default -depends NuGet_Publish, CommitAndTag

task Clean {
    Remove-Item $build_directory -Force -Recurse -ErrorAction SilentlyContinue
}

task Compile -depends Clean {
	
    ## Create Version Assembly Info
	Generate-Version-Assembly-Info `
		-file $build_assemblyPath `
		-version $version `
		-fileVersion $fileVersion
		
    if ((Test-Path $build_directory) -eq $false) {
        New-Item $build_directory -ItemType Directory | Out-Null
    }
}

task NuGet_Pack -depends Compile {

	# if we want to pack the symbols too.
    if ($nuget_symbolsPack) {
		exec { nuget pack $build_appPath -Symbols -Build -Properties "Configuration=Release;Platform=AnyCPU" -Version $version -OutputDirectory $nuget_directory }
	}
	else {
		exec { nuget pack $build_appPath -Build -Properties "Configuration=Release;Platform=AnyCPU" -Version $version -OutputDirectory $nuget_directory }
	}
	
}

task NuGet_Publish -depends NuGet_Pack {

	Remove-Item $build_directory -Force -Recurse -ErrorAction SilentlyContinue
	
	# push the package with assmblies.
	exec { nuget push $nuget_packPath -ApiKey $nuget_apiKey -Source $nuget_source }
	
	# if there are symbols push them too.
	if ($nuget_symbolsPack) {
		Write-Host "Push Symbols"
		exec { nuget push $nuget_packSymbolsPath -ApiKey $nuget_apiKey -Source $nuget_source }
	}
	
}

task CommitAndTag {

	if ($versionControlSystem -eq "git") { 
		exec { git commit -a -m "BUILD: New version of $logTitle" }
		exec { git tag "$logTitle-v$commitVersion" }
	}
	if ($versionControlSystem -eq "hg") {
		exec { hg commit -m "BUILD: New version of $logTitle" }
		exec { hg tag "$logTitle-v$commitVersion" }
	}

}
