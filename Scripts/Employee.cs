using UnityEngine;

public class Employee : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float pushForce = 3f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Animation")]
    private Animator anim;

    [Header("Document Carry")]
    public Transform holdPosition;
    private Transform heldDocument = null;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!controller.enabled) return;

        isGrounded = false;
        if (controller.isGrounded)
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position - new Vector3(0, controller.height / 2, 0),
                0.2f
            );
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Ground"))
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += -9.81f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (anim != null)
            anim.SetFloat("Speed", moveZ);

        // E — interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
            foreach (Collider hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;
                KeycardPickup keycard = hit.GetComponent<KeycardPickup>();
                if (keycard != null) { keycard.Pickup(); break; }
                PunchMachine machine = hit.GetComponent<PunchMachine>();
                if (machine != null) { machine.TryOpen(); break; }
                FokyuPC pc = hit.GetComponent<FokyuPC>();
                if (pc != null) { pc.TrySubmit(); break; }
            }
        }

        // F — grab or drop document
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldDocument != null)
            {
                DropDocument();
                return;
            }
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
            foreach (Collider hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;
                DocumentPickup doc = hit.GetComponent<DocumentPickup>();
                if (doc != null) { GrabDocument(doc.transform); break; }
            }
        }
    }

    void GrabDocument(Transform doc)
    {
        heldDocument = doc;
        heldDocument.SetParent(holdPosition);
        heldDocument.localPosition = Vector3.zero;
        heldDocument.localRotation = Quaternion.identity;
        Rigidbody rb = heldDocument.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        GameManager.Instance.hasDocument = true;
    }

    void DropDocument()
    {
        heldDocument.SetParent(null);
        Rigidbody rb = heldDocument.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * 2f + transform.forward * 1f, ForceMode.Impulse);
        }
        heldDocument = null;
        GameManager.Instance.hasDocument = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}