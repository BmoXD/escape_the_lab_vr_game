using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public Transform holder;
    public Transform receiver;
    public GameObject indicatorPrefab;
    public Material glowingMaterial;
    public Material defaultMaterial;

    private GameObject currentItem;
    private GameObject indicator;

    private void OnTriggerEnter(Collider other)
    {
        if (currentItem == null && other.CompareTag("Item"))
        {
            ShowIndicator(other.gameObject);
        }
        else if (currentItem != null && other.CompareTag("Hand"))
        {
            GiveItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (indicator != null && other.gameObject == currentItem)
        {
            Destroy(indicator);
            indicator = null;
        }
    }

    private void ShowIndicator(GameObject item)
    {
        indicator = Instantiate(indicatorPrefab, item.transform.position, Quaternion.identity);
        indicator.transform.SetParent(item.transform);
        indicator.GetComponent<Renderer>().material = glowingMaterial;
    }

    private void Update()
    {
        if (currentItem != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RemoveItem();
            }
        }
    }

    private void RemoveItem()
    {
        Destroy(indicator);
        currentItem.transform.SetParent(holder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.layer = LayerMask.NameToLayer("HeldItem");
        currentItem = null;
    }

    private void GiveItem()
    {
        currentItem.transform.SetParent(receiver);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.layer = LayerMask.NameToLayer("HeldItem");
        currentItem = null;
    }
}
