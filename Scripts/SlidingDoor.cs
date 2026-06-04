using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    public Transform leftPanel;
    public Transform rightPanel;
    public float slideDistance = 2f;
    public float speed = 2f;
    public float autoCloseDelay = 3f;

    private bool isOpen = false;
    private Vector3 leftOrigin;
    private Vector3 rightOrigin;

    void Start()
    {
        leftOrigin = leftPanel.position;
        rightOrigin = rightPanel.position;
    }

    public void OpenDoor()
    {
        if (!isOpen)
            StartCoroutine(SlideOpen());
    }

    IEnumerator SlideOpen()
    {
        isOpen = true;

        Vector3 leftTarget = leftOrigin + Vector3.left * slideDistance;
        Vector3 rightTarget = rightOrigin + Vector3.right * slideDistance;

        while (Vector3.Distance(leftPanel.position, leftTarget) > 0.01f)
        {
            leftPanel.position = Vector3.MoveTowards(leftPanel.position, leftTarget, speed * Time.deltaTime);
            rightPanel.position = Vector3.MoveTowards(rightPanel.position, rightTarget, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(autoCloseDelay);

        StartCoroutine(SlideClose());
    }

    IEnumerator SlideClose()
    {
        while (Vector3.Distance(leftPanel.position, leftOrigin) > 0.01f)
        {
            leftPanel.position = Vector3.MoveTowards(leftPanel.position, leftOrigin, speed * Time.deltaTime);
            rightPanel.position = Vector3.MoveTowards(rightPanel.position, rightOrigin, speed * Time.deltaTime);
            yield return null;
        }

        isOpen = false;
    }
}