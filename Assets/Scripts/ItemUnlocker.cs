using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
public class ItemUnlocker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The item to be unlocked, which must have an XRGrabInteractable component")]
    private GameObject m_Item;

    [SerializeField]
    [Tooltip("The animation to play when unlocking the item")]
    private AnimationClip m_UnlockAnimation;

    [SerializeField]
    [Tooltip("The cage object to hide when the item is unlocked")]
    private GameObject m_CageObject;

    private XRGrabInteractable m_GrabInteractable;
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        ValidateItem();
    }

    private void ValidateItem()
    {
        if (m_Item != null)
        {
            m_GrabInteractable = m_Item.GetComponent<XRGrabInteractable>();
            if (m_GrabInteractable == null)
            {
                Debug.LogWarning($"The item {m_Item.name} does not have an XRGrabInteractable component.");
            }
            else
            {
                m_GrabInteractable.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("No item assigned to unlock.");
        }
    }

    public void UnlockItem()
    {
        if (m_UnlockAnimation != null)
        {
            m_Animator.Play(m_UnlockAnimation.name);
            StartCoroutine(EnableGrabInteractableAfterAnimation(m_UnlockAnimation.length));
        }
        else if (m_CageObject != null)
        {
            m_CageObject.SetActive(false);
            EnableGrabInteractable();
        }
        else
        {
            EnableGrabInteractable();
        }
    }

    private System.Collections.IEnumerator EnableGrabInteractableAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableGrabInteractable();
    }

    private void EnableGrabInteractable()
    {
        if (m_GrabInteractable != null)
        {
            m_GrabInteractable.enabled = true;
        }
    }

    private void OnValidate()
    {
        ValidateItem();
    }
}

