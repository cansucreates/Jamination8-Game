using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CustomerRequest
{
    public string potionName;
    public MaterialType requiredType;
    public int minInstability;
    public int maxInstability;
    public Sprite TypeSprite;

    [TextArea(2, 5)]
    public string customerDialog; // Yeni alan

    public string GetTypeString()
    {
        switch (requiredType)
        {
            case MaterialType.Sıvı:
                return "Sıvı";
            case MaterialType.Bitki:
                return "Bitki";
            case MaterialType.Enerji:
                return "Enerji";
            case MaterialType.Hayvan:
                return "Hayvan";
            default:
                return "Bilinmiyor";
        }
    }
}
