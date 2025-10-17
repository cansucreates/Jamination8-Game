using System.Collections.Generic; // for list
using UnityEngine;

[System.Serializable]
public class CustomerRequest
{
    public string potionName;
    public int requiredType; // 1: Sıvı, 2: Bitki, 3: Enerji, 4: Hayvan
    public int minInstability;
    public int maxInstability;
}

public class GameManager : MonoBehaviour
{
    // müşteri isteklerini tutan liste
    public List<CustomerRequest> allRequests = new List<CustomerRequest>();

    // card listesini tutan liste
    public List<CardData> allCards = new List<CardData>();

    // eldeki kartları tutan liste
    public List<CardData> playerCards = new List<CardData>();
    public int shopRating = 0;

    [HideInInspector]
    public CustomerRequest currentRequest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1. rastgele müşteri isteği seç
        currentRequest = allRequests[Random.Range(0, allRequests.Count)];

        // 2. rastgele 5 kart seç
        playerCards.Clear();
        List<CardData> shuffledDeck = new List<CardData>(allCards);
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, shuffledDeck.Count);
            playerCards.Add(shuffledDeck[randomIndex]);
            shuffledDeck.RemoveAt(randomIndex);
        }

        // 3. müşteri isteğini ve kartları konsola yazdır (daha sonra UI ile değiştirilecek)
        Debug.Log(
            "Müşteri İsteği: "
                + currentRequest.potionName
                + " | Tür: "
                + currentRequest.requiredType
                + " | İstikrar Aralığı: "
                + currentRequest.minInstability
                + " - "
                + currentRequest.maxInstability
        );
    }

    // Update is called once per frame
    void Update() { }
}
