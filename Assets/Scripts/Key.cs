using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Key : MonoBehaviour
{
	// [Tooltip("Animation clip for key insertion and turning.")]
	// public AnimationClip insertionAnimation;

	private XRGrabInteractable interactable;
	private Rigidbody rb;
	private Animator animator;
	public Material instancedMaterial;
	private Renderer meshRenderer;
	private bool keyUsed;

	private void Start()
	{
		interactable = GetComponent<XRGrabInteractable>();
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		animator.enabled = false;
	}

	public void UseKey(Keyhole keyhole)
	{
		keyUsed = true;
		// Detach the key from the player's hand
		Debug.Log("Key used");
		interactable.enabled = false;
		if (rb != null)
		{
			rb.isKinematic = true;
		}

		transform.SetParent(keyhole.transform);
		transform.localPosition = new Vector3 (0,0,-0.2f);
		
		AlignLocalZAxis(transform, keyhole.transform);
		
		// Play the key insertion animation
		if (animator != null)
		{
			animator.enabled = true;
			animator.Play("UseKey");
		}
		else
		{
			Debug.LogError("Insertion animation clip or Animator component is missing!");
		}
		
	}

	private void AlignLocalZAxis(Transform targetObject, Transform referenceObject)
    {
        // Get the forward direction (local z-axis) of the reference object
        Vector3 referenceForward = referenceObject.forward;

        // Calculate the new rotation for the target object
        Quaternion newRotation = Quaternion.LookRotation(referenceForward, Vector3.up);

        // Apply the new rotation to the target object
        targetObject.rotation = newRotation;
    }
	
	private void OnValidate()	
	{
		meshRenderer = GetComponentInChildren<Renderer>();
		if (keyUsed == true)
		{
			rb.isKinematic = true;
		}
	}
	
	public void ApplyInstancedMaterial(Material material)
	{
		if (material != null)
		{
			instancedMaterial = material;

			if (meshRenderer != null)
			{
				var materialsCopy = meshRenderer.sharedMaterials;
				if (materialsCopy.Length > 1)
				{
					materialsCopy[1] = instancedMaterial;
					meshRenderer.sharedMaterials = materialsCopy;
				}
				else
				{
					Debug.LogWarning("Not enough materials on the key mesh renderer to set the instance material at index 1.");
				}
			}
			else
			{
				Debug.LogWarning("Mesh renderer not found! : "+meshRenderer);
			}
		}
	}

	public void UpdateMaterialColor(Color color)
	{
		if (instancedMaterial != null)
		{
			instancedMaterial.SetColor("_BoundColor", color);
		}
	}
}
