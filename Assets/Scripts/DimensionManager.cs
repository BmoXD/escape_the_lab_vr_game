using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DimensionManager : MonoBehaviour
{
    [System.Serializable]
    public class Dimension
    {
        public string DimensionLetter;
        public GameObject DimensionParent;
    }

    public List<Dimension> Dimensions = new List<Dimension>();
    public List<GameObject> ImmuneObjects = new List<GameObject>();

    [SerializeField] private string currentDimensionLetter;
    public string CurrentDimensionLetter
    {
        get => currentDimensionLetter;
        set
        {
            if (currentDimensionLetter != value)
            {
                currentDimensionLetter = value;
                OnDimensionChanged?.Invoke(currentDimensionLetter);
                RefreshDimensionObjects();
            }
        }
    }

    public UnityAction<string> OnDimensionChanged;

    private void Awake()
    {
        RefreshDimensionObjects();
    }

    public void AddObjectToDimension(string dimensionLetter, GameObject obj)
    {
        foreach (var dimension in Dimensions)
        {
            if (dimension.DimensionLetter == dimensionLetter)
            {
                obj.transform.SetParent(dimension.DimensionParent.transform);
                return;
            }
        }
    }

    public void RemoveObjectFromDimension(GameObject obj)
    {
        obj.transform.SetParent(null);
    }

    public void AddObjectToImmuneList(GameObject obj)
    {
        if (!ImmuneObjects.Contains(obj))
        {
            ImmuneObjects.Add(obj);
        }
    }

    public void RemoveObjectFromImmuneList(GameObject obj)
    {
        if (ImmuneObjects.Contains(obj))
        {
            ImmuneObjects.Remove(obj);
        }
    }

    private void RefreshDimensionObjects()
    {
        foreach (var dimension in Dimensions)
        {
            bool isActive = dimension.DimensionLetter == currentDimensionLetter;

            foreach (Transform child in dimension.DimensionParent.transform)
            {
                SetObjectActiveState(child.gameObject, isActive);
            }
        }
    }

    private void SetObjectActiveState(GameObject obj, bool isActive)
    {
        if (!ImmuneObjects.Contains(obj))
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = isActive;
            }

            var colliders = obj.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = isActive;
            }

            var rigidbodies = obj.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rigidbodies)
            {
                if (rb != null)
                {
                    rb.isKinematic = !isActive;
                    if (!isActive)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            RefreshDimensionObjects();
        }
    }
}
