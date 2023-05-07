using System.Reflection;
/* 
 * PrepareModuleManifest.cs
 * 
 *   Created: 2023-03-30-02:16:47
 *   Modified: 2023-03-30-02:18:30
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.PsProj.Tasks;

using System.Collections;
using Dgmjr.MSBuild.Extensions;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using static Dgmjr.MSBuild.Constants.MSBuildPropertyNames;
using static Dgmjr.MSBuild.Constants.CommonPropertyNames;
using static Path;
public class PrepareModuleManifest : MSBTask
{
    private Project Project => this.TryGetProject();
    private string ProjectFileFullPath => Project.FullPath;
    private string ProjectFileDirectory => GetDirectoryName(this.ProjectFileFullPath);
    private string ProjectFileName => GetFileNameWithoutExtension(this.ProjectFileFullPath);
    private string PsdManifestFileName => Combine(this.ProjectFileDirectory, this.ProjectFileName + ".psd1");
    private string Guid => Properties["Guid"] ?? guid.NewGuid().ToString();
    private string Title => Properties["Title"] ?? "Untitled Project";
    private string CompanyName => Properties["Company"] ?? "Anonymous, Inc.";
    private string Author => Properties["Author"] ?? "Anonymous";
    private string Version => Properties["Version"] ?? "0.0.1";
    private string RootModule => Properties["RootModule"] ?? Properties[MSBuildProjectName] + ".psm1";
    private string Description => Properties["Description"] ?? "No description provided.";
    private string LicenseExpression => Properties["LicenseExpression"] ?? "Unlicence";
    // private string CLRVersion => Properties["TargetFramework"] ?? "netstandard2.0";
    private string DotNetFrameworkVersion => Properties[TargetFramework] ?? "netstandard2.0";
    private IDictionary<string, string> _properties = null!;
    private IDictionary<string, string> Properties => _properties ??= this.GetAllEvaluatedProperties();
    private IEnumerable<string> PsdManifestItems => this.GetAllEvaluatedItems().Where(i => i.ItemType == "PsdManifest").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> RequiredModules => this.GetAllEvaluatedItems().Where(i => i.ItemType == "RequiredModule").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> RequiredScripts => this.GetAllEvaluatedItems().Where(i => i.ItemType == "Script").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> RequiredAssemblies => this.GetAllEvaluatedItems().Where(i => i.ItemType == "Reference").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> FunctionsToExport => this.GetAllEvaluatedItems().Where(i => i.ItemType == "Function").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> AliasesToExport => this.GetAllEvaluatedItems().Where(i => i.ItemType == "Alias").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> VariablesToExport => this.GetAllEvaluatedItems().Where(i => i.ItemType == "Variable").Select(i => $"\"{i.EvaluatedInclude}\"");
    private IEnumerable<string> CmdletsToExport => this.GetAllEvaluatedItems().Where(i => i.ItemType == "Cmdlet").Select(i => $"\"{i.EvaluatedInclude}\"");

    public override bool Execute()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "pwsh",
            Arguments = $""""
            -NoProfile `
            -NonInteractive `
            -Command `
            \"New-ModuleManifest `
            -Guid '{Guid}' `
            -Path '{PsdManifestFileName}'\" `
            -ModuleVersion '{Version}'\" `
            -DotNetFrameworkVersion '@({DotNetFrameworkVersion}') `
            -ScriptsToProcess @({RequiredScripts.Join(",")}') `
            -RequiredAssemblies @({RequiredAssemblies.Join(",")}) `
            -RequiredModules @({RequiredModules.Join(",")}) `
            -FunctionsToExport @({FunctionsToExport.Join(",")}) `
            -AliasesToExport @({AliasesToExport.Join(",")}) `
            -VariablesToExport @({VariablesToExport.Join(",")}) `
            -CmdletsToExport @({CmdletsToExport.Join(",")}) `
            -LicenseUri 'https://opensource.org/licenses/{LicenseExpression}' `
            """",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        var ps = new Process { StartInfo = processStartInfo };
        ps.Start();
        ps.WaitForExit();
        return true;
    }
}
