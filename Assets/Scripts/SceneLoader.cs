using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("The plane object used for fading.")]
    public GameObject fadePlane;

    [Tooltip("Duration of the fade effect in seconds.")]
    public float fadeDuration = 2.0f;

    [Tooltip("Name of the level to load.")]
    public string levelName = "Level";

    [Tooltip("Create a volume (Box Collider) to trigger the scene transition when touched by the player.")]
    public bool MakeVolume = false;

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

        if (MakeVolume)
        {
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
            }
        }
    }

    public void ChangeScene()
    {
        StartCoroutine(FadeAndLoadScene(false));
    }

    public void CloseGame()
    {
        StartCoroutine(FadeAndLoadScene(true));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFading && other.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            StartCoroutine(FadeAndLoadScene(false));
        }
    }

    private IEnumerator FadeAndLoadScene(bool isClosingGame)
    {
        if (fadeMaterial == null)
        {
            Debug.LogError("Fade material is not set!");
            yield break;
        }

        isFading = true;
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

        if (isClosingGame)
        {
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
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

        if (MakeVolume)
        {
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
            }
        }
        else
        {
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                DestroyImmediate(boxCollider);
            }
        }
    }
}
