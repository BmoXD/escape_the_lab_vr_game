using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BB_EndScreen : MonoBehaviour
{
    [Tooltip("The plane object used for fading.")]
    public GameObject fadePlane;

    [Tooltip("Duration of the fade effect in seconds.")]
    public float fadeDuration = 2.0f;

    [Tooltip("Name of the level to load.")]
    public string levelName = "Level";

    private Material fadeMaterial;
    private bool isFading = false;

    private void Start()
    {
        if (fadePlane != null)
        {
            Renderer renderer = fadePlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                fadeMaterial = renderer.material;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFading && other.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (fadeMaterial == null)
        {
            Debug.LogError("Fade material is not set!");
            yield break;
        }

        Color originalColor = fadeMaterial.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        fadeMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        SceneManager.LoadScene(levelName);
    }

    private void OnValidate()
    {
        if (fadePlane != null)
        {
            Renderer renderer = fadePlane.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (fadeMaterial == null)
                {
                    fadeMaterial = new Material(renderer.sharedMaterial);
                    renderer.sharedMaterial = fadeMaterial;
                }

                fadeMaterial.SetColor("_BaseColor", new Color(fadeMaterial.color.r, fadeMaterial.color.g, fadeMaterial.color.b, 0));
            }
        }
    }
}
