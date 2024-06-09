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

	private void Start()
	{
		interactable = GetComponent<XRGrabInteractable>();
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		animator.enabled = false;
	}

	public void UseKey(Keyhole keyhole)
	{
		// Detach the key from the player's hand
		Debug.Log("Key used");
		interactable.enabled = false;
		if (rb != null)
		{
			rb.isKinematic = true;
		}

		// // Position the key
		// Vector3 keyholeDirection = keyhole.transform.forward;
		// Vector3 keyPosition = keyhole.transform.position - keyholeDirection * 0.5f; // Adjust as needed
		// transform.position = keyPosition;
		// transform.rotation = Quaternion.LookRotation(keyholeDirection, Vector3.up);


		transform.SetParent(keyhole.transform);
		transform.localPosition = new Vector3 (0,0,-0.2f);
		//transform.localRotation = Quaternion.LookRotation(keyhole.transform.forward);

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
