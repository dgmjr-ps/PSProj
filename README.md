---
authors:
  - dgmjr
title: PowerShell Project SDK
description: An SDK for packaging PowerShell projects
---

# PowerShell Project SDK

An SDK for packaging PowerShell projects

## Usage

Place the "PSProj" inside the "Sdk" attribute of your project and populate `ItemGroup`s like so:

```xml
<Project Sdk="PSProj">
  <PropertyGroup>
    <!-- This becomes the title of the generated module -->
    <Title>My Amazing module</Title>
    <!-- This becomes the author of the generated module -->
    <Authors>John Smith</Title>
    <!-- This becomes the GUID for the generated module -->
    <ProjectGuid>718e4509-2fe7-430b-917e-54196ad71b6f</ProjectGuid>
  </PropertyGroup>
  <ItemGroup>
    <NestedModule Include="NestedModle1.psm1" />
    <NestedModule Include="NestedModle2.psd1" />
    <RequiredModule Include="RequiredModule1.psm1" />
    <Script Include="ScriptToProcess1.ps1" />
    <Script Include="ScriptToProcess2.ps1" />
</Project>
```
