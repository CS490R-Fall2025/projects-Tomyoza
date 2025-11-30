using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("Shop Type")]
    // Type exactly: "Meat", "Vegetable", "Fish", or "Weapon"
    public string shopType = "Meat"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShopManager.Instance.OpenShop(shopType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShopManager.Instance.CloseShop();
        }
    }
}