using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    private GameManager gameManager;

    public CardDisplay[] cardSlots = new CardDisplay[5];
    public TextMeshProUGUI requestText;
    public TextMeshProUGUI shopRatingText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI dialogText;
    public Image TypeImage;
    public Button submitButton;

    [Header("Seçilen Kart Önizlemeleri")]
    public Image[] previewSlots = new Image[3];

    [Header("Karıştırma Sesi (Opsiyonel)")]
    public AudioSource audioSource;
    public AudioClip stirSound;

    private List<CardDisplay> selectedCards = new List<CardDisplay>();
    private Vector2[] originalPositions;
    private Vector3[] originalScales;
    private CanvasGroup[] canvasGroups;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);

        if (gameManager != null && gameManager.currentRequest != null)
            UpdateUI();

        // Önizleme slotlarının başlangıç durumlarını kaydet
        originalPositions = new Vector2[previewSlots.Length];
        originalScales = new Vector3[previewSlots.Length];
        canvasGroups = new CanvasGroup[previewSlots.Length];

        for (int i = 0; i < previewSlots.Length; i++)
        {
            if (previewSlots[i] != null)
            {
                RectTransform rt = previewSlots[i].rectTransform;
                originalPositions[i] = rt.anchoredPosition;
                originalScales[i] = rt.localScale;
                CanvasGroup cg = previewSlots[i].GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = previewSlots[i].gameObject.AddComponent<CanvasGroup>();
                canvasGroups[i] = cg;
                cg.alpha = 0f; // Başlangıçta görünmez
            }
        }
    }

    public void UpdateUI()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null || gameManager.currentRequest == null)
            return;

        CustomerRequest req = gameManager.currentRequest;

        if (requestText != null)
        {
            requestText.text =
                $"İSTEK: {req.potionName}\n"
                + $"Tür: {req.GetTypeString()} \n (Minimum 2 aynı tür)\n"
                + $"Denge Değeri: {req.minInstability} - {req.maxInstability}";
        }

        if (req.TypeSprite != null)
            TypeImage.sprite = req.TypeSprite;

        if (dialogText != null)
            dialogText.text = $"Müşteri: \"{req.customerDialog}\"";

        shopRatingText.text = $"DÜKKAN PUANI: {gameManager.shopRating}";

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < gameManager.playerCards.Count)
            {
                cardSlots[i].SetCard(gameManager.playerCards[i]);
                cardSlots[i].gameObject.SetActive(true);
            }
            else
                cardSlots[i].gameObject.SetActive(false);
        }

        selectedCards.Clear();
        if (submitButton != null)
            submitButton.interactable = false;
    }

    public void UpdateTimer(int seconds)
    {
        if (timerText != null)
            timerText.text = $"Süre: {seconds}";
    }

    public void ShowCustomerMessage(string message)
    {
        if (messageText == null)
            return;

        messageText.text = message;
        StartCoroutine(ClearMessageAfterDelay());
    }

    private IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (messageText != null)
            messageText.text = "";
    }

    public void OnCardSelected(CardDisplay card, bool isSelected)
    {
        if (isSelected)
            selectedCards.Add(card);
        else
            selectedCards.Remove(card);

        if (submitButton != null)
            submitButton.interactable = selectedCards.Count == 3;

        UpdatePreviewSlots();
    }

    private void UpdatePreviewSlots()
    {
        for (int i = 0; i < previewSlots.Length; i++)
        {
            Image slot = previewSlots[i];
            if (slot == null)
                continue;

            if (i < selectedCards.Count && selectedCards[i].cardData != null)
            {
                slot.sprite = selectedCards[i].cardData.cardImage;
                slot.enabled = true;

                CanvasGroup cg = canvasGroups[i];
                StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, 0.3f));
            }
            else
            {
                CanvasGroup cg = canvasGroups[i];
                cg.alpha = 0f;
                slot.enabled = false;
            }
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    private void OnSubmitClicked()
    {
        // Ses efekti (opsiyonel)
        if (audioSource && stirSound)
            audioSource.PlayOneShot(stirSound);

        List<CardData> selectedData = new List<CardData>();
        foreach (var c in selectedCards)
            if (c.cardData != null)
                selectedData.Add(c.cardData);

        StartCoroutine(FadeOutAndMerge(0.7f, selectedData));
    }

    private IEnumerator FadeOutAndMerge(float duration, List<CardData> selectedData)
    {
        float elapsed = 0f;

        // Ortak birleşme noktası (3 preview'ün ortası)
        Vector2 targetPos = Vector2.zero;
        Vector3 targetScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < previewSlots.Length; i++)
            {
                Image slot = previewSlots[i];
                if (slot == null || slot.sprite == null)
                    continue;

                RectTransform rt = slot.rectTransform;
                CanvasGroup cg = canvasGroups[i];

                // Ortaya hareket (container merkezine)
                rt.anchoredPosition = Vector2.Lerp(originalPositions[i], targetPos, t);
                rt.localScale = Vector3.Lerp(originalScales[i], targetScale, t);
                cg.alpha = 1f - t;
            }

            yield return null;
        }

        // Resetle
        for (int i = 0; i < previewSlots.Length; i++)
        {
            previewSlots[i].sprite = null;
            previewSlots[i].enabled = false;
            canvasGroups[i].alpha = 0f;

            RectTransform rt = previewSlots[i].rectTransform;
            rt.anchoredPosition = originalPositions[i];
            rt.localScale = originalScales[i];
        }

        // Kart gönderimi (tam animasyon bittiğinde)
        gameManager?.SubmitPotion(selectedData);
    }
}
