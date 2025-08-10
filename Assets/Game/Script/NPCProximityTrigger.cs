using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCProximityTrigger : MonoBehaviour
{
    public GameObject notificationPanel;
    public GameObject npc;
    public Button continueButton;
    public CanvasGroup panelCanvasGroup;
    public GameObject thisTriggerZone; // TriggerZone1
    public GameObject nextTriggerZone; // TriggerZone2
    public Transform nextSpawnPoint;   // Lokacija na koju će se NPC pomjeriti
    public string playerTag = "Player";
    public float fadeDuration = 0.01f;

    private bool isPanelActive = false;

    void Start()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);

        if (npc != null)
            npc.SetActive(true); // NPC vidljiv na prvoj lokaciji

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinuePressed);

        if (nextTriggerZone != null)
            nextTriggerZone.SetActive(false); // Druga zona isključena na početku
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isPanelActive)
        {
            Debug.Log("Player entered first trigger zone");

            if (notificationPanel != null)
                notificationPanel.SetActive(true);

            if (panelCanvasGroup != null)
                StartCoroutine(FadeCanvas(panelCanvasGroup, 0f, 1f));

            Time.timeScale = 0;
            isPanelActive = true;
        }
    }

    public void OnContinuePressed()
    {
        Debug.Log("Continue pressed – transitioning to next stage");

        if (panelCanvasGroup != null)
            StartCoroutine(FadeOutAndTransition());
        else
        {
            TransitionToNextStage();
        }
    }

    private IEnumerator FadeOutAndTransition()
    {
        yield return FadeCanvas(panelCanvasGroup, 1f, 0f);
        TransitionToNextStage();
    }

    private void TransitionToNextStage()
    {
        // Sakrij panel
        if (notificationPanel != null)
            notificationPanel.SetActive(false);

        // Premjesti NPC
        if (npc != null && nextSpawnPoint != null)
        {
            npc.transform.position = nextSpawnPoint.position;
            npc.SetActive(false); 
        }

        // Aktiviraj sljedeću trigger zonu
        if (nextTriggerZone != null)
            nextTriggerZone.SetActive(true);

        // Deaktiviraj trenutnu trigger zonu
        if (thisTriggerZone != null)
            thisTriggerZone.SetActive(false);

        Time.timeScale = 1;
        isPanelActive = false;
    }

    private IEnumerator FadeCanvas(CanvasGroup canvas, float from, float to)
    {
        float timer = 0f;
        canvas.alpha = from;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvas.alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }

        canvas.alpha = to;
    }
}
