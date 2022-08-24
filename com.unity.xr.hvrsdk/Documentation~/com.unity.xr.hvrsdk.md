# About the Huawei XR Plugin

<!-- TOC -->

- [About the Huawei XR Plugin](#about-the-huawei-xr-plugin)
  - [Supported devices](#supported-devices)
  - [Supported XR plugin subsystems](#supported-xr-plugin-subsystems)
    - [Display](#display)
    - [Input](#input)
  - [Old SDK API compatibility](#old-sdk-api-compatibility)
  - [XR Management support](#xr-management-support)
- [Known issues](#known-issues)

<!-- /TOC -->

## Supported devices
* HUAWEI VR Glass + Huawei mobile phone(
 [supported phone](https://developer.huawei.com/consumer/cn/doc/development/graphics-Guides/introduction-0000001097358942#section43314313363)).

## Supported XR plugin subsystems

### Display 
* Rendering feature
  * Multi pass 
    * Most widely supported mode. Draw each eye separately.
  * Multiview 
    * Draw left and right eye at the same time with texture2D Array in rednering. It will save CPU load. You may need to modify shader to adapt this mode. More information from [Single Pass Instanced rendering](https://docs.unity3d.com/Manual/SinglePassInstancing.html). 
  * Foveated 
    * This mode will improve display clarity by fixed foveated rendering and it is recommended to enable. It alse use Single Pass Instanced rendering. So you may need to modify shaders. (Unity 2020.3.0 and above)
  * URP compatibility
    * Huawei XR Plugin compatibility of URP conform to Unity XR System. More information, see [Universal Render Pipeline compatibility in XR](https://docs.unity3d.com/Manual/xr-render-pipeline-compatibility.html)


* Graphics APIs 
  * OpenGL ES 3.0  - Supported
  * Vulkan  -  **Not Supported**. Remove Vulkan item from `Project Settings->Player->Graphics APIs`

### Input 
* HMD Tracking 
  * Support 3Dof and 6Dof mode. 

* Controller Tracking - choose one of below for your project
  * Input module of HUAWEI VR SDK For Unity
    * For compatibility, the old input system is also supported. Just import `controller_for_UnityXR.unitypackage`. And drag HVRController and HVREventSystem prefab into your Sence. Then the input module can use as before. \
    You may delete controller object of XR Rig from `XR Rig/Camera Offset/` of sence.\
  ![](images/HVRController.png)
  
  * [Unity XR Input ](https://docs.unity3d.com/Manual/xr_input.html)
    * In planning, will supported in the near feature. 

## Old SDK API compatibility
The API of old sdk [HUAWEI VR SDK For Unity](https://developer.huawei.com/consumer/cn/doc/development/graphics-References/overview-0000001100594564) still be access to apps' C# script. But all the rendering camera related API is not available.
|Class|Compatibility|Description|
|----|---|---|
|`HvrApi`|Not fully supported| GetHvrSdkVersion/GetHelmetHandle/GetControllerHandle
|`HVRArmModel`|Supported| 
|`HVRCamCore`|Not supported| ×
|`HVRDefCore`|Supported|all
|`HVREventListener`|Supported| while using Input module of HUAWEI VR SDK
|`HVRHelpMessage`|Not supported| ×
|`HVRLayoutCore`|Not supported| ×
|`HVRLinePointer`|Supported| while using Input module of HUAWEI VR SDK
|`HVRPluginCore`|Not fully supported| 

## XR Management support

Integration with XR Management isn't required to use the [HUAWEI VR SDK For Unity](https://developer.huawei.com/consumer/cn/doc/development/graphics-Library/unity-sdk-download-0000001142315529) (Unless old input module is used),  You just need to import Huawei XR Plugin then choose the sdk in `XR Plug-in Management`. For more information from [XR Management Documention](https://docs.unity3d.com/Packages/com.unity.xr.management@latest). Check for project script errors if you don't see Hvrsdk checkbox.

![](images/XR_management.png)

# Known issues
