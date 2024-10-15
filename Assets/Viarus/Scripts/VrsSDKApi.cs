using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vrs.Internal
{

    public class VrsSDKApi
    {
        private static object syncRoot = new object();

        private static VrsSDKApi _instance = null;

        public static VrsSDKApi Instance
        {
            get
            {
                if (_instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (_instance == null) 
                        {
                            _instance = new VrsSDKApi();
                        }
                    }
                }

                return _instance;
            }
        }

        private VrsSDKApi()
        {
            IsInXRMode = false;
        }

        public bool IsInXRMode { set; get; }

        public void Destroy()
        {
        }

        public bool IsSptMultiThreadedRendering { set; get; }

        public string assetPath = "Assets/Viarus/Resources/Config/";

#if UNITY_EDITOR
        public SettingsAssetConfig GetSettingsAssetConfig()
        {
            var assetpath = assetPath + "SettingsAssetConfig.asset";
            SettingsAssetConfig asset;
            if (System.IO.File.Exists(assetpath))
            {
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<SettingsAssetConfig>(assetpath);
            }
            else
            {
                asset = new SettingsAssetConfig();
                asset.mSleepTimeoutMode = SleepTimeoutMode.NEVER_SLEEP;
                asset.mHeadControl = HeadControl.GazeApplication;
                asset.mTextureQuality = TextureQuality.Best;
                asset.mTextureMSAA = TextureMSAA.MSAA_2X;
                UnityEditor.AssetDatabase.CreateAsset(asset, assetpath);
            }

            return asset;
        }
#endif

        public bool IsSptEyeLocalRp { get; set; }
        public float[] LeftEyeLocalRotation = new float[9];
        public float[] LeftEyeLocalPosition = new float[3];
        public float[] RightEyeLocalRotation = new float[9];
        public float[] RightEyeLocalPosition = new float[3];

    }
}