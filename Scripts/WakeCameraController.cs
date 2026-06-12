using UnityEngine;

public class WakeCameraController : MonoBehaviour
{
    [Header("Look Limits")]
    public float sensitivity = 100f;
    public float horizontalClamp = 90f;  // 180 total range
    public float verticalClamp = 30f;

    private float yaw = 0f;
    private float pitch = 0f;
    private Quaternion baseRotation;

    public bool canLook = false;

    void Start()
    {
        baseRotation = transform.rotation;
        // Initialize yaw/pitch from current rotation so it doesn't snap
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        yaw = Mathf.Clamp(yaw, baseRotation.eulerAngles.y - horizontalClamp, baseRotation.eulerAngles.y + horizontalClamp);
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}