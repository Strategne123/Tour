
namespace Vrs.Internal
{

    public class VrsGlobal
    {
        public class Permission
        {
            public static string CAMERA = "android.permission.CAMERA";
            public static string WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
            public static string READ_EXTERNAL_STORAGE = "android.permission.READ_EXTERNAL_STORAGE";
            public static string ACCESS_COARSE_LOCATION = "android.permission.ACCESS_COARSE_LOCATION";
            public static string ACCESS_NETWORK_STATE = "android.permission.ACCESS_NETWORK_STATE";
            public static string WRITE_SETTINGS = "android.permission.WRITE_SETTINGS";
            public static string BLUETOOTH = "android.permission.BLUETOOTH";
            public static string BLUETOOTH_ADMIN = "android.permission.BLUETOOTH_ADMIN";
            public static string INTERNET = "android.permission.INTERNET";
            public static string GET_TASKS = "android.permission.GET_TASKS";
            public static string RECORD_AUDIO = "android.permission.RECORD_AUDIO";
            public static string READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
        }

        public static bool hasInfinityARSDK;

        public static float defaultGazeDistance = 50;

        public static bool nvrStarted = false;

        public static bool supportDtr = false;

        public static bool useNvrSo = false;

        public static bool distortionEnabled = false;

        public static string offaxisDistortionConfigData;

        public static string sdkConfigData;

        public static float refreshRate = -1;

        public static float[] dftProfileParams = new float[21];

        public static float fovNear = -1;
        public static float fovFar = -1;

        public static int soVersion = -1;

        public static float focusObjectDistance = defaultGazeDistance;

        public static bool DEBUG_LOG_ENABLED = false;
    }

    public enum GazeTag
    {
        Show = 0,
        Hide = 1,
        Set_Distance = 2,
        Set_Size = 3,
        Set_Color = 4
    }

    public enum GazeSize
    {
        Original = 0,
        Large = 1,
        Medium = 2,
        Small = 3
    }

    public enum SleepTimeoutMode
    {
        NEVER_SLEEP = 0, 
        SYSTEM_SETTING = 1
    }

    public enum TextureQuality
    {
        Simple = 2,
        Good = 0,
        Better = 3,
        Best = 1
    }

    public enum TextureMSAA
    {
        NONE = 1,
        MSAA_2X = 2,
        MSAA_4X = 4,
        MSAA_8X = 8
    }

    public enum FrameRate
    {
        FPS_60 = 270,
        FPS_72 = 72,
        FPS_75 = 75,
        FPS_90 = 90
    }

    public enum DISPLAY_MODE
    {
        MODE_2D = 0,
        MODE_3D = 1
    }

    public enum HeadControl
    {
        GazeSystem = 0,
        GazeApplication = 1,
        Hover = 2,
        Controller=3
    }
}
