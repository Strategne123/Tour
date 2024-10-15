using UnityEngine;

namespace Vrs.Internal
{
    [CreateAssetMenu(fileName = "SettingsAssetConfig", order = 1)]
    public class SettingsAssetConfig : ScriptableObject
    {
        [SerializeField]
        public SleepTimeoutMode mSleepTimeoutMode = SleepTimeoutMode.NEVER_SLEEP;

        [SerializeField]
        public HeadControl mHeadControl = HeadControl.GazeApplication;

        [SerializeField]
        public TextureQuality mTextureQuality = TextureQuality.Best;

        [SerializeField]
        public TextureMSAA mTextureMSAA = TextureMSAA.MSAA_2X;
    }
}
