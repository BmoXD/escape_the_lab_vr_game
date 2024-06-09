// using System.Collections.Generic;
// using UnityEngine;

// public class DimensionObject : MonoBehaviour
// {
//     public DimensionManager.Dimension ObjectDimension;
    
//     private List<GameObject> childObjects;

//     private void Awake()
//     {
//         childObjects = new List<GameObject>();
//         GetChildObjects(transform);
//     }

//     private void GetChildObjects(Transform parent)
//     {
//         foreach (Transform child in parent)
//         {
//             childObjects.Add(child.gameObject);
//             GetChildObjects(child);
//         }
//     }

//     private void OnEnable()
//     {
//         if (DimensionManager.Instance != null)
//         {
//             DimensionManager.Instance.OnDimensionChanged += OnDimensionChanged;
//             SetActiveState(DimensionManager.Instance.CurrentDimension);
//         }
//     }

//     private void OnDisable()
//     {
//         if (DimensionManager.Instance != null)
//         {
//             DimensionManager.Instance.OnDimensionChanged -= OnDimensionChanged;
//         }
//     }

//     private void OnDimensionChanged(DimensionManager.Dimension newDimension)
//     {
//         SetActiveState(newDimension);
//     }

//     private void SetActiveState(DimensionManager.Dimension currentDimension)
//     {
//         bool isActive = currentDimension == ObjectDimension;

//         foreach (var childObject in childObjects)
//         {
//             var renderers = childObject.GetComponents<Renderer>();
//             foreach (var renderer in renderers)
//             {
//                 renderer.enabled = isActive;
//             }

//             var colliders = childObject.GetComponents<Collider>();
//             foreach (var collider in colliders)
//             {
//                 collider.enabled = isActive;
//             }

//             var rigidbodies = childObject.GetComponents<Rigidbody>();
//             foreach (var rb in rigidbodies)
//             {
//                 if (rb != null)
//                 {
//                     rb.isKinematic = !isActive;
//                     if (!isActive)
//                     {
//                         rb.velocity = Vector3.zero;
//                         rb.angularVelocity = Vector3.zero;
//                     }
//                 }
//             }
//         }
//     }

//     public void RefreshState()
//     {
//         if (DimensionManager.Instance != null)
//         {
//             SetActiveState(DimensionManager.Instance.CurrentDimension);
//         }
//     }

//     private void OnValidate()
//     {
//         if (!Application.isPlaying && DimensionManager.Instance != null)
//         {
//             RefreshState();
//         }
//     }
// }
