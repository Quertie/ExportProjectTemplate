# Introduction

There is currently no way to export Visual Studio project templates via command-line.
This tool aims to do just that, enabling devs to produce project templates from within their CI pipelines.

# Usage

## Program usage
`ExportProjectTemplate projectPath=<...> templateName=<...> outputFolderPath=<...> iconPath=<...> previewImagePath=<...>`

with:
- **projectPath**: path to `.csproj` / `.vbproj` file
- **templateName**: Name of the template to be shown to the user when installed
- **outputfolderPath**: Folder where the generated template should be placed
- **iconPath**: the `.ico` file to be used for the template
- **previewImagePath**: the `.png` file to be used for the template

example:

`ExportProjectTemplate projectPath=".\project.sln" templateName="My template" outputFolderPath="C:\Users\Quertie\Documents\Visual Studio 2022\Templates\Project Templates" iconPath=".\icon.ico" previewImagePath=".\preview.png"`

## Usage in Azure DevOps pipeline

Placing the `.exe` and `.dll` at the root of your repo:

```YML
- task: PowerShell@2
    inputs:
      targetType: 'inline'
      script: .\ExportProjectTemplate.exe projectPath=".\project.csproj" templateName="Template Name" outputFolderPath="C:\Output" iconPath=".\icon.ico" previewImagePath=".\innovation.png"
  - task: PublishBuildArtifacts@1
    displayName: 'Publish AzureSignTool'
    inputs:
      PathtoPublish: 'C:\Output\Template Name.zip'
      ArtifactName: 'Template Artifact'
      publishLocation: 'Container'
```


# Limitations

This application was botched together for a specific project, so all possibilities were not necessarily considered.

- Only `.csproj` and `.vsproj` work for now, but it's an easy fix.
- any `.csproj` file is considered as a CSharp project (no distinction if it's a web project or not)
- Only .ico and .png files are excluded from "Replace parameters" in the `.vstemplate` file. (and the initial parametrization step). Other resource files face the risk of becoming corrupt.
- Only `$safeprojectname$` and `$guid1$` template parameters are introduced. This should be enough for a simple project.

Feel free to adapt the tool to your needs and why not submit a pull request if you wish to contribute.

# Download
You can find the latest version under the "Releases" section.