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
        requestText.text =
            $"İSTEK: {req.potionName}\n"
            + $"Tür: {req.GetTypeString()} (Min 2 aynı türden içerik seçmelisin)\n"
            + $"Instabilite: {req.minInstability} - {req.maxInstability} arasında olmalıdır.";
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
