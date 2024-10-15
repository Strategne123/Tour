
using UnityEngine;
using System.Globalization;
using System.Linq;

namespace Vrs.Internal
{
    public class AndroidDevice : VrsDevice
    {
        AndroidJavaObject viarusVRService = null;

        public override void Init()
        {
            ConnectToActivity();
            base.Init();
            RequsetPermission(new string[]
            {
                VrsGlobal.Permission.WRITE_EXTERNAL_STORAGE,
                VrsGlobal.Permission.READ_EXTERNAL_STORAGE,
                VrsGlobal.Permission.ACCESS_NETWORK_STATE,
                VrsGlobal.Permission.BLUETOOTH,
                VrsGlobal.Permission.BLUETOOTH_ADMIN,
                VrsGlobal.Permission.INTERNET,
            });
        }

        public override long CreateViarusVRService()
        {
            long pointer = 0;
            viarusVRService = androidActivity.Call<AndroidJavaObject>("getVRServiceGL");

            string initParams = viarusVRService.Call<string>("getResultStr");
            Debug.Log("initParams is " + initParams);
            string[] data = initParams.Split('_');
            pointer = long.Parse(data[0]);
            VrsGlobal.supportDtr = int.Parse(data[1]) == 1;
            VrsGlobal.distortionEnabled = int.Parse(data[2]) == 1;
            VrsGlobal.useNvrSo = int.Parse(data[3]) == 1;
            if (data.Length >= 5)
            {
                bool offaxisDistortionEnabled = int.Parse(data[4]) == 1; // Возможно нужно для AR-устройств, но не наш случай. Оставлено чтобы знать за что это поле отвечает
            }

            float fps = VrsGlobal.refreshRate = androidActivity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call<float>("getRefreshRate");
            VrsGlobal.refreshRate = Mathf.Max(60, fps > 0 ? fps : 0);

            VrsGlobal.soVersion = viarusVRService.Call<int>("getNVRParamI", 17);
            VrsSDKApi.Instance.IsSptMultiThreadedRendering = VrsGlobal.soVersion >= 414;
            if(!VrsSDKApi.Instance.IsSptMultiThreadedRendering && SystemInfo.graphicsMultiThreaded)
            {
                AndroidLog("*****Warning******\n\n System Does Not Support Unity MultiThreadedRendering !!! \n\n*****Warning******");
                AndroidLog("Support Unity MultiThreadedRendering Need V2 Version >=414, Currently Is " + VrsGlobal.soVersion + " !!!");
            }

            Debug.Log("AndDev->Service : [pointer]=" + pointer + ", [dtrSpt] =" + VrsGlobal.supportDtr + ", [DistEnabled]=" +
            VrsGlobal.distortionEnabled + ", [useNvrSo]=" + VrsGlobal.useNvrSo + ", [so]=" + VrsGlobal.soVersion
             + ",[fps]=" + VrsGlobal.refreshRate);

            string cardboardParams = viarusVRService.Call<string>("getNVRConfigFullStr");
            if (cardboardParams.Length > 0)
            {
                Debug.Log("cardboardParams is " + cardboardParams);
                string[] profileData = cardboardParams.Split('_');
                for (int i = 0; i < VrsGlobal.dftProfileParams.Length; i++)
                {
                    if (i >= profileData.Length) break;
                    if (profileData[i] == null || profileData[i].Length == 0) continue;
                    VrsGlobal.dftProfileParams[i] = float.Parse(profileData[i], NumberStyles.Any, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                Debug.Log("Vrs->AndroidDevice->getViarusVRConfigFull Failed ! ");
            }
            return pointer;
        }

        public override void SetDisplayQuality(int level)
        {
            viarusVRService.Call("setDisplayQuality", level);
        }

        public override void SetCameraNearFar(float near, float far)
        {
            viarusVRService.Call("setProjectionNearFar", near, far);
        }

        public override void SetIpd(float ipd)
        {
            if (viarusVRService == null) return;
            viarusVRService.Call("setIpd", ipd);
        }

        public override void SetTimeWarpEnable(bool enabled)
        {
            if (viarusVRService == null) return;
            viarusVRService.Call("setTimeWarpEnable", enabled);
        }

        public override void SetSplitScreenModeEnabled(bool enabled)
        {

        }
        public override void AndroidLog(string msg)
        {
            Debug.Log(msg);
        }

        public override void OnApplicationPause(bool pause)
        {
            base.OnApplicationPause(pause);

            if (!pause && androidActivity != null)
            {
                RunOnUIThread(androidActivity, new AndroidJavaRunnable(runOnUiThread));
            }

        }

        public override void AppQuit()
        {
            if (androidActivity != null)
            {
                RunOnUIThread(androidActivity, new AndroidJavaRunnable(() =>
                {
                    androidActivity.Call("finish");
                }));
            }
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
        }

        void runOnUiThread()
        {
            AndroidJavaObject androidWindow = androidActivity.Call<AndroidJavaObject>("getWindow");
            androidWindow.Call("addFlags", 128);
            AndroidJavaObject androidDecorView = androidWindow.Call<AndroidJavaObject>("getDecorView");
            androidDecorView.Call("setSystemUiVisibility", 5894);
        }

        public override void SetIsKeepScreenOn(bool keep)
        {
            if (androidActivity != null)
            {
                RunOnUIThread(androidActivity, new AndroidJavaRunnable(() =>
                {
                    SetScreenOn(keep);
                }));
            }
        }

        void SetScreenOn(bool enable)
        {
            if (enable)
            {
                AndroidJavaObject androidWindow = androidActivity.Call<AndroidJavaObject>("getWindow");
                androidWindow.Call("addFlags", 128);
            }
            else
            {
                AndroidJavaObject androidWindow = androidActivity.Call<AndroidJavaObject>("getWindow");
                androidWindow.Call("clearFlags", 128);
            }
        }

        private static AndroidJavaObject javaArrayFromCS(string[] values)
        {
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance",
                new AndroidJavaClass("java.lang.String"), values.Count());
            for (int i = 0; i < values.Count(); ++i)
            {
                arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
            }

            return arrayObject;
        }

        public void RequsetPermission(string[] names)
        {
            androidActivity.Call("requestOnlyUngainedPermission", javaArrayFromCS(names));
        }
        public override string GetStoragePath() { return GetAndroidStoragePath(); }
    }
}
