
using UnityEngine;

namespace Vrs.Internal
{
    [AddComponentMenu("VRS/VrsHead")]
    public class VrsHead : MonoBehaviour
    {
        public Vector3 BasePosition { set; get; }

        private bool trackRotation = true;

        public void SetTrackRotation(bool b)
        {
            trackRotation = b;
        }

        public bool IsTrackRotation()
        {
            return trackRotation;
        }

        void Awake()
        {
            VrsViewer.Create();
        }

        protected Transform mTransform;

        public Transform GetTransform()
        {
            return mTransform;
        }

        void Start()
        {
            mTransform = this.transform;
        }

        void LateUpdate()
        {
            VrsViewer.Instance.UpdateHeadPose();
            UpdateHead();
        }

        private bool hasResetTracker = true;
        private void UpdateHead()
        {
            if (VrsGlobal.hasInfinityARSDK)
            {
                trackRotation = false;
            }
            if (trackRotation)
            {
               Quaternion rot = VrsViewer.Instance.HeadPose.Orientation;
                if(!hasResetTracker)
                {
                    hasResetTracker = true;
                    VrsViewer.Instance.Recenter();
                }
                Vector3 eulerAngles = rot.eulerAngles;
                mTransform.localRotation = rot;
            }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            Vector3 pos = VrsViewer.Instance.HeadPose.Position;
            if (pos.x !=0 && pos.y !=0 && pos.z != 0)
            {
                mTransform.localPosition = BasePosition + pos;
            }
#endif
        }
    }
}
