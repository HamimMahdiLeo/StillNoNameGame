using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = transform.rotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, isOpen ? openRot : closedRot, Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Camera cam = Camera.main;
            if (Vector3.Distance(cam.transform.position, transform.position) < 3f)
                isOpen = !isOpen;
        }
    }
}