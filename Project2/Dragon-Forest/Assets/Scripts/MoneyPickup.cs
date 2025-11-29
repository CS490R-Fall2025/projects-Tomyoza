using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [SerializeField] private int value = 10;
    [SerializeField] private float rotateSpeed = 100f;


    private void Update()
    {
        // Simple visual rotation
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the wallet on the player
            PlayerWallet wallet = other.GetComponent<PlayerWallet>();
            
            if (wallet != null)
            {
                wallet.AddMoney(value);
                
                Destroy(gameObject);
            }
        }
    }
}