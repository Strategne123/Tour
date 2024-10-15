
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Globalization;

namespace Vrs.Internal
{
    [AddComponentMenu("VRS/GazeInputModule")]
    public class GazeInputModule : BaseInputModule
    {

        private static Vector3 raycastResultPosition = Vector3.zero;

        public static Vector3 GetRaycastResultPosition()
        {
            return raycastResultPosition;
        }

        [Tooltip("Whether gaze input is active in Split Mode only (true), or all the time (false).")]
        private bool splitModeOnly = false;

        public static VrsReticle gazePointer;

        private PointerEventData pointerData;
        private Vector2 lastHeadPose;

        private bool isActive = false;

        private const float clickTime = 0.1f; 

        private Vector2 screenCenterVec = Vector2.zero;

        bool isShowGaze = true;

        public override bool ShouldActivateModule()
        {
            bool activeState = base.ShouldActivateModule();

            activeState = activeState && (VrsViewer.Instance.SplitScreenModeEnabled || !splitModeOnly);

            if (activeState != isActive)
            {
                isActive = activeState;

                if (gazePointer != null)
                {
                    if (isActive)
                    {
                        gazePointer.OnGazeEnabled();
                    }
                }
            }

            return activeState;
        }

        public override void DeactivateModule()
        {
            //Debug.Log("DeactivateModule");
            DisableGazePointer();
            base.DeactivateModule();
            if (pointerData != null)
            {
                HandlePendingClick();
                HandlePointerExitAndEnter(pointerData, null);
                pointerData = null;
            }

            eventSystem.SetSelectedGameObject(null, pointerData);
            //Debug.Log("DeactivateModule");
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                VrsViewer.Instance.Triggered = false;
                if (pointerData != null && pointerData.selectedObject != null)
                {
                    HandlePointerExitAndEnter(pointerData, null);
                    eventSystem.SetSelectedGameObject(null, pointerData);
                    pointerData.Reset();
                }
            }
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
            return pointerData != null && pointerData.pointerEnter != null;
        }

        public override void Process()
        {

            if (TouchScreenKeyboard.visible)
            {
                VrsViewer.Instance.DismissReticle();

                VrsViewer.Instance.Triggered = false;
                pointerData.eligibleForClick = false;
                return;
            }

            if (!isShowGaze)
            {
                VrsViewer.Instance.SwitchControllerMode(false);
                isShowGaze = true;
                pointerData.pointerPress = null;
                HandlePointerExitAndEnter(pointerData, null);
            }

            GameObject gazeObjectPrevious = GetCurrentGameObject();
            CastRayFromGaze();
            UpdateCurrentObject();
            UpdateReticle(gazeObjectPrevious);

            if (!pointerData.eligibleForClick && VrsViewer.Instance.Triggered)
            {

                HandleTrigger();
                VrsViewer.Instance.Triggered = false;
            }
            else if (!VrsViewer.Instance.Triggered)
            {

                HandlePendingClick();
            }
            else if (pointerData.eligibleForClick && VrsViewer.Instance.Triggered)
            {
                VrsViewer.Instance.Triggered = false;
            }
            SetRaycastResultPosition();
        }

        private void CastRayFromGaze()
        {
            Vector2 headPose =
                NormalizedCartesianToSpherical(VrsViewer.Instance.HeadPose.Orientation * Vector3.forward);

            if (pointerData == null)
            {
                pointerData = new PointerEventData(eventSystem);
                lastHeadPose = headPose;
            }

            Vector2 diff = headPose - lastHeadPose;

            if (screenCenterVec.x == 0)
            {
                screenCenterVec = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);
            }

            pointerData.Reset();
            var raycastResult = new RaycastResult();
            raycastResult.Clear();
            Transform mTrans = VrsViewer.Instance.GetHead().GetTransform();
            raycastResult.worldPosition = mTrans.position;
            raycastResult.worldNormal = mTrans.forward;
            pointerData.pointerCurrentRaycast = raycastResult;
            pointerData.position = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

            pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            foreach (RaycastResult mRaycastResult in m_RaycastResultCache)
            {
                pointerData.pointerCurrentRaycast = mRaycastResult;
                break;
            }

