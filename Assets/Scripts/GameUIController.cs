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
    public TextMeshProUGUI messageText; // Uyarı mesajları
    public TextMeshProUGUI dialogText; // Müşteri diyaloğu

    public Button submitButton;

    private List<CardDisplay> selectedCards = new List<CardDisplay>();

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);

        if (gameManager != null && gameManager.currentRequest != null)
            UpdateUI();
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
                + $"Tür: {req.GetTypeString()} (Minimum 2 aynı tür)\n"
                + $"Instabilite: {req.minInstability} - {req.maxInstability}";
        }

        // Başlangıçta müşteri normal diyalogu
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

    // messageText için uyarı mesajlarını göster
    public void ShowCustomerMessage(string message)
    {
        if (messageText == null)
            return;

        messageText.text = message;
        StartCoroutine(ClearMessageAfterDelay());
    }

    // dialogText’i değiştirmek için
    public void SetCustomerDialogue(string dialogue)
    {
        if (dialogText != null)
            dialogText.text = dialogue;
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
    }

    private void OnSubmitClicked()
    {
        List<CardData> selectedData = new List<CardData>();
        foreach (var c in selectedCards)
            if (c.cardData != null)
                selectedData.Add(c.cardData);

        gameManager?.SubmitPotion(selectedData);
    }
}
