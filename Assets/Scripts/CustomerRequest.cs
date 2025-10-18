using UnityEngine;

[System.Serializable]
public class CustomerRequest
{
    public string potionName;
    public MaterialType requiredType;
    public int minInstability;
    public int maxInstability;

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
