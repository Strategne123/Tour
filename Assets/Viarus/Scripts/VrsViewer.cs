using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using Mirror;

namespace Vrs.Internal
{
    [AddComponentMenu("VRS/VrsViewer")]
    public class VrsViewer : MonoBehaviour
    {
        public const string VRS_SDK_VERSION = "2.5.3.0_20230109";
        public const bool IsAndroidKillProcess = true;
        public static bool USE_DTR = true;
        private static int _texture_count = 6;
        public static int kPreRenderEvent = 1;
        public UnityAction OnUpdateActionHandler;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public static VrsViewer Instance
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
                USE_DTR = false;
                if (instance == null && !Application.isPlaying)
                {
                    instance = FindObjectOfType<VrsViewer>();
                }
#endif
                return instance;
            }
        }

        private static VrsViewer instance = null;
        public VrsEye[] eyes = new VrsEye[2];

        public static void Create()
        {
            if (instance == null && FindObjectOfType<VrsViewer>() == null)
            {
                var go = new GameObject("VrsViewerMain", typeof(VrsViewer));
                go.transform.localPosition = Vector3.zero;
            }
        }

        public VrsStereoController Controller
        {
            get
            {
                if (currentController == null)
                {
                    currentController = FindObjectOfType<VrsStereoController>();
                }
                return currentController;
            }
        }

        private VrsStereoController currentController;

        public bool SplitScreenModeEnabled
        {
            get { return splitScreenModeEnabled; }
            set
            {
                if (value != splitScreenModeEnabled && device != null)
                {
                    device.SetSplitScreenModeEnabled(value);
                }
                splitScreenModeEnabled = value;
            }
        }

        [SerializeField] private bool splitScreenModeEnabled = true;

        public HeadControl HeadControl
        {
            get { return headControlEnabled; }
            set
            {
                headControlEnabled = value;
                UpdateHeadControl();
            }
        }

        [SerializeField] private HeadControl headControlEnabled = HeadControl.GazeApplication;

        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        [SerializeField] private float duration = 2;

        VrsReticle mVrsReticle;

        public VrsReticle GetVrsReticle()
        {
            InitVrsReticleScript();
            return mVrsReticle;
        }

        public void DismissReticle()
        {
            GetVrsReticle().Dismiss();
        }

        public void ShowReticle()
        {
            GetVrsReticle().Show();
        }

        private void InitVrsReticleScript()
        {
            if (mVrsReticle == null)
            {
                GameObject vrsReticleObject = GameObject.Find("VrsReticle");
                if (vrsReticleObject != null)
                {
                    mVrsReticle = vrsReticleObject.GetComponent<VrsReticle>();
                }
            }
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        [SerializeField]
        private FrameRate targetFrameRate = FrameRate.FPS_60;
        public FrameRate TargetFrameRate
        {
            get
            {
                return targetFrameRate;
            }
            set
            {
                if (value != targetFrameRate)
                {
                    targetFrameRate = value;
                }
            }
        }

#endif

        [SerializeField] public TextureMSAA textureMsaa = TextureMSAA.MSAA_2X;

        public TextureMSAA TextureMSAA
        {
            get { return textureMsaa; }
            set
            {
                if (value != textureMsaa)
                {
                    textureMsaa = value;
                }
            }
        }

        [SerializeField] private bool trackerPosition = true;

        [Serializable]
        public class VrsSettings
        {
            [Tooltip("Change Timewarp Status")] public int timewarpEnabled = -1; 
            [Tooltip("Change Sync Frame Status")] public bool syncFrameEnabled = false;
        }

        [SerializeField] public VrsSettings settings = new VrsViewer.VrsSettings();

        [SerializeField] private bool showFPS = false;

        public bool ShowFPS
        {
            get { return showFPS; }
            set
            {
                if (value != showFPS)
                {
                    showFPS = value;
                }
            }
        }

        [SerializeField] public TextureQuality textureQuality = TextureQuality.Better;

        public TextureQuality TextureQuality
        {
            get { return textureQuality; }
            set
            {
                if (value != textureQuality)
                {
                    textureQuality = value;
                }
            }
        }

        [SerializeField] private bool requestLock = false;

        public bool LockHeadTracker
        {
            get { return requestLock; }
            set
            {
                if (value != requestLock)
                {
                    requestLock = value;
                }
            }
        }

        public bool InitialRecenter { get; set; }

        public void RequestLock()
        {
            if (device != null)
            {
                device.NLockTracker();
            }
        }

        public void RequestUnLock()
        {
            if (device != null)
            {
                device.NUnLockTracker();
            }
        }

        public void SwitchControllerMode(bool enabled)
        {

            if (enabled)
            {
                HeadControl = HeadControl.Controller;
            }
            else
            {
                HeadControl = HeadControl.GazeApplication;
            }
        }

        private void SwitchApplicationReticle(bool enabled)
        {
            InitVrsReticleScript();

            bool IsControllerMode = HeadControl == HeadControl.Controller;

            if (enabled)
            {
                if (mVrsReticle != null) mVrsReticle.Show();
                GazeInputModule.gazePointer = mVrsReticle;
            }
            else if (!enabled && IsControllerMode)
            {
                if (mVrsReticle != null)
                {
                    mVrsReticle.Dismiss();
                }
                GazeInputModule.gazePointer = null;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX

        public VrsProfile.ScreenSizes ScreenSize
        {
            get { return screenSize; }
            set
            {
                if (value != screenSize)
                {
                    screenSize = value;
                    if (device != null)
                    {
                        device.UpdateScreenData();
                    }
                }
            }
        }

        [SerializeField] private VrsProfile.ScreenSizes screenSize = VrsProfile.ScreenSizes.Nexus5;

        public VrsProfile.ViewerTypes ViewerType
        {
            get { return viewerType; }
            set
            {
                if (value != viewerType)
                {
                    viewerType = value;
                    if (device != null)
                    {
                        device.UpdateScreenData();
                    }
                }
            }
        }

        [SerializeField] private VrsProfile.ViewerTypes viewerType = VrsProfile.ViewerTypes.CardboardMay2015;
#endif

        private static BaseARDevice device;

        public RenderTexture GetStereoScreen(int eye)
        {
            if (eyeStereoScreens[0] == null)
            {
                InitEyeStereoScreens();
            }

            if (Application.isEditor || (VrsViewer.USE_DTR && !VrsGlobal.supportDtr))
            {
                return eyeStereoScreens[0];
            }
            return eyeStereoScreens[eye + _current_texture_index];
        }

        public RenderTexture[] eyeStereoScreens = new RenderTexture[_texture_count];

        private void InitEyeStereoScreens()
        {
            InitEyeStereoScreens(-1, -1);
        }

        private void InitEyeStereoScreens(int width, int height)
        {
            RealeaseEyeStereoScreens();
            bool useDFT = VrsViewer.USE_DTR && !VrsGlobal.supportDtr;
            if (!USE_DTR || useDFT || IsWinPlatform)
            {
                RenderTexture rendetTexture = device.CreateStereoScreen(width, height);
                if (!rendetTexture.IsCreated())
                {
                    rendetTexture.Create();
                }
                int tid = (int) rendetTexture.GetNativeTexturePtr();
                for (int i = 0; i < _texture_count; i++)
                {
                    eyeStereoScreens[i] = rendetTexture;
                    _texture_ids[i] = tid;
                }
            }
            else
            {
                for (int i = 0; i < _texture_count; i++)
                {
                    eyeStereoScreens[i] = device.CreateStereoScreen(width, height);
                    eyeStereoScreens[i].Create();
                    _texture_ids[i] = (int) eyeStereoScreens[i].GetNativeTexturePtr();
                }
            }
        }

        private void RealeaseEyeStereoScreens()
        {
            for (int i = 0; i < _texture_count; i++)
            {
                if (eyeStereoScreens[i] != null)
                {
                    eyeStereoScreens[i].Release();
                    eyeStereoScreens[i] = null;
                    _texture_ids[i] = 0;
                }
            }
            Resources.UnloadUnusedAssets();
        }

        public VrsProfile Profile
        {
            get { return device.Profile; }
        }

        public enum Eye
        {
            Left,
            Right,
            Center 
        }

        public enum Distortion
        {
            Distorted,
            Undistorted 
        }

        public Pose3D HeadPose
        {
            get { return device.GetHeadPose(); }
        }

        public Matrix4x4 Projection(Eye eye, Distortion distortion = Distortion.Distorted)
        {
            return device.GetProjection(eye, distortion);
        }

        public Rect Viewport(Eye eye, Distortion distortion = Distortion.Distorted)
        {
            return device.GetViewport(eye, distortion);
        }

        private void InitDevice()
        {
            if (device != null)
            {
                device.Destroy();
            }
            device = BaseARDevice.GetDevice();
            device.Init();
            device.SetSplitScreenModeEnabled(splitScreenModeEnabled);
            device.UpdateScreenData();
        }

        public bool IsWinPlatform { get; set; }

        void Awake()
        {
            SettingsAssetConfig asset;
#if UNITY_EDITOR
            asset = VrsSDKApi.Instance.GetSettingsAssetConfig();
#else
            asset = Resources.Load<SettingsAssetConfig>("Config/SettingsAssetConfig");
#endif
            sleepTimeoutMode = asset.mSleepTimeoutMode;
            headControlEnabled = asset.mHeadControl;
            textureQuality = asset.mTextureQuality;
            textureMsaa = asset.mTextureMSAA;
            InitialRecenter = true;
            IsWinPlatform = false;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            IsWinPlatform = true;
#endif
            if (instance == null)
            {
                instance = this;

                if (Application.isMobilePlatform)
                {
                    QualitySettings.antiAliasing = 0;
                    Application.runInBackground = false;
                    Input.gyro.enabled = false;
                    if (SleepMode == SleepTimeoutMode.NEVER_SLEEP)
                    {
                        Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    }
                    else
                    {
                        Screen.sleepTimeout = SleepTimeout.SystemSetting;
                    }
                }
            }
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            InitDevice();
            if (!IsWinPlatform && !VrsGlobal.supportDtr)
            {
                AddPrePostRenderStages();
            }
#if UNITY_ANDROID

            int targetFrameRate = Application.platform == RuntimePlatform.Android
                ? ((int) VrsGlobal.refreshRate > 0 ? (int) VrsGlobal.refreshRate : 590)
                : 560;
            Application.targetFrameRate = targetFrameRate;
#endif
            if (Application.platform != RuntimePlatform.Android)
            {
                VrsSDKApi.Instance.IsInXRMode = true;
            }
            if (!VrsGlobal.supportDtr)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            if (IsWinPlatform)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = (int)targetFrameRate;
                Application.runInBackground = true;
                QualitySettings.maxQueuedFrames = -1;
                QualitySettings.antiAliasing = Mathf.Max(QualitySettings.antiAliasing, (int)TextureMSAA);
            }
#endif
            device.AndroidLog("Welcome to use Unity VRS SDK , current SDK VERSION is " + VRS_SDK_VERSION
                              + ", s " + VrsGlobal.soVersion + ", u " + Application.unityVersion + ", fps " +
                              Application.targetFrameRate + ", vsync "
                              + QualitySettings.vSyncCount + ", antiAliasing : " +
                              QualitySettings.antiAliasing);
            AddStereoControllerToCameras();
        }

        void Start()
        {
            if (!IsWinPlatform)
            {
                if (eyeStereoScreens[0] == null)
                {

                    InitEyeStereoScreens();
                    device.SetTextureSizeNative(eyeStereoScreens[0].width, eyeStereoScreens[1].height);
                }
            }
            UpdateHeadControl();
        }

        public void UpdateHeadControl()
        {
            switch (HeadControl)
            {
                case HeadControl.GazeApplication:
                    SwitchApplicationReticle(true);
                    break;
                case HeadControl.GazeSystem:
                    SwitchApplicationReticle(false);
                    break;
                case HeadControl.Hover:
                    SwitchApplicationReticle(true);
                    break;
                case HeadControl.Controller:
                    SwitchApplicationReticle(false);
                    break;
            }
        }

        private VrsHead head;

        public VrsHead GetHead()
        {
            if (head == null && Controller != null)
            {
                head = Controller.Head;
            }
            if (head == null)
            {
                head = FindObjectOfType<VrsHead>();
            }
            return head;
        }

        private void Update()
        {
            UpdateHeadControl();
            UpdateState();
            UpdateEyeTexture();
            if (Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp((KeyCode) 10) || Input.GetMouseButtonUp(0))
            {
                Triggered = true;
            }
            if (OnUpdateActionHandler!=null)
            {
                OnUpdateActionHandler();
            }
            if (GazeInputModule.gazePointer != null)
            {
                GazeInputModule.gazePointer.UpdateStatus();
            }
        }

        public BaseARDevice GetDevice()
        {
            return device;
        }

        public void AndroidLog(string msg)
        {
            if (device != null)
            {
                device.AndroidLog(msg);
            }
        }

        public void UpdateHeadPose()
        {
            if (device != null && VrsSDKApi.Instance.IsInXRMode)
                device.UpdateState();
        }

        public void UpdateEyeTexture()
        {
            if (USE_DTR && VrsGlobal.supportDtr)
            {
                SwapBuffers();
                VrsEye[] eyes = VrsViewer.Instance.eyes;
                for (int i = 0; i < 2; i++)
                {
                    VrsEye eye = eyes[i];
                    if (eye != null)
                    {
                        eye.UpdateTargetTexture();
                    }
                }
            }
        }

        void AddPrePostRenderStages()
        {
            var preRender = FindObjectOfType<VrsPreRender>();
            if (preRender == null)
            {
                var go = new GameObject("PreRender", typeof(VrsPreRender));
                go.SendMessage("Reset");
                go.transform.parent = transform;
            }
            var postRender = FindObjectOfType<VrsPostRender>();
            if (postRender == null)
            {
                var go = new GameObject("PostRender", typeof(VrsPostRender));
                go.SendMessage("Reset");
                go.transform.parent = transform;
            }
        }

        public bool Triggered { get; set; }
        public bool ProfileChanged { get; private set; }
        private int updatedToFrame = 0;

        public void UpdateState()
        {
            if (updatedToFrame != Time.frameCount)
            {
                updatedToFrame = Time.frameCount;
                DispatchEvents();
                if (NeedUpdateNearFar && device != null && device.viarusVRServiceId != 0)
                {
                    float far = GetCameraFar();
                    float mNear = 0.0305f;
                    if (VrsGlobal.fovNear > -1)
                    {
                        mNear = VrsGlobal.fovNear;
                    }
                    device.SetCameraNearFar(mNear, far);
                    Instance.NeedUpdateNearFar = false;
                    for (int i = 0; i < 2; i++)
                    {
                        VrsEye eye = eyes[i];
                        if (eye != null)
                        {
                            if (eye.cam.farClipPlane < VrsGlobal.fovFar)
                            {
                                eye.cam.farClipPlane = VrsGlobal.fovFar;
                            }
                        }
                    }
                }
            }
        }

        int[] lastKeyAction;

        private void DispatchEvents()
        {
            if (device == null) return;
            ProfileChanged = device.profileChanged;
            if (device.profileChanged)
            {
                if (VrsOverrideSettings.OnProfileChangedEvent != null) VrsOverrideSettings.OnProfileChangedEvent();
                device.profileChanged = false;
            }
        }

        public void Recenter()
        {
            device.Recenter();
        }

        public static void AddStereoControllerToCameras()
        {
            for (int i = 0; i < Camera.allCameras.Length; i++)
            {
                Camera camera = Camera.allCameras[i];
                if (
                    (camera.tag == "MainCamera")
                    && camera.targetTexture == null &&
                    camera.GetComponent<VrsStereoController>() == null &&
                    camera.GetComponent<VrsEye>() == null &&
                    camera.GetComponent<VrsPreRender>() == null &&
                    camera.GetComponent<VrsPostRender>() == null)
                {
                    camera.gameObject.AddComponent<VrsStereoController>();
                }
            }
        }

        void OnEnable()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            if (device == null)
            {
                InitDevice();
            }
#endif
            device.OnPause(false);
            StartCoroutine("EndOfFrame");
        }

        void OnDisable()
        {
            //device.OnPause(true);
            //StopCoroutine("EndOfFrame");
        }

        private Coroutine onResume = null;

        void OnPause()
        {
            onResume = null;
            device.OnApplicationPause(true);
        }

        IEnumerator OnResume()
        {
            yield return new WaitForSeconds(1.0f);

            if (VrsGlobal.supportDtr)
            {
                InitVrsReticleScript();
                UpdateHeadControl();
            }
            device.OnApplicationPause(false);
        }

        public void SetPause(bool pause)
        {
            if (pause)
            {
                OnPause();
            }
            else if (onResume == null)
            {
                onResume = StartCoroutine(OnResume());
            }
        }

        void OnApplicationPause(bool pause)
        {
            SetPause(pause);
        }

        void OnApplicationFocus(bool focus)
        {
           device.OnFocus(focus);
        }

        void OnApplicationQuit()
        {
            StopAllCoroutines();
            device.OnApplicationQuit();
            if (VrsOverrideSettings.OnApplicationQuitEvent != null)
            {
                VrsOverrideSettings.OnApplicationQuitEvent();
            }

#if UNITY_ANDROID && !UNITY_EDITOR
			if(IsAndroidKillProcess) 
            {
                 VrsSDKApi.Instance.Destroy();
                 System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
#endif
        }

        public void AppQuit()
        {
            device.AppQuit();
        }

        void OnDestroy()
        {
            this.SplitScreenModeEnabled = false;
            if (device != null)
            {
                device.Destroy();
            }
            if (instance == this)
            {
                instance = null;
            }
        }

        public void ResetHeadTrackerFromAndroid()
        {
            if (instance != null && device != null)
            {
                Recenter();
            }
        }

        void OnActivityPause()
        {
        }

        void OnActivityResume()
        {
        }

        public void SetSystemSplitMode(int flag)
        {
            device.NSetSystemSplitMode(flag);
        }

        private int[] _texture_ids = new int[_texture_count];
        private int _current_texture_index, _next_texture_index;

        public bool SwapBuffers()
        {
            bool ret = true;
            for (int i = 0; i < _texture_count; i++)
            {
                if (!eyeStereoScreens[i].IsCreated())
                {
                    eyeStereoScreens[i].Create();
                    _texture_ids[i] = (int) eyeStereoScreens[i].GetNativeTexturePtr();
                    ret = false;
                }
            }

            _current_texture_index = _next_texture_index;
            _next_texture_index = (_next_texture_index + 2) % _texture_count;
            return ret;
        }

        public int GetEyeTextureId(int eye)
        {
            return _texture_ids[_current_texture_index + (int) eye];
        }

        public int GetTimeWarpViewNum()
        {
            return device.GetTimewarpViewNumber();
        }

        public List<GameObject> GetAllObjectsInScene()
        {
            GameObject[] pAllObjects = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));
            List<GameObject> pReturn = new List<GameObject>();
            foreach (GameObject pObject in pAllObjects)
            {
                if (pObject == null || !pObject.activeInHierarchy || pObject.hideFlags == HideFlags.NotEditable ||
                    pObject.hideFlags == HideFlags.HideAndDontSave)
                {
                    continue;
                }

                pReturn.Add(pObject);
            }
            return pReturn;
        }

        public Texture2D createTexture2D(RenderTexture renderTexture)
        {
            int width = renderTexture.width;
            int height = renderTexture.height;
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture2D.Apply();
            return texture2D;
        }

        private int frameCount = 0;

        [Obsolete]
        private void EndOfFrameCore()
        {
            if (USE_DTR && (!VrsSDKApi.Instance.IsInXRMode && frameCount < 3))
            {
                frameCount++;
                GL.Clear(false, true, Color.black);
            }
            else
            {
                frameCount++;
                if (USE_DTR && VrsGlobal.supportDtr)
                {
                    if (settings.timewarpEnabled >= 0 && frameCount > 0 && frameCount < 10)
                    {
                        device.SetTimeWarpEnable(false);
                    }
                }
            }
            bool IsHeadPoseUpdated = device.IsHeadPoseUpdated();
            if (USE_DTR && VrsGlobal.supportDtr && IsHeadPoseUpdated)
                VrsPluginEvent.IssueWithData(ViarusRenderEventType.TimeWarp, VrsViewer.Instance.GetTimeWarpViewNum());
        }

        [Obsolete]
        IEnumerator EndOfFrame()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                EndOfFrameCore();
            }
        }

        public int GetFrameId()
        {
            return frameCount;
        }

        private float mFar = -1;
        private bool needUpdateNearFar = false;

        public void UpateCameraFar(float far)
        {
            mFar = far;
            needUpdateNearFar = true;
            VrsGlobal.fovFar = far;
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {

                Camera.main.farClipPlane = far;
            }
        }

        public float GetCameraFar()
        {
            return mFar;
        }

        public bool NeedUpdateNearFar
        {
            get { return needUpdateNearFar; }
            set
            {
                if (value != needUpdateNearFar)
                {
                    needUpdateNearFar = value;
                }
            }
        }

        private float oldFov = -1;

        private Matrix4x4[] eyeOriginalProjection = null;

        public void UpdateEyeCameraProjection(Eye eye)
        {
            if (oldFov != -1 && eye == Eye.Right)
            {
                UpdateCameraFov(oldFov);
            }
            if (!Application.isEditor && device != null && eye == Eye.Right)
            {
                if (mFar > 0)
                {
                    float mNear = 0.0305f;
                    if (VrsGlobal.fovNear > -1)
                    {
                        mNear = VrsGlobal.fovNear;
                    }
                    float fovLeft = mNear * Mathf.Tan(-Profile.viewer.maxFOV.outer * Mathf.Deg2Rad);
                    float fovTop = mNear * Mathf.Tan(Profile.viewer.maxFOV.upper * Mathf.Deg2Rad);
                    float fovRight = mNear * Mathf.Tan(Profile.viewer.maxFOV.inner * Mathf.Deg2Rad);
                    float fovBottom = mNear * Mathf.Tan(-Profile.viewer.maxFOV.lower * Mathf.Deg2Rad);
                    Matrix4x4 eyeProjection =
                        BaseARDevice.MakeProjection(fovLeft, fovTop, fovRight, fovBottom, mNear, mFar);
                    for (int i = 0; i < 2; i++)
                    {
                        VrsEye mEye = eyes[i];
                        if (mEye != null)
                        {
                            mEye.cam.projectionMatrix = eyeProjection;
                        }
                    }
                }
            }
        }

        public void ResetCameraFov()
        {
            for (int i = 0; i < 2; i++)
            {
                if (eyeOriginalProjection == null || eyeOriginalProjection[i] == null) return;
                VrsEye eye = eyes[i];
                if (eye != null)
                {
                    eye.cam.projectionMatrix = eyeOriginalProjection[i];
                }
            }

            oldFov = -1;
        }

        public void UpdateCameraFov(float fov)
        {
            if (fov > 90) fov = 90;
            if (fov < 5) fov = 5;
            if (eyeOriginalProjection == null && eyes[0] != null && eyes[1] != null)
            {
                eyeOriginalProjection = new Matrix4x4[2];
                eyeOriginalProjection[0] = eyes[0].cam.projectionMatrix;
                eyeOriginalProjection[1] = eyes[1].cam.projectionMatrix;
            }
            oldFov = fov;
            float near = VrsGlobal.fovNear > 0 ? VrsGlobal.fovNear : 0.0305f;
            float far = VrsGlobal.fovFar > 0 ? VrsGlobal.fovFar : 2000;
            far = far > 100 ? far : 2000;
            float fovLeft = near * Mathf.Tan(-fov * Mathf.Deg2Rad);
            float fovTop = near * Mathf.Tan(fov * Mathf.Deg2Rad);
            float fovRight = near * Mathf.Tan(fov * Mathf.Deg2Rad);
            float fovBottom = near * Mathf.Tan(-fov * Mathf.Deg2Rad);
            Matrix4x4 eyeProjection = BaseARDevice.MakeProjection(fovLeft, fovTop, fovRight, fovBottom, near, far);
            if (device != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    VrsEye eye = eyes[i];
                    if (eye != null)
                    {
                        eye.cam.projectionMatrix = eyeProjection;
                    }
                }
            }
        }

        private float displacementCoefficient = 1.0f;

        public float DisplacementCoefficient
        {
            get { return displacementCoefficient; }
            set { displacementCoefficient = value; }
        }

        public string GetStoragePath()
        {
            return device.GetStoragePath();
        }

        public void SetIsKeepScreenOn(bool keep)
        {
            device.SetIsKeepScreenOn(keep);
        }

        private float defaultIpd = -1;
        private float userIpd = -1;

        public void SetIpd(float ipd)
        {
            if (defaultIpd < 0)
            {
                defaultIpd = GetIpd();
            }
            VrsGlobal.dftProfileParams[0] = ipd; 
            userIpd = ipd;
            device.SetIpd(ipd);
            device.UpdateScreenData();
        }

        public void ResetIpd()
        {
            if (defaultIpd < 0) return;
            SetIpd(defaultIpd);
        }

        public float GetIpd()
        {
            if (userIpd > 0) return userIpd;

            return eyes[0] == null ? 0.060f : 2 * Math.Abs(eyes[0].GetComponent<Camera>().transform.localPosition.x);
        }

        public float GetUseIpd()
        {
            return userIpd;
        }

        [SerializeField] public SleepTimeoutMode sleepTimeoutMode = SleepTimeoutMode.NEVER_SLEEP;

        public SleepTimeoutMode SleepMode
        {
            get { return sleepTimeoutMode; }
            set
            {
                if (value != sleepTimeoutMode)
                {
                    sleepTimeoutMode = value;
                }
            }
        }

        public Camera GetMainCamera()
        {
            return Controller.cam;
        }

        public Camera GetLeftEyeCamera()
        {
            return Controller.Eyes[(int) Eye.Left].cam;
        }

        public Camera GetRightEyeCamera()
        {
            return Controller.Eyes[(int) Eye.Right].cam;
        }

        public Quaternion GetCameraQuaternion()
        {
            return GetHead().transform.rotation;
        }

        public bool IsCameraLocked()
        {
            return !VrsViewer.Instance.GetHead().IsTrackRotation();
        }

        public void LockCamera(bool isLock)
        {
            VrsHead head = VrsViewer.Instance.GetHead();
            head.SetTrackRotation(!isLock);
        }
        public Coroutine ViarusStartCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void ViarusStopCoroutine(Coroutine routine)
        {
            StopCoroutine(routine);
        }
    }
}
