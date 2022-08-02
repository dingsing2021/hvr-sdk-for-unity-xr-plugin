#if XR_MGMT_GTE_320

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using HVRCORE;

namespace Unity.XR.Huawei.Editor
{
    internal class HvrsdkMetadata : IXRPackage
    {
        private static string TAG = "HvrsdkMetadata";
        private class HvrsdkPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }

        }

        private class HvrsdkLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        private static IXRPackageMetadata s_Metadata = new HvrsdkPackageMetadata() {
            packageName = "Huawei XR Plugin",
            packageId = "com.unity.xr.huawei",
            settingsType = "Unity.XR.Huawei.HvrsdkSettings",
            loaderMetadata = new List<IXRLoaderMetadata>() {
                new HvrsdkLoaderMetadata() {
                        loaderName = "Hvrsdk",
                        loaderType = "Unity.XR.Huawei.HvrsdkLoader",
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Android
                        }
                    },
                }
        };
        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            HvrsdkSettings settings = obj as HvrsdkSettings;
            if (settings != null)
            {
                return true;
            }

            return false;
        }
    }
}

#endif // XR_MGMT_GTE_320
