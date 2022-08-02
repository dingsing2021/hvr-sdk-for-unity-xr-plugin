using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Management;
using HVRCORE;

namespace Unity.XR.Huawei
{
    [System.Serializable]
    [XRConfigurationData("Huawei", "Unity.XR.Huawei.Settings")]
    public class HvrsdkSettings : ScriptableObject
    {
        private static string TAG = "HvrsdkSettings";
        public enum StereoRenderingModeAndroid
        {
            MultiPass = 0,
            Multiview = 2,
            Foveated = 3
        }

        public StereoRenderingModeAndroid stereoRenderingModeAndroid;

        public ushort GetStereoRenderingMode()
        {
            return (ushort)stereoRenderingModeAndroid;
        }

        public enum AppDegreesOfFreedom
        {
            _3Dof = 3,
            _6Dof = 6,
            _3_6dof=9
        }

        [SerializeField, Tooltip("Tracking-data app using. The 3dof app only use 3dof data of HMD and controller.")]
        public AppDegreesOfFreedom AppDof = AppDegreesOfFreedom._3Dof;

        public static HvrsdkSettings settings;
		public void Awake()
		{
            HVRLogCore.LOGI(TAG, "Awake");
			settings = this;
		}
    }
}
