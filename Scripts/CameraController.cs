using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    public Transform playerTransform;
    public float sensitivity = 200f;
    public float minXAngle = -80f;
    public float maxXAngle = 80f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Head Bob")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;
    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.01f;

    private float bobTimer = 0f;
    private Vector3 defaultCamPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCamPos = transform.localPosition;
    }

    void Update()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minXAngle, maxXAngle);
        rotationY += mouseX;

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        playerTransform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        // Head bob
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * walkBobSpeed;
            float bobY = Mathf.Sin(bobTimer) * walkBobAmount;
            float bobX = Mathf.Sin(bobTimer * 0.5f) * walkBobAmount * 0.5f;
            transform.localPosition = new Vector3(
                defaultCamPos.x + bobX,
                defaultCamPos.y + bobY,
                defaultCamPos.z
            );
        }
        else
        {
            bobTimer += Time.deltaTime * idleBobSpeed;
            float idleBob = Mathf.Sin(bobTimer) * idleBobAmount;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(defaultCamPos.x, defaultCamPos.y + idleBob, defaultCamPos.z),
                Time.deltaTime * 5f
            );
        }
    }
}