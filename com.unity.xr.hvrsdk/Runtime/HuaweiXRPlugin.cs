using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Unity.XR.Huawei
{
    public class HXRPlugin
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct UserDefinedSettings
        {
            public ushort stereoRenderingMode;
        }

        internal const string m_HWVRLibName = "HuaweiXRPlugin";
        [DllImport(m_HWVRLibName)]
        internal static extern void HXR_SetInitVariables(IntPtr activity);

        [DllImport(m_HWVRLibName)]
        internal static extern void HXR_SetUserDefinedSettings(UserDefinedSettings settings);
    }
}
