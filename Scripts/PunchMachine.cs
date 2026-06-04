using UnityEngine;

public class PunchMachine : MonoBehaviour
{
    public SlidingDoor door;
    public bool isInsideMachine = false;

    public void TryOpen()
    {
        if (!GameManager.Instance.hasKeycard) return;

        bool playerIsInside = GameManager.Instance.playerIsInside;

        if (isInsideMachine == playerIsInside)
            door.OpenDoor();
    }
}