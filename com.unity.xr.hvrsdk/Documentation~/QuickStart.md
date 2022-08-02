# Quick start
It is recommended to read  [Configuring your Unity Project for XR](https://docs.unity3d.com/Manual/configuring-project-for-xr.html) first. 

1. Create a new project with Unity hub.
2. Switch Platform into Android  from `File->Build Settings`.
3. Goto `Edit->Prject Settings->Player->Other Settings`, remove Vulkan item from `Graphics APIS` and set `Minimum API Level` not less than 26.
4. Installing XR Plug-in Management in `Edit->Prject Settings->Player->XR Plug-in Management`.
5. Installing Huawei XR Plugin and select the checkbox in XR Plug-in Management.
6. Import `controller_for_UnityXR.unitypackage` and drag HVRController, HVREventSystem into your sence.
7. Delete default `Main Camera` and add `GameObject->Camera`.Then add `Room-Scale XR Rig` or `Stationary XR Rig` from `GameObject->XR` (The camera will be move into XR Rig aotumatically ). And delete `LeftHand Controller`,`RightHand Controller` in `XR Rig/Camera Offset/`.
8. Finally, you can build the demo and check whether everything is ok.


## Some tips 
Encounter problems, you can check the following:

- Make sure there are no script errors in project before installing Huawei XR plugin.
- Check platforms of Huawei XR plugin libraries select `Android` only.
- In `Edit->Prject Settings->XR Plug-in Management`, Check `Hvrsdk` and `Initialize XR on Startup` selected.
- There are no other camera except `XR Rig/Offset/Your Camera` in root hierarchy of sence.


