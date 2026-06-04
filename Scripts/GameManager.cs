using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool hasKeycard = false;
	public bool hasDocument = false;
	public bool documentSubmitted = false;
    public bool playerIsInside = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}