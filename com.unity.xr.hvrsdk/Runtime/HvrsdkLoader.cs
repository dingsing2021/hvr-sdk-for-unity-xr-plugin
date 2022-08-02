using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR;
#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
#endif

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
#endif
using HVRCORE;


namespace Unity.XR.Huawei
{
    public class HvrsdkLoader : XRLoaderHelper
    {
        private static string TAG = "HvrsdkLoader";
        
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors =
            new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors =
            new List<XRInputSubsystemDescriptor>();
        
        public override bool Initialize()
        {
            HVRLogCore.GetAndroidLogClass();
            HVRLogCore.LOGI(TAG, "Initialize");
            HvrsdkSettings settings = GetSettings();
            if (settings != null) {
                HXRPlugin.UserDefinedSettings userDefinedSettings;
                userDefinedSettings.stereoRenderingMode = settings.GetStereoRenderingMode();
                HXRPlugin.HXR_SetUserDefinedSettings(userDefinedSettings);
            }
            else{
                HVRLogCore.LOGI(TAG, "settings == null");
            }

            LoadHVRSDK();
            InitController();
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "huawei display");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "huawei input");
            return true;
        }
        
        public override bool Start()
        {
            HVRLogCore.LOGI(TAG, "Start");
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
            return true;
        }

        public override bool Stop()
        {
            HVRLogCore.LOGI(TAG, "Stop");
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            HVRLogCore.LOGI(TAG, "Deinitialize");
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            return true;
        }

        public HvrsdkSettings GetSettings()
        {
            HVRLogCore.LOGI(TAG, "GetSettings");
            return HvrsdkSettings.settings;
        }
        static void LoadHVRSDK()
        {
            HVRLogCore.LOGI(TAG, "LoadHVRSDK");
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject jo = new AndroidJavaObject("com.huawei.hvr.LibUpdateClient", context);
            jo.Call<bool>("runUpdate");
            HXRPlugin.HXR_SetInitVariables(activity.GetRawObject());
        }

        private static void InitController()
        {
            IControllerHandle controler = HvrApi.GetControllerHandle();
            if (controler == null) {
                HVRLogCore.LOGE(TAG, "controler handle is null!");
            }
            Application.onBeforeRender += Update;
        }

        private static void Update()
        {
            HVRCOREProxy.TryUpdateControllerData();
        }

    }
}
