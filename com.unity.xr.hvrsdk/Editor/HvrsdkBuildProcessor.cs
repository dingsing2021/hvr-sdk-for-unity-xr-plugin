using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.XR.Huawei;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;

namespace UnityEditor.XR.Huawei
{
    public class HuaweiBuildProcessor : XRBuildHelper<HvrsdkSettings>
    {
        public override string BuildSettingsKey { get { return "Unity.XR.Huawei.Settings"; } }

        private static List<BuildTarget> s_ValidStandaloneBuildTargets = new List<BuildTarget>()
            {
                BuildTarget.Android,
            };

        private bool IsCurrentBuildTargetVaild(BuildReport report)
        {
            return report.summary.platformGroup == BuildTargetGroup.Android ;
        }

        private bool HasLoaderEnabledForTarget(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup != BuildTargetGroup.Android)
                return false;

            XRGeneralSettings settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            if (settings == null)
                return false;

            bool loaderFound = false;
            for (int i = 0; i < settings.Manager.activeLoaders.Count; ++i)
            {
                if (settings.Manager.activeLoaders[i] as HvrsdkLoader != null)
                {
                    loaderFound = true;
                    break;
                }
            }

            return loaderFound;
        }

        private readonly string[] runtimePluginNames = new string[]
        {
                "HVRCORE.dll",
                "libHuaweiXRPlugin.so",
                "libPluginProxy.so",
                "hvrbridge.jar"
        };

        private bool ShouldIncludeRuntimePluginsInBuild(string path, BuildTargetGroup platformGroup)
        {
            return HasLoaderEnabledForTarget(platformGroup);
        }


        /// <summary>OnPreprocessBuild override to provide XR Plugin specific build actions.</summary>
        /// <param name="report">The build report.</param>
        public override void OnPreprocessBuild(BuildReport report)
        {
            if (IsCurrentBuildTargetVaild(report) && HasLoaderEnabledForTarget(report.summary.platformGroup))
                base.OnPreprocessBuild(report);

            var allPlugins = PluginImporter.GetAllImporters();
            foreach (var plugin in allPlugins)
            {
                if (plugin.isNativePlugin)
                {
                    foreach (var pluginName in runtimePluginNames)
                    {
                        if (plugin.assetPath.Contains(pluginName))
                        {
                            plugin.SetIncludeInBuildDelegate((path) => { return ShouldIncludeRuntimePluginsInBuild(path, report.summary.platformGroup); });
                            break;
                        }
                    }
                }
            }
        }
    }

    public static class HuaweiBuildTools
{
    public static bool LoaderPresentInSettingsForBuildTarget(BuildTargetGroup btg)
    {
        var generalSettingsForBuildTarget = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(btg);
        if (!generalSettingsForBuildTarget)
            return false;
        var settings = generalSettingsForBuildTarget.AssignedSettings;
        if (!settings)
            return false;

        bool loaderFound = false;
        for (int i = 0; i < settings.activeLoaders.Count; ++i)
        {
            if (settings.activeLoaders[i] as HvrsdkLoader != null)
            {
                loaderFound = true;
                break;
            }
        }

        return loaderFound;
    }

    public static HvrsdkSettings GetSettings()
    {
        HvrsdkSettings settings = null;
#if UNITY_EDITOR
        UnityEditor.EditorBuildSettings.TryGetConfigObject<HvrsdkSettings>("Unity.XR.Huawei.Settings", out settings);
#else
            settings = HvrsdkSettings.settings;
#endif
            return settings;
    }
}


    internal class HuaweiPrebuildSettings : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!HuaweiBuildTools.LoaderPresentInSettingsForBuildTarget(report.summary.platformGroup))
                return;

            if (report.summary.platformGroup == BuildTargetGroup.Android)
            {
                GraphicsDeviceType firstGfxType = PlayerSettings.GetGraphicsAPIs(report.summary.platform)[0];
                if (firstGfxType != GraphicsDeviceType.OpenGLES3 && firstGfxType != GraphicsDeviceType.OpenGLES2)
                {
                    throw new BuildFailedException("OpenGLES2 and OpenGLES3 are currently the only graphics APIs compatible with the Huawei XR Plugin on mobile platforms.");
                }
                if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel26)
                {
                    throw new BuildFailedException("Android Minimum API Level must be set to 26 or higher for the Huawei XR Plugin.");
                }
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3 });
            }
        }
    }

#if UNITY_ANDROID
internal class HvrsdkManifest : IPostGenerateGradleAndroidProject
{
    static readonly string k_AndroidURI = "http://schemas.android.com/apk/res/android";
    static readonly string k_AndroidManifestPath = "/src/main/AndroidManifest.xml";

    void UpdateOrCreateAttributeInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
    {
        var xmlNode = doc.SelectSingleNode(parentPath + "/" + tag);

        if (xmlNode != null)
        {
            ((XmlElement)xmlNode).SetAttribute(name, k_AndroidURI, value);
        }
    }

    void UpdateOrCreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
        string firstName, string firstValue, string secondName, string secondValue)
    {
        var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

        foreach (XmlNode node in xmlNodeList)
        {
            var attributeList = ((XmlElement)node).Attributes;

            foreach (XmlAttribute attrib in attributeList)
            {
                if (attrib.Value == firstValue)
                {
                    XmlAttribute valueAttrib = attributeList[secondName, k_AndroidURI];
                    if (valueAttrib != null)
                    {
                        valueAttrib.Value = secondValue;
                    }
                    else
                    {
                        ((XmlElement)node).SetAttribute(secondName, k_AndroidURI, secondValue);
                    }
                    return;
                }
            }
        }

        // Didn't find any attributes that matched, create both (or all three)
        XmlElement childElement = doc.CreateElement(tag);
        childElement.SetAttribute(firstName, k_AndroidURI, firstValue);
        childElement.SetAttribute(secondName, k_AndroidURI, secondValue);

        var xmlParentNode = doc.SelectSingleNode(parentPath);

        if (xmlParentNode != null)
        {
            xmlParentNode.AppendChild(childElement);
        }
    }

    // same as above, but don't create if the node already exists
    void CreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
        string firstName, string firstValue, string secondName = null, string secondValue = null, string thirdName = null, string thirdValue = null)
    {
        var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

        // don't create if the firstValue matches
        foreach (XmlNode node in xmlNodeList)
        {
            foreach (XmlAttribute attrib in node.Attributes)
            {
                if (attrib.Value == firstValue)
                {
                    return;
                }
            }
        }

        XmlElement childElement = doc.CreateElement(tag);
        childElement.SetAttribute(firstName, k_AndroidURI, firstValue);

        if (secondValue != null)
        {
            childElement.SetAttribute(secondName, k_AndroidURI, secondValue);
        }

        if (thirdValue != null)
        {
            childElement.SetAttribute(thirdName, k_AndroidURI, thirdValue);
        }

        var xmlParentNode = doc.SelectSingleNode(parentPath);

        if (xmlParentNode != null)
        {
            xmlParentNode.AppendChild(childElement);
        }
    }

    void RemoveNameValueElementInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
    {
        var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

        foreach (XmlNode node in xmlNodeList)
        {
            var attributeList = ((XmlElement)node).Attributes;

            foreach (XmlAttribute attrib in attributeList)
            {
                if (attrib.Name == name && attrib.Value == value)
                {
                    node.ParentNode?.RemoveChild(node);
                }
            }
        }
    }

    void AddProGuardRule(string path)
    {
    }

    public void OnPostGenerateGradleAndroidProject(string path)
{
    if (!HuaweiBuildTools.LoaderPresentInSettingsForBuildTarget(BuildTargetGroup.Android))
        return;

    AddProGuardRule(path);

    var manifestPath = path + k_AndroidManifestPath;
    var manifestDoc = new XmlDocument();
    manifestDoc.Load(manifestPath);

    var sdkVersion = (int)PlayerSettings.Android.minSdkVersion;

    UpdateOrCreateAttributeInTag(manifestDoc, "/", "manifest", "installLocation", "auto");

    var nodePath = "/manifest/application";
    UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "com.huawei.android.vr.application.mode", "value", "vr_only");

    string appDofValue = HuaweiBuildTools.GetSettings().AppDof == HvrsdkSettings.AppDegreesOfFreedom._6Dof ? "6dof" : "3dof";

    if(HuaweiBuildTools.GetSettings().AppDof == HvrsdkSettings.AppDegreesOfFreedom._3_6dof)
     appDofValue ="3dof|6dof";

    UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "com.huawei.vr.application.freeDegree", "value", appDofValue);

    nodePath = "/manifest/application/activity/intent-filter";
    CreateNameValueElementsInTag(manifestDoc, nodePath, "action", "name", "com.huawei.android.vr.action.MAIN");
    CreateNameValueElementsInTag(manifestDoc, nodePath, "category", "name", "android.intent.category.DEFAULT");

    nodePath = "/manifest";
    CreateNameValueElementsInTag(manifestDoc, nodePath, "uses-permission", "name", "com.huawei.android.permission.VR");
    CreateNameValueElementsInTag(manifestDoc, nodePath, "uses-permission", "name", "com.huawei.vrhandle.permission.DEVICE_MANAGER");
    CreateNameValueElementsInTag(manifestDoc, nodePath, "uses-permission", "name", "android.permission.WRITE_EXTERNAL_STORAGE");

    manifestDoc.Save(manifestPath);
}

    public int callbackOrder { get { return 10000; } }
}
#endif
}
