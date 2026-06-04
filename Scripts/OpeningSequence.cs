using UnityEngine;
using System.Collections;

public class OpeningSequence : MonoBehaviour
{
    [Header("Cameras")]
    public Camera cutsceneCamera;       // Separate cutscene cam at desk
    public Camera playerCamera;         // Main cam (child of Employee) — disabled at start

    [Header("Cutscene Cam Positions")]
    public Transform sleepTransform;    // Empty GO: face-down position at desk
    public Transform wakeTransform;     // Empty GO: lifted/upright position

    [Header("Timing")]
    public float sleepHoldDuration = 1.5f;      // how long to stay face-down
    public float wakeUpDuration = 3f;            // how long the head lift takes
    public float afterWakePause = 1.5f;          // pause before Fokyu placeholder
    public float fokyuPlaceholderDuration = 2f;  // placeholder wait (replace with Fokyu later)
    public float fadeDuration = 1f;              // fade out + fade in duration

    [Header("Fade")]
    public CanvasGroup fadeCanvas;      // Black UI canvas covering screen

    [Header("Player")]
    public Employee employeeScript;
    public CameraController cameraController;

    void Start()
    {
        // Disable player control
        employeeScript.enabled = false;
        cameraController.enabled = false;
        playerCamera.gameObject.SetActive(false);

        // Set cutscene cam to sleep position
        cutsceneCamera.gameObject.SetActive(true);
        cutsceneCamera.transform.position = sleepTransform.position;
        cutsceneCamera.transform.rotation = sleepTransform.rotation;

        // Start fully faded in (black), then fade out to reveal scene
        fadeCanvas.alpha = 1f;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Fade in from black
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Hold sleep position
        yield return new WaitForSeconds(sleepHoldDuration);

        // Lift head — lerp cutscene cam from sleep to wake transform
        float elapsed = 0f;
        while (elapsed < wakeUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / wakeUpDuration);
            cutsceneCamera.transform.position = Vector3.Lerp(
                sleepTransform.position, wakeTransform.position, t);
            cutsceneCamera.transform.rotation = Quaternion.Lerp(
                sleepTransform.rotation, wakeTransform.rotation, t);
            yield return null;
        }

        // Snap to wake position
        cutsceneCamera.transform.position = wakeTransform.position;
        cutsceneCamera.transform.rotation = wakeTransform.rotation;

        yield return new WaitForSeconds(afterWakePause);

        // --- FOKYU PLACEHOLDER ---
        // Phase 2: trigger Fokyu entrance here
        // e.g. FokyuController.Instance.TriggerEntrance();
        Debug.Log("[OpeningSequence] Fokyu would enter here.");
        yield return new WaitForSeconds(fokyuPlaceholderDuration);
        // -------------------------

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Switch to player camera
        cutsceneCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Give player control
        employeeScript.enabled = true;
        cameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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