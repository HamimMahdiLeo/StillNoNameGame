using UnityEngine;
using System.Collections;

public class OpeningSequence : MonoBehaviour
{
    [Header("Cameras")]
    public Camera cutsceneCamera;   // does the Sleep->Wake lerp
    public Camera wakeCamera;       // handles limited look while phone rings
    public Camera playerCamera;     // takes over after call ends

    [Header("Cutscene Cam Positions")]
    public Transform sleepTransform;
    public Transform wakeTransform;

    [Header("Timing")]
    public float blackScreenDuration = 1f;
    public float wakeUpDuration = 3f;
    public float fadeDuration = 1f;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;

    [Header("Player")]
    public Employee employeeScript;
    public CameraController cameraController;

    [Header("Phone")]
    public AudioSource phoneRingSource;
    public AudioClip phoneRingClip;
    public AudioSource fokyuVoiceSource;
    public AudioClip fokyuIntroClip;

    [Header("UI")]
    public GameObject phonePrompt;

    [Header("Task Manager")]
    public TaskManager taskManager;

    private WakeCameraController wakeCameraController;

    void Start()
    {
        employeeScript.enabled = false;
        cameraController.enabled = false;
        playerCamera.gameObject.SetActive(false);
        wakeCamera.gameObject.SetActive(false);

        // CutsceneCamera starts at sleep pos
        cutsceneCamera.gameObject.SetActive(true);
        cutsceneCamera.transform.position = sleepTransform.position;
        cutsceneCamera.transform.rotation = sleepTransform.rotation;

        // Grab WakeCameraController and disable until needed
        wakeCameraController = wakeCamera.GetComponent<WakeCameraController>();
        if (wakeCameraController != null)
        {
            wakeCameraController.enabled = false;
            wakeCameraController.canLook = false;
        }

        fadeCanvas.alpha = 1f;
        StartCoroutine(PlayOpeningSequence());
    }

    IEnumerator PlayOpeningSequence()
    {
        yield return new WaitForSeconds(blackScreenDuration);

        // Start phone ringing
        phoneRingSource.clip = phoneRingClip;
        phoneRingSource.loop = true;
        phoneRingSource.Play();

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Lerp cutscene cam from sleep to wake
        yield return StartCoroutine(LerpCamera(sleepTransform, wakeTransform, wakeUpDuration));

        // Swap to wake cam
        cutsceneCamera.gameObject.SetActive(false);
        wakeCamera.gameObject.SetActive(true);

        // Enable limited look
        if (wakeCameraController != null)
        {
            wakeCameraController.enabled = true;
            wakeCameraController.canLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Show prompt, wait for E
        if (phonePrompt != null)
			Debug.Log("Showing prompt: " + (phonePrompt != null ? phonePrompt.name : "NULL"));
            phonePrompt.SetActive(true);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

        if (phonePrompt != null)
            phonePrompt.SetActive(false);

        if (wakeCameraController != null)
            wakeCameraController.canLook = false;

        AnswerPhone();
    }

    void AnswerPhone()
    {
        phoneRingSource.Stop();

        if (fokyuIntroClip != null)
        {
            fokyuVoiceSource.clip = fokyuIntroClip;
            fokyuVoiceSource.Play();
            StartCoroutine(WaitForVoiceEnd());
        }
        else
        {
            StartCoroutine(TransitionToPlayer());
        }
    }

    IEnumerator WaitForVoiceEnd()
    {
        yield return new WaitForSeconds(fokyuIntroClip.length);
        fokyuVoiceSource.Stop();
        yield return StartCoroutine(TransitionToPlayer());
    }

    IEnumerator TransitionToPlayer()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        wakeCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        employeeScript.enabled = true;
        cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        if (taskManager != null)
            taskManager.StartTasks();
    }

    IEnumerator LerpCamera(Transform from, Transform to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cutsceneCamera.transform.position = Vector3.Lerp(from.position, to.position, t);
            cutsceneCamera.transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, t);
            yield return null;
        }
        cutsceneCamera.transform.position = to.position;
        cutsceneCamera.transform.rotation = to.rotation;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        fadeCanvas.alpha = to;
    }
}