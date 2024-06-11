using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StashItem : MonoBehaviour
{
    public Transform stashTransform; // Reference to the Stash object's transform
    public DimensionManager dimensionManager; // Reference to the DimensionManager
    private XRGrabInteractable interactable;
    private bool isNearStash = false;
    private bool isStashed = false;
    private Vector3 originalScale;

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnItemGrabbed); // Updated event name
        interactable.selectExited.AddListener(OnItemReleased); // Updated event name
        originalScale = transform.localScale;
        interactable.trackScale = false;

        if (dimensionManager == null)
        {
            dimensionManager = FindObjectOfType<DimensionManager>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stash"))
        {
            isNearStash = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stash"))
        {
            isNearStash = false;
        }
    }

    private void OnItemReleased(SelectExitEventArgs args) // Updated method signature
    {
        if (isStashed && !isNearStash && !args.isCanceled) // Check for !args.isCanceled as suggested
        {
            DetachFromStash();
        }
        else if (isNearStash)
        {
            SnapToStash();
        }
    }

    private void OnItemGrabbed(SelectEnterEventArgs args) // Updated method signature
    {
        if (isStashed)
        {
            Debug.Log("Scaling back to original scale!");
            ScaleItem(originalScale);
        }
    }

    private void SnapToStash()
    {
        // Disable the Rigidbody so the item doesn't move
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Set the item's position and rotation to match the stash
        transform.position = stashTransform.position;
        transform.rotation = stashTransform.rotation;

        // Parent the item to the stash
        transform.SetParent(stashTransform);
        originalScale = transform.localScale;

        // Mark as stashed
        isStashed = true;

        // Make the item immune to dimension changes
        if (dimensionManager != null)
        {
            dimensionManager.AddObjectToImmuneList(gameObject);
        }

        // Scale down the item
        ScaleItem(originalScale * 0.4f);
    }

    private void DetachFromStash()
    {
        // Enable the Rigidbody so the item can move
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        // ScaleItem(originalScale);
        // Unparent the item from the stash
        transform.SetParent(null);

        // Mark as not stashed
        isStashed = false;

        // Remove immunity to dimension changes
        if (dimensionManager != null)
        {
            dimensionManager.RemoveObjectFromImmuneList(gameObject);
            // Add the item to the current dimension
            dimensionManager.AddObjectToDimension(dimensionManager.CurrentDimensionLetter, gameObject);
        }

        ScaleItem(originalScale);
    }

    private void ScaleItem(Vector3 scale)
    {
        Debug.Log("Scale to: " + scale);
        transform.localScale = scale;
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnItemGrabbed); // Updated event name
            interactable.selectExited.RemoveListener(OnItemReleased); // Updated event name
        }
    }
}