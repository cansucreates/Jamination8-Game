using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Potion Jam/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int materialType; // 1: Sıvı, 2: Bitki, 3: Enerji 4: Hayvan
    public int instabilityValue; // +3, -1, 0 etc.
    public Sprite cardImage;

    public string GetTypeString()
    {
        switch (materialType)
        {
            case 1:
                return "Sivi";
            case 2:
                return "Bitki";
            case 3:
                return "Enerji";
            case 4:
                return "Hayvan";
            default:
                return "Bilinmiyor";
        }
    }
}
