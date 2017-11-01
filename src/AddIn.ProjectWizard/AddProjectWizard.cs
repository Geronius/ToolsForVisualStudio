// Deployment Framework for BizTalk Tools for Visual Studio
// Copyright (C) 2008-Present Thomas F. Abraham. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.Win32;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace DeploymentFramework.VisualStudioAddIn.ProjectWizard
{
    public class AddProjectWizard : IWizard
    {
        #region IWizard Members

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            DTE2 dte = automationObject as DTE2;

            if (dte == null)
            {
                MessageBox.Show(
                    "Cannot convert automation object to DTE2.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!dte.Solution.IsOpen)
            {
                MessageBox.Show(
                    "Please open your BizTalk solution, then use the Add New Project dialog on the solution to add this project.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            string destinationPath = replacementsDictionary["$destinationdirectory$"];

            if (string.IsNullOrEmpty(destinationPath))
            {
                MessageBox.Show(
                    "Cannot determine destination directory ($destinationdirectory$ is missing).",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var btdfInstallDir = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\DeploymentFrameworkForBizTalk", "InstallPath", null);
            if (btdfInstallDir == null)
                btdfInstallDir = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\DeploymentFrameworkForBizTalk", "InstallPath", null);

            if (string.IsNullOrEmpty(btdfInstallDir))
            {
                MessageBox.Show(
                    "Cannot find Deployment Framework registry key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DeploymentOptions options = new DeploymentOptions();

            OptionsForm optionsFrm = new OptionsForm(options);
            if (optionsFrm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string templateDir = Path.Combine(btdfInstallDir, @"Developer\ProjectTemplate");

            string[] templateFiles = Directory.GetFiles(templateDir, "*.*", SearchOption.AllDirectories);

            try
            {
                CopyDirectory(templateDir, destinationPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to copy Deployment Framework template files to destination folder: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            
            var solutionName = Path.GetFileNameWithoutExtension(dte.Solution.FileName);
            const string projectFileName = "Deployment.btdfproj";
            var projectFilePath = Path.Combine(destinationPath, projectFileName);

            var replacements = new Dictionary<string, string>();
            replacements.Add("[PROJECTNAME]", solutionName);
            
            var solutionNameParts = solutionName.Split('.');
            if (solutionNameParts.Length == 3)
            {
                replacements["[PROJECTNAME]"] = "$(Manufacturer).$(ProjectSubPrefix).$(ProjectSubName)";

                replacements.Add("[MANUFACTURER]", solutionNameParts[0]);
                replacements.Add("[PROJECTSUBPREFIX]", solutionNameParts[1]);
                replacements.Add("[PROJECTSUBNAME]", solutionNameParts[2]);                            
            }

            ReplaceInTextFile(destinationPath, "InstallWizard.xml", replacements, Encoding.UTF8);
            ReplaceInTextFile(destinationPath, "UnInstallWizard.xml", replacements, Encoding.UTF8);

            replacements.Add("[GUID1]", Guid.NewGuid().ToString());
            replacements.Add("[GUID2]", Guid.NewGuid().ToString());
            ReplaceInTextFile(destinationPath, projectFileName, replacements, Encoding.UTF8);

            CreateWebsiteFolder(dte, options, projectFilePath, solutionName);

            UpdateProjectFile(projectFilePath, options, optionsFrm.WritePropertiesOnlyWhenNonDefault);
            UpdateSolutionFile(projectFilePath, dte);


            try
            {
                dte.ExecuteCommand("File.OpenFile", '"' + projectFilePath + '"');
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to open .btdfproj file in editor.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }

            MessageBox.Show(
                "A default Deployment Framework for BizTalk project has been configured in " + destinationPath + ". You must edit " + projectFileName + " to configure your specific deployment requirements.",
                "Project Created",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static void CreateWebsiteFolder(DTE2 dte, DeploymentOptions options, string deploymentProjectPath, string solutionName)
        {
            var soln = (Solution2)dte.Solution;
            var deploymentFolderFullPath = Path.GetDirectoryName(soln.FullName);
            var wcfWebsiteFolder = Path.Combine(deploymentFolderFullPath, String.Format("{0}.WebsiteWCF", solutionName));
            var soapWebsiteFolder = Path.Combine(deploymentFolderFullPath, String.Format("{0}.WebsiteSoap", solutionName));

            var solutionNameParts = solutionName.Split('.');

            if (options.CreateWCFWebsiteFolder && !Directory.Exists(wcfWebsiteFolder))
            {
                var wcfWebsiteFolderInfo = Directory.CreateDirectory(wcfWebsiteFolder);
                var deploymentprj = soln.AddSolutionFolder("WebsiteWCF");

                //create App_Data Folder
                wcfWebsiteFolderInfo.CreateSubdirectory("App_Data");
                var serializationFileName = String.Format("{0}\\App_Data\\Serialization.xsd", wcfWebsiteFolder);
                using (var serializationFile = File.CreateText(serializationFileName))
                {
                    serializationFile.WriteLine(HttpUtility.HtmlDecode(@"&lt;?xml version=&quot;1.0&quot;?&gt;&lt;xs:schema xmlns:tns=&quot;http://schemas.microsoft.com/2003/10/Serialization/&quot; attributeFormDefault=&quot;qualified&quot; elementFormDefault=&quot;qualified&quot; targetNamespace=&quot;http://schemas.microsoft.com/2003/10/Serialization/&quot; xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot;&gt;&lt;xs:element name=&quot;anyType&quot; nillable=&quot;true&quot; type=&quot;xs:anyType&quot; /&gt;&lt;xs:element name=&quot;anyURI&quot; nillable=&quot;true&quot; type=&quot;xs:anyURI&quot; /&gt;&lt;xs:element name=&quot;base64Binary&quot; nillable=&quot;true&quot; type=&quot;xs:base64Binary&quot; /&gt;&lt;xs:element name=&quot;boolean&quot; nillable=&quot;true&quot; type=&quot;xs:boolean&quot; /&gt;&lt;xs:element name=&quot;byte&quot; nillable=&quot;true&quot; type=&quot;xs:byte&quot; /&gt;&lt;xs:element name=&quot;dateTime&quot; nillable=&quot;true&quot; type=&quot;xs:dateTime&quot; /&gt;&lt;xs:element name=&quot;decimal&quot; nillable=&quot;true&quot; type=&quot;xs:decimal&quot; /&gt;&lt;xs:element name=&quot;double&quot; nillable=&quot;true&quot; type=&quot;xs:double&quot; /&gt;&lt;xs:element name=&quot;float&quot; nillable=&quot;true&quot; type=&quot;xs:float&quot; /&gt;&lt;xs:element name=&quot;int&quot; nillable=&quot;true&quot; type=&quot;xs:int&quot; /&gt;&lt;xs:element name=&quot;long&quot; nillable=&quot;true&quot; type=&quot;xs:long&quot; /&gt;&lt;xs:element name=&quot;QName&quot; nillable=&quot;true&quot; type=&quot;xs:QName&quot; /&gt;&lt;xs:element name=&quot;short&quot; nillable=&quot;true&quot; type=&quot;xs:short&quot; /&gt;&lt;xs:element name=&quot;string&quot; nillable=&quot;true&quot; type=&quot;xs:string&quot; /&gt;&lt;xs:element name=&quot;unsignedByte&quot; nillable=&quot;true&quot; type=&quot;xs:unsignedByte&quot; /&gt;&lt;xs:element name=&quot;unsignedInt&quot; nillable=&quot;true&quot; type=&quot;xs:unsignedInt&quot; /&gt;&lt;xs:element name=&quot;unsignedLong&quot; nillable=&quot;true&quot; type=&quot;xs:unsignedLong&quot; /&gt;&lt;xs:element name=&quot;unsignedShort&quot; nillable=&quot;true&quot; type=&quot;xs:unsignedShort&quot; /&gt;&lt;xs:element name=&quot;char&quot; nillable=&quot;true&quot; type=&quot;tns:char&quot; /&gt;&lt;xs:simpleType name=&quot;char&quot;&gt;&lt;xs:restriction base=&quot;xs:int&quot; /&gt;&lt;/xs:simpleType&gt;&lt;xs:element name=&quot;duration&quot; nillable=&quot;true&quot; type=&quot;tns:duration&quot; /&gt;&lt;xs:simpleType name=&quot;duration&quot;&gt;&lt;xs:restriction base=&quot;xs:duration&quot;&gt;&lt;xs:pattern value=&quot;\-?P(\d*D)?(T(\d*H)?(\d*M)?(\d*(\.\d*)?S)?)?&quot; /&gt;&lt;xs:minInclusive value=&quot;-P10675199DT2H48M5.4775808S&quot; /&gt;&lt;xs:maxInclusive value=&quot;P10675199DT2H48M5.4775807S&quot; /&gt;&lt;/xs:restriction&gt;&lt;/xs:simpleType&gt;&lt;xs:element name=&quot;guid&quot; nillable=&quot;true&quot; type=&quot;tns:guid&quot; /&gt;&lt;xs:simpleType name=&quot;guid&quot;&gt;&lt;xs:restriction base=&quot;xs:string&quot;&gt;&lt;xs:pattern value=&quot;[\da-fA-F]{8}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{12}&quot; /&gt;&lt;/xs:restriction&gt;&lt;/xs:simpleType&gt;&lt;xs:attribute name=&quot;FactoryType&quot; type=&quot;xs:QName&quot; /&gt;&lt;/xs:schema&gt;"));
                    serializationFile.Close();
                }

                //create svc file
                var svcFileName = String.Format("{0}\\{1}.svc", wcfWebsiteFolder, solutionNameParts.Last());
                using (var svcFile = File.CreateText(svcFileName))
                {
                    svcFile.WriteLine("<%@ ServiceHost Language=\"c#\" Factory=\"Microsoft.BizTalk.Adapter.Wcf.Runtime.BasicHttpWebServiceHostFactory, Microsoft.BizTalk.Adapter.Wcf.Runtime, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\" %>");
                    svcFile.Close();
                }

                //create web.config file
                var webconfig = String.Format("{0}\\Web.config", wcfWebsiteFolder);
                using (var webconfigFile = File.CreateText(webconfig))
                {
                    webconfigFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine("  Note: As an alternative to hand editing this file you can use the");
                    webconfigFile.WriteLine("  web admin tool to configure settings for your application. Use");
                    webconfigFile.WriteLine("  the Website->Asp.Net Configuration option in Visual Studio.");
                    webconfigFile.WriteLine("  A full list of settings and comments can be found in");
                    webconfigFile.WriteLine("  machine.config.comments usually located in");
                    webconfigFile.WriteLine("  \\Windows\\Microsoft.Net\\Framework\\v2.x\\Config");
                    webconfigFile.WriteLine("  -->");
                    webconfigFile.WriteLine("<configuration xmlns=\"http://schemas.microsoft.com/.NetConfiguration/v2.0\">");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" The <configSections> section declares handlers for custom configuration sections.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<configSections>");
                    webconfigFile.WriteLine("<section name=\"bizTalkSettings\" type=\"Microsoft.BizTalk.Adapter.Wcf.Runtime.BizTalkConfigurationSection, Microsoft.BizTalk.Adapter.Wcf.Runtime, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\" />");
                    webconfigFile.WriteLine("</configSections>");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" The <bizTalkSettings> section specifies BizTalk specific configuration.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<bizTalkSettings>");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" mexServiceHostFactory debug: Set to \"true\" to launch debugger when MexServiceHostFactory.CreateServiceHost(...) is called by IIS.");
                    webconfigFile.WriteLine(" Used to debug from initial point of activation by IIS. Default value is \"false\" for normal operation.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<mexServiceHostFactory debug=\"false\">");
                    webconfigFile.WriteLine("<receiveLocationMappings><!--add markupFileName=\"*.svc\" receiveLocationName=\"?\" publicBaseAddress=\"protocol://host[:port]\" /-->");
                    webconfigFile.WriteLine("</receiveLocationMappings>");
                    webconfigFile.WriteLine("</mexServiceHostFactory>");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" webServiceHostFactory debug:");
                    webconfigFile.WriteLine(" Set to \"true\" to launch debugger when WebServiceHostFactory.CreateServiceHost(...) is called by IIS.");
                    webconfigFile.WriteLine(" Used to debug from initial point of activation by IIS.");
                    webconfigFile.WriteLine(" Default value is \"false\" for normal operation.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<webServiceHostFactory debug=\"false\" />");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" isolatedReceiver disable:");
                    webconfigFile.WriteLine(" Set to \"true\" to skip IBTTransportProxy.RegisterIsolatedReceiver(...) and IBTTransportProxy.TerminateIsolatedReceiver(...) calls.");
                    webconfigFile.WriteLine(" Used for testing metadata exchange without having to setup receive location.");
                    webconfigFile.WriteLine(" Default value is \"false\" for normal operation.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<isolatedReceiver disable=\"false\" />");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" btsWsdlExporter disable: Set to \"true\" to skip adding BtsWsdlExporter behavior extension to service endpoint.");
                    webconfigFile.WriteLine(" Used for testing or comparing strongly-typed WSDL customization versus weakly-typed WSDL of generic WCF service.");
                    webconfigFile.WriteLine(" Default value is \"false\" for normal operation.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<btsWsdlExporter disable=\"false\" />");
                    webconfigFile.WriteLine("</bizTalkSettings>");
                    webconfigFile.WriteLine("<system.web>");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine("  Set compilation debug=\"true\" to insert debugging symbols into the compiled page.");
                    webconfigFile.WriteLine(" Because this affects performance, set this value to true only during development.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<compilation defaultLanguage=\"c#\" debug=\"false\">");
                    webconfigFile.WriteLine("<assemblies>");
                    webconfigFile.WriteLine("<add assembly=\"mscorlib, version=2.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089\" />");
                    webconfigFile.WriteLine("<add assembly=\"Microsoft.BizTalk.Adapter.Wcf.Common, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\" />");
                    webconfigFile.WriteLine("<add assembly=\"Microsoft.BizTalk.Adapter.Wcf.Runtime, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\" />");
                    webconfigFile.WriteLine("</assemblies>");
                    webconfigFile.WriteLine("</compilation>");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" The <authentication> section enables configuration of the security authentication mode");
                    webconfigFile.WriteLine(" used by ASP.NET to identify an incoming user.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<authentication mode=\"Windows\" />");
                    webconfigFile.WriteLine("<!--");
                    webconfigFile.WriteLine(" The <customErrors> section enables configuration of what to do if/when an unhandled error");
                    webconfigFile.WriteLine(" occurs during the execution of a request. Specifically, it enables developers to configure");
                    webconfigFile.WriteLine(" html error pages to be displayed in place of a error stack trace.");
                    webconfigFile.WriteLine(" -->");
                    webconfigFile.WriteLine("<customErrors mode=\"Off\" />");
                    webconfigFile.WriteLine("</system.web>");
                    webconfigFile.WriteLine("<!-- The <system.serviceModel> section specifies Windows Communication Foundation (WCF) configuration. -->");
                    webconfigFile.WriteLine("<system.serviceModel>");
                    webconfigFile.WriteLine("<behaviors>");
                    webconfigFile.WriteLine("<serviceBehaviors>");
                    webconfigFile.WriteLine("<behavior name=\"ServiceBehaviorConfiguration\">");
                    webconfigFile.WriteLine("<serviceDebug includeExceptionDetailInFaults=\"true\" />");
                    webconfigFile.WriteLine("<serviceMetadata httpGetEnabled=\"true\" /> <!-- Aanpassen naar 'httpsGetEnabled=\"true\"' indien https binding wordt gebruikt -->");
                    webconfigFile.WriteLine("</behavior>");
                    webconfigFile.WriteLine("</serviceBehaviors>");
                    webconfigFile.WriteLine("</behaviors>");
                    webconfigFile.WriteLine("<services>");
                    webconfigFile.WriteLine("<!-- Note: the service name must match the configuration name for the service implementation. -->");
                    webconfigFile.WriteLine("<service name=\"Microsoft.BizTalk.Adapter.Wcf.Runtime.BizTalkServiceInstance\" behaviorConfiguration=\"ServiceBehaviorConfiguration\">");
                    webconfigFile.WriteLine("</service>");
                    webconfigFile.WriteLine("</services>");
                    webconfigFile.WriteLine("</system.serviceModel>");
                    webconfigFile.WriteLine("</configuration>");
                    webconfigFile.Close();
                }

                AddSolutionFoldersAndFiles(wcfWebsiteFolder, deploymentprj);
            }

            if (options.CreateSoapWebsiteFolder && !Directory.Exists(soapWebsiteFolder))
            {
                var soapWebsiteFolderInfo = Directory.CreateDirectory(soapWebsiteFolder);

                var deploymentprj = soln.AddSolutionFolder("WebsiteSoap");

                //create App_Data Folder
                soapWebsiteFolderInfo.CreateSubdirectory("App_Code");
                var dataTypesFileName = String.Format("{0}\\App_Code\\DataTypes.cs", soapWebsiteFolder);
                using (var dataTypesFile = File.CreateText(dataTypesFileName))
                {
                    dataTypesFile.Write("");
                    dataTypesFile.Close();
                }

                //create asmx file
                var asmxFileName = String.Format("{0}\\{1}.asmx", wcfWebsiteFolder, solutionNameParts.Last());
                using (var svcFile = File.CreateText(asmxFileName))
                {
                    svcFile.WriteLine(String.Format("<%@ WebService Language=\"c#\" CodeBehind=\"~/App_Code/{0}.asmx.cs\" Class=\"BizTalkWebService.{0}\" %>", solutionNameParts.Last()));
                    svcFile.Close();
                }

                AddSolutionFoldersAndFiles(soapWebsiteFolder, deploymentprj);
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        #endregion

        private static void ReplaceInTextFile(
            string filePath, string fileName, Dictionary<string, string> replacements, Encoding encodingMode)
        {
            var combinedFilePath = Path.Combine(filePath, fileName);
            var fileContents = File.ReadAllText(combinedFilePath, encodingMode);

            fileContents = replacements.Aggregate(fileContents, (current, replacement) => current.Replace(replacement.Key, replacement.Value));

            File.WriteAllText(combinedFilePath, fileContents, encodingMode);
        }

        // Copy directory structure recursively
        // From a CodeProject article by Richard Lopes  
        private static void CopyDirectory(string src, string dest)
        {
            string[] files;

            if (dest[dest.Length - 1] != Path.DirectorySeparatorChar)
            {
                dest += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            files = Directory.GetFileSystemEntries(src);

            foreach (string element in files)
            {
                // Sub directories
                if (Directory.Exists(element))
                {
                    CopyDirectory(element, dest + Path.GetFileName(element));
                }
                // Files in directory
                else
                {
                    File.Copy(element, dest + Path.GetFileName(element), true);
                }
            }
        }

        private static void AddSolutionFoldersAndFiles(string path, Project solutionFolderProject)
        {

            foreach (var fileInfo in new DirectoryInfo(path).GetFiles())
            {
                solutionFolderProject.ProjectItems.AddFromFile(fileInfo.FullName);
            }

            foreach (var directoryInfo in new DirectoryInfo(path).GetDirectories())
            {
                var deploymentParentFoldername = directoryInfo.Name;
                var deploymentsf = (SolutionFolder)solutionFolderProject.Object;
                var solutionFolderProject2 = deploymentsf.AddSolutionFolder(deploymentParentFoldername);

                AddSolutionFoldersAndFiles(directoryInfo.FullName, solutionFolderProject2);
            }

        }

        private static void UpdateSolutionFile(string deploymentProjectPath, DTE2 dte)
        {

            var deploymentParentFoldername = new DirectoryInfo(deploymentProjectPath).Parent.Name;
            var deploymentFolderFullPath = Path.GetDirectoryName(deploymentProjectPath);

            var soln = (Solution2)dte.Solution;

            var deploymentprj = soln.AddSolutionFolder(deploymentParentFoldername);

            AddSolutionFoldersAndFiles(deploymentFolderFullPath, deploymentprj);
        }

        private void UpdateProjectFile(string projectFilePath, DeploymentOptions options, bool writeOnlyWhenNonDefault)
        {
            XmlDocument projectXml = new XmlDocument();
            projectXml.Load(projectFilePath);

            XmlNamespaceManager xnm = new XmlNamespaceManager(projectXml.NameTable);
            xnm.AddNamespace(string.Empty, "http://schemas.microsoft.com/developer/msbuild/2003");
            xnm.AddNamespace("ns0", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlElement projectElement = projectXml.SelectSingleNode("/ns0:Project", xnm) as XmlElement;
            string bizTalkProductName = GetBizTalkProductName();

            if (string.Compare(bizTalkProductName, "Microsoft BizTalk Server 2010", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(bizTalkProductName, "Microsoft BizTalk Server 2013", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(bizTalkProductName, "Microsoft BizTalk Server 2013 R2", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(bizTalkProductName, "Microsoft BizTalk Server 2016", StringComparison.OrdinalIgnoreCase) == 0
                )
             {
                projectElement.SetAttribute("ToolsVersion", "4.0");
            }

            XmlElement generalPropertyGroup = projectXml.SelectSingleNode("/ns0:Project/ns0:PropertyGroup[1]", xnm) as XmlElement;

            Type doType = options.GetType();
            PropertyInfo[] doProperties = doType.GetProperties();

            foreach (PropertyInfo pi in doProperties)
            {
                var cat = (pi.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault() as CategoryAttribute).Category;

                if (pi.PropertyType == typeof(bool) && cat != "4. DeploymentWizard Options")
                {
                    if (writeOnlyWhenNonDefault)
                    {
                        object[] dvAttribute = pi.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                        DefaultValueAttribute dva = dvAttribute[0] as DefaultValueAttribute;
                        bool defaultValue = (bool)dva.Value;

                        bool propertyValue = (bool)pi.GetValue(options, null);

                        if (defaultValue != propertyValue)
                        {
                            WriteElementText(projectXml, xnm, generalPropertyGroup, pi.Name, propertyValue);
                        }
                    }
                    else
                    {
                        WriteElementText(projectXml, xnm, generalPropertyGroup, pi.Name, (bool)pi.GetValue(options, null));
                    }
                }
            }


            projectXml.Save(projectFilePath);
        }

        private void WriteElementText<T>(
            XmlDocument projectXml, XmlNamespaceManager xnm, XmlElement propertyGroup, string elementName, T elementValue)
        {
            XmlElement xe = propertyGroup.SelectSingleNode("ns0:" + elementName, xnm) as XmlElement;

            if (xe == null)
            {
                xe = projectXml.CreateElement(string.Empty, elementName, xnm.DefaultNamespace);
                xe.InnerText = elementValue.ToString();
                propertyGroup.AppendChild(xe);
            }
            else
            {
                xe.InnerText = elementValue.ToString();
            }
        }

        private string GetBizTalkProductName()
        {
            string bizTalkProduct =
                (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\BizTalk Server\3.0", "ProductName", null);

            if (string.IsNullOrEmpty(bizTalkProduct))
            {
                MessageBox.Show(
                    "Cannot find BizTalk Server install registry key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return bizTalkProduct;
        }
    }
}
