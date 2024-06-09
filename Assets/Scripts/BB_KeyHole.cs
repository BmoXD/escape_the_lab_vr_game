using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[ExecuteInEditMode] // Ensure this script runs in edit mode
public class Keyhole : MonoBehaviour
{
	[Tooltip("The key GameObject that can interact with this keyhole.")]
	public GameObject boundKey;

	[Tooltip("Events to trigger when the correct key is used.")]
	public UnityEvent onKeyUsed;

	[Tooltip("Material to apply to the keyhole.")]
	public Material keyholeMaterial;

	[Tooltip("Color to apply to the keyhole material's _BoundColor property.")]
	public Color keyholeColor;

	private Key key;
	private Material instancedMaterial;

	private Renderer meshRenderer;

	private void Start()
	{
		ApplyMaterial();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == boundKey)
		{
			if (key != null)
			{
				key.UseKey(this);
				OnKeyInserted();
			}
		}
	}

	public void OnKeyInserted()
	{
		onKeyUsed.Invoke();
	}

	private void OnValidate()
	{
		if (boundKey != null)
		{
			key = boundKey.GetComponent<Key>();
			Debug.Log(key);
		}
		// Apply material changes in edit mode
		ApplyMaterial();
	}

	private void ApplyMaterial()
	{
		if (keyholeMaterial != null)
		{
			meshRenderer = GetComponent<MeshRenderer>();

			// Check if instancedMaterial already exists, if not, create a new instance
			if (instancedMaterial == null)
			{
				instancedMaterial = new Material(keyholeMaterial);
				
				
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
			
			// Apply the instanced material to the key
			if(instancedMaterial != null && key != null && key.instancedMaterial == null)
			{
				key.ApplyInstancedMaterial(instancedMaterial);
				Debug.Log("Instanced material applied to key");
			}

			// Set the color property of the instanced material
			instancedMaterial.SetColor("_BoundColor", keyholeColor);

			//Update the color for the key
			key.UpdateMaterialColor(keyholeColor);
		}
	}
}
