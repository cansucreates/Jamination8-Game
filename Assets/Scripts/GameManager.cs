using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<CustomerRequest> allRequests = new List<CustomerRequest>();
    public List<CardData> allCards = new List<CardData>();

    public List<CardData> playerCards = new List<CardData>();
    public int shopRating = 0;

    [Header("Timer Settings")]
    public float roundTime = 30f; // saniye cinsinden
    private float timeRemaining;
    private bool timerRunning = false;

    private GameUIController uiController;

    [HideInInspector]
    public CustomerRequest currentRequest;

    void Awake()
    {
        uiController = FindFirstObjectByType<GameUIController>();
        StartNewRound();
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            uiController?.UpdateTimer(Mathf.CeilToInt(timeRemaining));

            if (timeRemaining <= 0f)
            {
                TimerExpired();
            }
        }
    }

    public void StartNewRound()
    {
        StopAllCoroutines();
        timerRunning = false;

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

        // Yeni müşteri seç
        currentRequest = allRequests[Random.Range(0, allRequests.Count)];
        playerCards.Clear();

        // Kartları ekleme işlemleri...
        List<CardData> typeCards = allCards.FindAll(c =>
            c.materialType == currentRequest.requiredType
        );
        for (int i = 0; i < 2 && typeCards.Count > 0; i++)
        {
            int idx = Random.Range(0, typeCards.Count);
            playerCards.Add(typeCards[idx]);
            typeCards.RemoveAt(idx);
        }

        List<CardData> remainingCards = new List<CardData>(allCards);
        remainingCards.RemoveAll(c => playerCards.Contains(c));
        for (int i = playerCards.Count; i < 5; i++)
        {
            int idx = Random.Range(0, remainingCards.Count);
            playerCards.Add(remainingCards[idx]);
            remainingCards.RemoveAt(idx);
        }

        // Debug.Log
        Debug.Log($"Yeni tur başladı! Müşteri isteği: {currentRequest.potionName}");

        // UI güncelle
        uiController = uiController ?? FindFirstObjectByType<GameUIController>();
        uiController?.UpdateUI();

        // **UI üzerinden kısa mesaj göstermek**
        uiController?.ShowCustomerMessage(
            $"Yeni müşteri geldi! {currentRequest.potionName} istiyor."
        );

        // Timer başlat
        timeRemaining = roundTime;
        timerRunning = true;
        uiController?.UpdateTimer(Mathf.CeilToInt(timeRemaining));
    }

    public void SubmitPotion(List<CardData> selectedCards)
    {
        if (selectedCards == null || selectedCards.Count != 3)
            return;

        timerRunning = false;

        int totalInstability = 0;
        int typeMatchCount = 0;
        foreach (CardData card in selectedCards)
        {
            totalInstability += card.instabilityValue;
            if (card.materialType == currentRequest.requiredType)
                typeMatchCount++;
        }

        int ratingChange = 0;
        string warningMessage = ""; // messageText için
        string customerDialogue = ""; // dialogText için

        bool typeSuccess = (typeMatchCount >= 2);
        bool instabilitySuccess =
            totalInstability >= currentRequest.minInstability
            && totalInstability <= currentRequest.maxInstability;

        if (!typeSuccess)
        {
            ratingChange = -2;
            warningMessage =
                $"İksir türü müşterinin istediğinden değil! ({typeMatchCount}/2) Dükkan popülaritesi: -2 .";
            customerDialogue = "Müşteri: Bu istemediğim bir tür!";
        }
        else if (!instabilitySuccess)
        {
            ratingChange = -1;
            if (totalInstability < currentRequest.minInstability)
            {
                warningMessage = "İksir çok zayıf! -1 Dükkan popülaritesi.";
                customerDialogue = "Müşteri: Çok zayıf, bunun bir etkisi olmaz ki!";
            }
            else
            {
                warningMessage = "İksir çok güçlü, riskli! -1 Dükkan popülaritesi.";
                customerDialogue = "Müşteri: Bu çok riskli, içemem!";
            }
        }
        else
        {
            ratingChange = +2;
            warningMessage = "Mükemmel! +2 Dükkan popülaritesi.";
            customerDialogue = "Müşteri: Mükemmel, tam istediğim gibi!";
        }

        shopRating += ratingChange;

        // UI'ya gönder
        uiController?.ShowCustomerMessage(warningMessage); // messageText
        uiController?.SetCustomerDialogue(customerDialogue); // dialogText

        // 3 saniye bekle ve yeni round başlat
        StartCoroutine(WaitAndNextRound(3f));
    }

    private IEnumerator WaitAndNextRound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StartNewRound();
    }

    private void TimerExpired()
    {
        timerRunning = false;
        shopRating -= 5;
        Debug.Log("⏰ Süre doldu! Dükkan puanı -5");

        uiController?.ShowCustomerMessage("Müşteri sinirlendi ve gitti! -5 Dükkan puanı.");
        uiController?.SetCustomerDialogue("Müşteri: Ne biçim iksir dükkanı bu!"); // Diyalog da değişir
        StartCoroutine(WaitAndNextRound(3f));
    }
}
