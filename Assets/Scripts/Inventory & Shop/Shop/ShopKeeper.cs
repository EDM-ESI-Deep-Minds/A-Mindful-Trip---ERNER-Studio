using UnityEngine;
using UnityEngine.UI;

public class Shopkeeper : MonoBehaviour
{
    [SerializeField] private GameObject banner; // Reference to Banner child object
    private Animator bannerAnimator;
    private PlayerController currentPlayerInRange;

    private void Start()
    {
        if (banner != null)
        {
            banner.SetActive(true); // Active at all times
            bannerAnimator = banner.GetComponent<Animator>(); ;
        }
        // Debugging SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("No SpriteRenderer found on ShopKeeper!");
        else
            Debug.Log("ShopKeeper SpriteRenderer is active: " + sr.enabled);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.IsOwner)
            {
                currentPlayerInRange = player;
                currentPlayerInRange.SetNearShopkeeper(true);

                if (bannerAnimator != null)
                    bannerAnimator.SetBool("playerInRange", true); // Set parameter
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.IsOwner)
            {
                currentPlayerInRange.SetNearShopkeeper(false);

                // Hide banner
                if (bannerAnimator != null)
                    bannerAnimator.SetBool("playerInRange", false);

                currentPlayerInRange = null;
            }
        }
    }
}
