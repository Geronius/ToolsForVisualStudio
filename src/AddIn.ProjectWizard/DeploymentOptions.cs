﻿// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DeploymentFramework.VisualStudioAddIn.ProjectWizard
{
    public class DeploymentOptions
    {
        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing schemas?")]
        [DisplayName("Deploy schemas?")]
        [DefaultValue(false)]
        public bool IncludeSchemas { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing orchestrations?")]
        [DisplayName("Deploy orchestrations?")]
        [DefaultValue(false)]
        public bool IncludeOrchestrations { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing maps/transforms?")]
        [DisplayName("Deploy transforms/maps?")]
        [DefaultValue(false)]
        public bool IncludeTransforms { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing custom pipelines?")]
        [DisplayName("Deploy custom pipelines?")]
        [DefaultValue(false)]
        public bool IncludePipelines { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing .NET components?")]
        [DisplayName("Deploy .NET components?")]
        [DefaultValue(false)]
        public bool IncludeComponents { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing custom pipeline components?")]
        [DisplayName("Deploy pipeline components?")]
        [DefaultValue(false)]
        public bool IncludePipelineComponents { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy assemblies containing custom functoids?")]
        [DisplayName("Deploy custom functoids?")]
        [DefaultValue(false)]
        public bool IncludeCustomFunctoids { get; set; }
        
        [Category("1. What to Deploy")]
        [Description("Deploy BRE vocabularies?")]
        [DisplayName("Deploy BRE vocabs?")]
        [DefaultValue(false)]
        public bool IncludeBREVocabularies { get; set; }
        
        [Category("1. What to Deploy")]
        [Description("Deploy BRE rule policies?")]
        [DisplayName("Deploy BRE policies?")]
        [DefaultValue(false)]
        public bool IncludeBREPolicies { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy IIS virtual directories?")]
        [DisplayName("Deploy IIS virtual directories?")]
        [DefaultValue(false)]
        public bool IncludeVirtualDirectories { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy BizTalk bindings from PortBindings.xml or PortBindingsMaster.xml file?")]
        [DisplayName("Deploy BizTalk bindings?")]
        [DefaultValue(false)]
        public bool IncludeMessagingBindings { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy an NUnit unit test assembly for testing the success of the deployment?")]
        [DisplayName("Deploy an NUnit unit test assembly?")]
        [DefaultValue(false)]
        public bool IncludeDeploymentTest { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy log4net configuration XML file ($(ProjectName).log4net), DLLs and registry key?")]
        [DisplayName("Deploy log4net artifacts?")]
        [DefaultValue(false)]
        public bool Includelog4net { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy environment settings into SSO affiliate application?")]
        [DisplayName("Deploy settings into SSO?")]
        [DefaultValue(false)]
        public bool IncludeSSO { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy ESB Toolkit itineraries into the itinerary database?")]
        [DisplayName("Deploy ESB Toolkit itineraries?")]
        [DefaultValue(false)]
        public bool IncludeEsbItineraries { get; set; }

        [Category("1. What to Deploy")]
        [Description("Deploy BAM activities and/or views?")]
        [DisplayName("Deploy BAM activities and/or views?")]
        [DefaultValue(false)]
        public bool IncludeBam { get; set; }


        [Category("2. How to Deploy")]
        [Description("Auto recreate VirtualDirectory?")]
        [DisplayName("Auto recreate VirtualDirectory?")]
        [DefaultValue(false)]
        public bool ReCreateWcfServices { get; set; }

        [Category("2. How to Deploy")]
        [Description("Run InstallUtil.exe on all .NET component assemblies listed in Components?")]
        [DisplayName("Run InstallUtil on all .NET component assemblies?")]
        [DefaultValue(false)]
        public bool IncludeInstallUtilForComponents { get; set; }

        [Category("2. How to Deploy")]
        [Description("Use a master/template bindings XML file (PortBindingsMaster.xml)? It is transformed at deploy time to PortBindings.xml by optional re-encoding of nested XML fragments and optional replacement of template values from the settings spreadsheet.")]
        [DisplayName("Use a master bindings XML file?")]
        [DefaultValue(false)]
        public bool UsingMasterBindings { get; set; }

        [Category("2. How to Deploy")]
        [Description("Require XML files to contain #ifdef preprocessor directives when performing macro replacements from the settings spreadsheet? Otherwise, macros (like ${SettingName}) are replaced globally.")]
        [DisplayName("Require #ifdef for XML macro replacement?")]
        [DefaultValue(false)]
        public bool RequireXmlPreprocessDirectives { get; set; }

        [Category("2. How to Deploy")]
        [Description("Use un-encoded (&gt; to > etc.) XML in adapter bindings in PortBindings.xml or PortBindingsMaster.xml? ElementTunnel.exe will be executed to re-encode the XML before passing it to BizTalk.")]
        [DisplayName("Use un-encoded XML in adapter bindings?")]
        [DefaultValue(false)]
        public bool ApplyXmlEscape { get; set; }

        [Category("2. How to Deploy")]
        [Description("Include SettingsFileGenerator.xml Excel spreadsheet in the server install MSI? The spreadsheet may contain sensitive information like passwords and connection strings. It may be preferred to keep it in a secure location.")]
        [DisplayName("Include SettingsFileGenerator.xml in the MSI?")]
        [DefaultValue(false)]
        public bool IncludeSettingsSpreadsheetInMsi { get; set; }

        [Category("2. How to Deploy")]
        [Description("Add .NET component assemblies and IIS vdirs as resources in the BizTalk app? May be desirable if you ever export an MSI from the BizTalk Admin Console.")]
        [DisplayName("Add components/vdirs as resources in BizTalk app?")]
        [DefaultValue(false)]
        public bool IncludeCompsAndVDirsAsResources { get; set; }

        [Category("2. How to Deploy")]
        [Description("Skip IIS reset or AppPool reset during deployment? Reset is not necessary if not using IIS-related ports that could lock files in the application.")]
        [DisplayName("Skip IIS/AppPool reset during deploy?")]
        [DefaultValue(false)]
        public bool SkipIISReset { get; set; }

        [Category("2. How to Deploy")]
        [Description("Skip BizTalk host instances restart during deployment? Skipping restart could leave outdated artifacts in memory!")]
        [DisplayName("Skip host instances restart during deploy?")]
        [DefaultValue(false)]
        public bool SkipHostInstancesRestart { get; set; }

        [Category("2. How to Deploy")]
        [Description("Start the BizTalk app after deployment or leave it in un-started state?")]
        [DisplayName("Start BizTalk app after deployment?")]
        [DefaultValue(false)]
        public bool StartApplicationOnDeploy { get; set; }

        [Category("2. How to Deploy")]
        [Description("Enable all receive locations in the BizTalk app after deployment or leave them as-is? Does not apply if StartApplicationOnDeploy = false.")]
        [DisplayName("Enable all receive locations after deploy?")]
        [DefaultValue(false)]
        public bool EnableAllReceiveLocationsOnDeploy { get; set; }

        [Category("2. How to Deploy")]
        [Description("Start all other BizTalk apps referenced by this app during deployment or leave them as-is? Does not apply if StartApplicationOnDeploy = false.")]
        [DisplayName("Start referenced apps during deploy?")]
        [DefaultValue(false)]
        public bool StartReferencedApplicationsOnDeploy { get; set; }

        [Category("3. Advanced Deployment Options")]
        [Description("Configure BizTalk to run this application in an isolated AppDomain? BTSNTSvc.exe.config will be modified to map this project's assemblies into a unique AppDomain.")]
        [DisplayName("Run app in an isolated AppDomain?")]
        [DefaultValue(false)]
        public bool UseIsolatedAppDomain { get; set; }

        [Category("3. Advanced Deployment Options")]
        [Description("Enable BizTalk's extended logging debug option? BTSNTSvc.exe.config will be modified to enable this option.")]
        [DisplayName("Enable BizTalk's extended logging debug option?")]
        [DefaultValue(false)]
        public bool EnableBizTalkExtendedLogging { get; set; }

        [Category("3. Advanced Deployment Options")]
        [Description("Enable BizTalk's assembly validation debug option? BTSNTSvc.exe.config will be modified to enable this option.")]
        [DisplayName("Enable BizTalk's assembly validation debug option?")]
        [DefaultValue(false)]
        public bool EnableBizTalkAssemblyValidation { get; set; }

        [Category("3. Advanced Deployment Options")]
        [Description("Enable BizTalk's correlation validation debug option? BTSNTSvc.exe.config will be modified to enable this option.")]
        [DisplayName("Enable BizTalk's correlation validation debug option?")]
        [DefaultValue(false)]
        public bool EnableBizTalkCorrelationValidation { get; set; }

        [Category("3. Advanced Deployment Options")]
        [Description("Enable BizTalk's schema validation debug option? BTSNTSvc.exe.config will be modified to enable this option.")]
        [DisplayName("Enable BizTalk's schema validation debug option?")]
        [DefaultValue(false)]
        public bool EnableBizTalkSchemaValidation { get; set; }
        
        [Category("4. DeploymentWizard Options")]
        [Description("Create WCFWebsite folder?")]
        [DisplayName("Create WCFWebsite folder?")]
        [DefaultValue(false)]
        public bool CreateWCFWebsiteFolder { get; set; }

        [Category("4. DeploymentWizard Options")]
        [Description("Create SoapWebsite folder?")]
        [DisplayName("Create SoapWebsite folder?")]
        [DefaultValue(false)]
        public bool CreateSoapWebsiteFolder { get; set; }


        public DeploymentOptions()
        {
            this.IncludeSchemas = true;
            this.IncludeOrchestrations = true;
            this.IncludeTransforms = true;
            this.IncludeMessagingBindings = true;
            //this.IncludeSSO = true;
            this.IncludeSettingsSpreadsheetInMsi = true;
            //this.StartApplicationOnDeploy = true;
            //this.EnableAllReceiveLocationsOnDeploy = true;
            //this.StartReferencedApplicationsOnDeploy = true;
            this.CreateSoapWebsiteFolder = false;
            this.CreateWCFWebsiteFolder = false;
            this.UsingMasterBindings = true;
            this.RequireXmlPreprocessDirectives = false;
            this.SkipHostInstancesRestart = true;
            this.SkipIISReset = true;
            this.ApplyXmlEscape = true;
        }
    }
}
