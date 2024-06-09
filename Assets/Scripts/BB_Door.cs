using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB_Door : MonoBehaviour
{
	[Tooltip("Material to apply.")]
	public Material doorBoundMaterial;
	
	[Tooltip("Color to apply to the Door's material's _BoundColor property.")]
	public Color doorColor;
	
	private Animator animator;
	private Material instancedMaterial;
	private Renderer meshRenderer;
	
	private void Start()
	{
		ApplyMaterial();
		animator = GetComponent<Animator>();
		animator.enabled = false;
	}

	private void OnValidate()
	{
		if(meshRenderer == null)
		{
			meshRenderer = GetComponentInChildren<Renderer>();
		}
		Debug.Log(meshRenderer);
		// Apply material changes in edit mode
		ApplyMaterial();
	}
	
	public void UnlockDoor()
	{
		// Play the key insertion animation
		if (animator != null)
		{
			animator.enabled = true;
			animator.Play("Door_Open");
		}
		else
		{
			Debug.LogError("Insertion animation clip or Animator component is missing!");
		}
		
	}
	
	private void ApplyMaterial()
	{
		if (doorBoundMaterial != null)
		{

			// Check if instancedMaterial already exists, if not, create a new instance
			if (instancedMaterial == null)
			{
				Debug.Log("dsadaifnofinofafnoafnsaw");
				instancedMaterial = new Material(doorBoundMaterial);
				
				// Apply the instanced material to the keyhole
				var sharedMaterialsCopy = meshRenderer.sharedMaterials;
				if (sharedMaterialsCopy.Length > 1)
				{
					sharedMaterialsCopy[1] = instancedMaterial;
					meshRenderer.sharedMaterials = sharedMaterialsCopy;
				}
				else
				{
					Debug.LogWarning("Not enough materials on the keyhole mesh renderer to set the instance material at index 1.");
				}
			}

			// Set the color property of the instanced material
			instancedMaterial.SetColor("_BoundColor", doorColor);
		}
	}
}
