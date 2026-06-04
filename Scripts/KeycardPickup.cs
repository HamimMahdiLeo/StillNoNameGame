using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    public void Pickup()
    {
        GameManager.Instance.hasKeycard = true;
        gameObject.SetActive(false);
    }
}