            m_RaycastResultCache.Clear();
            pointerData.delta = diff;
            lastHeadPose = headPose;
            if (pointerData.pointerCurrentRaycast.gameObject == null && eventSystem.currentSelectedGameObject != null)
            {
                //Debug.LogError("Clear Seleted GameObject-Gaze=>" + eventSystem.currentSelectedGameObject.name);
                eventSystem.SetSelectedGameObject(null);
            }
        }

        private void SetRaycastResultPosition()
        {
            if (pointerData != null && pointerData.pointerCurrentRaycast.gameObject)
            {
                raycastResultPosition = pointerData.pointerCurrentRaycast.worldPosition;
            }
            else
            {
                raycastResultPosition = Vector3.zero;
            }
        }

        private void UpdateCurrentObject()
        {
            if (pointerData == null)
            {
                return;
            }

            var go = pointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerData, go);
        }

        void UpdateReticle(GameObject previousGazedObject)
        {
            if (pointerData == null)
            {
                return;
            }

            Camera camera = pointerData.enterEventCamera; 
            Vector3 intersectionPosition = GetIntersectionPosition();
            GameObject gazeObject = GetCurrentGameObject(); 

            if (gazeObject != null && VrsOverrideSettings.OnGazeEvent != null)
            {
                VrsOverrideSettings.OnGazeEvent(gazeObject);
            }

            float gazeZ = VrsGlobal.defaultGazeDistance;
            if (gazeObject != null)
            {
                gazeZ = intersectionPosition.z; 
            }

            VrsGlobal.focusObjectDistance = (int) (Mathf.Abs(gazeZ) * 100) / 100.0f;

            bool isInteractive = pointerData.pointerPress == gazeObject ||
                                 ExecuteEvents.GetEventHandler<ISelectHandler>(gazeObject) != null;

            if (gazeObject != previousGazedObject)
            {
                if (previousGazedObject != null && gazePointer != null)
                {
                    gazePointer.OnGazeExit(camera, previousGazedObject);
                    if (VrsViewer.Instance != null)
                    {
                        if (VrsViewer.Instance.HeadControl == HeadControl.Hover)
                        {
                            if (ViarusHMDControl.baseEventData != null)
                            {
                                ViarusHMDControl.baseEventData = null;
                                ViarusHMDControl.eventGameObject = null;
                            }
                        }
                    }
                }

                if (gazeObject != null && gazePointer != null)
                {
                    gazePointer.OnGazeStart(camera, gazeObject, intersectionPosition, isInteractive);
                    if (VrsViewer.Instance != null)
                    {
                        if (VrsViewer.Instance.HeadControl == HeadControl.Hover)
                        {
                            ViarusHMDControl.baseEventData = pointerData;
                        }
                    }
                }
            }
        }

        private void HandleDrag()
        {
            bool moving = pointerData.IsPointerMoving();

            if (moving && pointerData.pointerDrag != null && !pointerData.dragging)
            {
                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData,
                    ExecuteEvents.beginDragHandler);
                pointerData.dragging = true;
            }

            if (pointerData.dragging && moving && pointerData.pointerDrag != null)
            {

                if (pointerData.pointerPress != pointerData.pointerDrag)
                {
                    ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

                    pointerData.eligibleForClick = false;
                    pointerData.pointerPress = null;
                    pointerData.rawPointerPress = null;
                }

                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
            }
        }

        private void HandlePendingClick()
        {
            if (!pointerData.eligibleForClick && !pointerData.dragging)
            {
                return;
            }

            if (gazePointer != null)
            {
                Camera camera = pointerData.enterEventCamera;
                gazePointer.OnGazeTriggerEnd(camera);
            }

            var go = pointerData.pointerCurrentRaycast.gameObject;

            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);

            if (pointerData.eligibleForClick)
            {
                if (VrsViewer.Instance != null)
                {
                    ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);
                }
            }
            else if (pointerData.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.dropHandler);
                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.endDragHandler);
            }

            pointerData.pointerPress = null;
            pointerData.rawPointerPress = null;
            pointerData.eligibleForClick = false;
            pointerData.clickCount = 0;
            pointerData.clickTime = 0;
            pointerData.pointerDrag = null;
            pointerData.dragging = false;
        }

        private void HandleTrigger()
        {
            var go = pointerData.pointerCurrentRaycast.gameObject;

            pointerData.pressPosition = pointerData.position;
            pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
            pointerData.pointerPress =
                ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler)
                ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

            pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);
            if (pointerData.pointerDrag != null)
            {
                ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);
            }

            pointerData.rawPointerPress = go;
            pointerData.eligibleForClick = true;
            pointerData.delta = Vector2.zero;
            pointerData.dragging = false;
            pointerData.useDragThreshold = true;
            pointerData.clickCount = 1;
            pointerData.clickTime = Time.unscaledTime;

            if (gazePointer != null)
            {
                gazePointer.OnGazeTriggerStart(pointerData.enterEventCamera);
            }
        }

        private Vector2 NormalizedCartesianToSpherical(Vector3 cartCoords)
        {
            cartCoords.Normalize();
            if (cartCoords.x == 0)
                cartCoords.x = Mathf.Epsilon;
            float outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
            if (cartCoords.x < 0)
                outPolar += Mathf.PI;
            float outElevation = Mathf.Asin(cartCoords.y);
            return new Vector2(outPolar, outElevation);
        }

        GameObject GetCurrentGameObject()
        {
            if (pointerData != null && pointerData.enterEventCamera != null)
            {
                return pointerData.pointerCurrentRaycast.gameObject;
            }

            return null;
        }

        Vector3 GetIntersectionPosition()
        {

            Camera cam = pointerData.enterEventCamera;
            if (cam == null)
            {
                return Vector3.zero;
            }

            float intersectionDistance = pointerData.pointerCurrentRaycast.distance + cam.nearClipPlane;
            Vector3 intersectionPosition = cam.transform.position + cam.transform.forward * intersectionDistance;

            return intersectionPosition;
        }

        void DisableGazePointer()
        {
            if (gazePointer == null)
            {
                return;
            }

            GameObject currentGameObject = GetCurrentGameObject();
            if (currentGameObject)
            {
                Camera camera = pointerData.enterEventCamera;
                gazePointer.OnGazeExit(camera, currentGameObject);
                if (VrsViewer.Instance != null)
                {
                    if (VrsViewer.Instance.HeadControl == HeadControl.Hover)
                    {
                        if (ViarusHMDControl.baseEventData != null)
                            ViarusHMDControl.baseEventData = null;
                    }
                }
            }

            gazePointer.OnGazeDisabled();
        }
    }
}