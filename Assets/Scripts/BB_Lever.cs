using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction
{
    public class CustomXRLever : XRBaseInteractable
    {
        [SerializeField]
        [Tooltip("The lever's handle that can be manipulated")]
        private Transform m_Handle;

        [SerializeField]
        [Tooltip("Minimum angle for the lever")]
        [Range(-90f, 90f)]
        private float m_MinAngle = 79f;

        [SerializeField]
        [Tooltip("Maximum angle for the lever")]
        [Range(-90f, 90f)]
        private float m_MaxAngle = -79f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Percentage of how much the lever needs to be pulled for it to snap in place at max angle when let go")]
        private float m_SnapPercentage = 0.8f;

        [SerializeField]
        [Tooltip("Duration of the animation when snapping to max angle (in seconds)")]
        private float m_AnimationLength = 0.5f;

        [SerializeField]
        [Tooltip("List of ItemUnlocker objects to unlock when the lever is pulled")]
        //private List<ItemUnlocker> m_ItemsToUnlock;

        private bool m_IsLeverUsable = true;
        private IXRSelectInteractor m_Interactor;
        private Quaternion m_InitialRotation;
        private Coroutine m_AnimationCoroutine;

        public UnityEvent onActivated;  // Event for lever activation

        private void Start()
        {
            m_InitialRotation = m_Handle.localRotation;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnLeverGrabbed);
            selectExited.AddListener(OnLeverReleased);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(OnLeverGrabbed);
            selectExited.RemoveListener(OnLeverReleased);
            base.OnDisable();
        }

        private void OnLeverGrabbed(SelectEnterEventArgs args)
        {
            if (!m_IsLeverUsable) return;
            m_Interactor = args.interactorObject;
        }

        private void OnLeverReleased(SelectExitEventArgs args)
        {
            if (!m_IsLeverUsable) return;
            UpdateLeverState();
            m_Interactor = null;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && m_IsLeverUsable && isSelected)
            {
                UpdateLeverRotation();
            }
        }

        private void UpdateLeverRotation()
        {
            Vector3 direction = m_Interactor.GetAttachTransform(this).position - m_Handle.position;
            direction = transform.InverseTransformDirection(direction);
            direction.x = 0;

            float angle = Mathf.Atan2(direction.z, direction.y) * Mathf.Rad2Deg;

            if (m_MinAngle < m_MaxAngle)
            {
                angle = Mathf.Clamp(angle, m_MinAngle, m_MaxAngle);
            }
            else
            {
                angle = Mathf.Clamp(angle, m_MaxAngle, m_MinAngle);
            }

            m_Handle.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }

        private void UpdateLeverState()
        {
            float currentAngle = m_Handle.localRotation.eulerAngles.x;
            if (currentAngle > 180f) currentAngle -= 360f;

            float normalizedAngle = (currentAngle - m_MinAngle) / (m_MaxAngle - m_MinAngle);
            if (normalizedAngle > m_SnapPercentage)
            {
                if (m_AnimationCoroutine == null)
                {
                    m_AnimationCoroutine = StartCoroutine(SmoothAnimation(m_MaxAngle));
                    m_IsLeverUsable = false;
                    onActivated?.Invoke();
                }
            }
            else
            {
                m_Handle.localRotation = m_InitialRotation;
            }
        }

        // private void UnlockItems()
        // {
        //     foreach (var unlocker in m_ItemsToUnlock)
        //     {
        //         unlocker.UnlockItem();
        //     }
        // }

        private System.Collections.IEnumerator SmoothAnimation(float targetAngle)
        {
            Quaternion startRotation = m_Handle.localRotation;
            Quaternion targetRotation = Quaternion.Euler(targetAngle, 0f, 0f);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / m_AnimationLength;
                m_Handle.localRotation = Quaternion.Slerp(startRotation, targetRotation, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            m_Handle.localRotation = targetRotation;
            m_AnimationCoroutine = null;
        }

        private void OnValidate()
        {
            // Update lever rotation when minimum angle changes in editor
            if (Application.isPlaying)
            {
                UpdateLeverRotation();
            }
            else
            {
                m_Handle.localRotation = Quaternion.Euler(m_MinAngle, 0f, 0f);
                m_InitialRotation = m_Handle.localRotation;
            }
        }
    }
}
