using UnityEngine;
using UnityEngine.UI;

public class FokyuPC : MonoBehaviour
{
    public GameObject updateScreen; // optional: a canvas/image that shows "Updating..."
    
    public void TrySubmit()
    {
        if (!GameManager.Instance.hasDocument) return;
        if (GameManager.Instance.documentSubmitted) return;

        GameManager.Instance.documentSubmitted = true;
        if (updateScreen != null)
            updateScreen.SetActive(true);

        Debug.Log("Document submitted to PC!");
        // add whatever happens next - task complete, door unlocks, etc.
    }
}