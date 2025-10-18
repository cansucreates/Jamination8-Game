using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<CustomerRequest> allRequests = new List<CustomerRequest>();
    public List<CardData> allCards = new List<CardData>();

    public List<CardData> playerCards = new List<CardData>();
    public int shopRating = 0;

    private GameUIController uiController;

    [HideInInspector]
    public CustomerRequest currentRequest;

    void Awake()
    {
        uiController = FindFirstObjectByType<GameUIController>();
        StartNewRound();
    }

    public void StartNewRound()
    {
        if (allRequests == null || allRequests.Count == 0)
        {
            Debug.LogError("allRequests boş! Inspector'da isteklerin ekli olduğundan emin ol.");
            return;
        }

        if (allCards == null || allCards.Count < 5)
        {
            Debug.LogError("allCards eksik! En az 5 kart olmalı.");
            return;
        }

        // 1️⃣ Rastgele bir müşteri isteği seç
        currentRequest = allRequests[Random.Range(0, allRequests.Count)];

        playerCards.Clear();

        // 2️⃣ İstenen türden en az 2 kart garanti
        List<CardData> typeCards = allCards.FindAll(c =>
            c.materialType == currentRequest.requiredType
        );
        if (typeCards.Count < 2)
        {
            Debug.LogWarning(
                "Yeterli sayıda istenen tür kartı yok! Tüm available tür kartları ekliyoruz."
            );
        }

        for (int i = 0; i < 2 && typeCards.Count > 0; i++)
        {
            int idx = Random.Range(0, typeCards.Count);
            playerCards.Add(typeCards[idx]);
            typeCards.RemoveAt(idx);
        }

        // 3️⃣ Kalan kartları rastgele ekle
        List<CardData> remainingCards = new List<CardData>(allCards);
        remainingCards.RemoveAll(c => playerCards.Contains(c));

        for (int i = playerCards.Count; i < 5; i++)
        {
            int idx = Random.Range(0, remainingCards.Count);
            playerCards.Add(remainingCards[idx]);
            remainingCards.RemoveAt(idx);
        }

        Debug.Log("Yeni tur başladı! Müşteri isteği: " + currentRequest.potionName);

        // 4️⃣ UI güncellemesini çağır
        uiController = uiController ?? FindFirstObjectByType<GameUIController>();
        uiController?.UpdateUI();
    }

    public void SubmitPotion(List<CardData> selectedCards)
    {
        if (selectedCards == null || selectedCards.Count != 3)
        {
            Debug.Log("Lütfen tam olarak 3 kart seçin!");
            return;
        }

        int totalInstability = 0;
        int typeMatchCount = 0;
        foreach (CardData card in selectedCards)
        {
            totalInstability += card.instabilityValue;
            if (card.materialType == currentRequest.requiredType)
                typeMatchCount++;
        }

        int ratingChange = 0;
        bool typeSuccess = (typeMatchCount >= 2);
        bool instabilitySuccess =
            totalInstability >= currentRequest.minInstability
            && totalInstability <= currentRequest.maxInstability;

        if (!typeSuccess)
        {
            ratingChange = -2;
            Debug.Log($"Tür eşleşmedi! ({typeMatchCount}/2)");
        }
        else if (!instabilitySuccess)
        {
            ratingChange = -1;
            Debug.Log($"Instabilite dışında. ({totalInstability})");
        }
        else
        {
            ratingChange = +2;
            Debug.Log($"Başarılı iksir! Instabilite: {totalInstability}");
        }

        shopRating += ratingChange;
        Debug.Log("Yeni Puan: " + shopRating);
        StartNewRound();
    }
}
