using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

namespace Lean.Touch
{
    // This script allows you to translate the current GameObject relative to the camera
    public class CustomLeanTranslate : MonoBehaviour
    {
        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreStartedOverGui = true;

        [Tooltip("Ignore fingers with IsOverGui?")]
        public bool IgnoreIsOverGui;

        [Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
        public int RequiredFingerCount;

        [Tooltip("Does translation require an object to be selected?")]
        public LeanSelectable RequiredSelectable;

        [Tooltip("The camera the translation will be calculated using (None = MainCamera)")]
        public Camera Camera;

        public LayerMask ARTestLayer { get { return _ARTestLayer; } set { _ARTestLayer = value; } }
        private LayerMask _ARTestLayer;

        [SerializeField]
        public ARSessionOrigin sessionOrigin { get { return m_SessionOrigin; } set { m_SessionOrigin = value; } }
        private ARSessionOrigin m_SessionOrigin;

        [SerializeField]
        public List<ARRaycastHit> hits { get { return s_Hits; } set { s_Hits = value; } }
        private List<ARRaycastHit> s_Hits;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Start();
        }
#endif

        protected virtual void Start()
        {
            if (RequiredSelectable == null)
            {
                RequiredSelectable = GetComponent<LeanSelectable>();
            }
        }

        protected virtual void Update()
        {
            // Get the fingers we want to use
            var fingers = LeanSelectable.GetFingers(IgnoreStartedOverGui, IgnoreIsOverGui, RequiredFingerCount, RequiredSelectable);

            // Calculate the screenDelta value based on these fingers
            var screenDelta = LeanGesture.GetScreenDelta(fingers);

            // Perform the translation
            if (transform is RectTransform)
            {
                TranslateUI(screenDelta);
            }
            else
            {
                Translate(screenDelta);
            }
        }

        protected virtual void TranslateUI(Vector2 screenDelta)
        {
            // Screen position of the transform
            var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera, transform.position);

            // Add the deltaPosition
            screenPoint += screenDelta;

            // Convert back to world space
            var worldPoint = default(Vector3);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, Camera, out worldPoint) == true)
            {
                transform.position = worldPoint;
            }
        }

        protected virtual void Translate(Vector2 screenDelta)
        {
            // Make sure the camera exists
            var camera = LeanTouch.GetCamera(Camera, gameObject);

            if (camera != null)
            {
                // Screen position of the transform
                var screenPoint = camera.WorldToScreenPoint(transform.position);

                // Add the deltaPosition
                screenPoint += (Vector3)screenDelta;

                Ray ray = Camera.main.ScreenPointToRay(screenPoint);
#if UNITY_EDITOR
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500f, _ARTestLayer))
                {
                    transform.position = hit.point;
                }

#else
                if (m_SessionOrigin != null && s_Hits != null)
                {
                    if (m_SessionOrigin.Raycast(ray, s_Hits, TrackableType.PlaneWithinInfinity))
                    {
                        Pose hitPose = s_Hits[0].pose;
                        transform.position = hitPose.position;
                    }
                }
#endif
            }
        }
    }
}