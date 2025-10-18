using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    private bool isSelected = false;
    private Image backgroundImage;
    private GameUIController uiController;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public Image cardImage;

    void Awake()
    {
        backgroundImage = GetComponent<Image>();
        uiController = FindFirstObjectByType<GameUIController>();

        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(ToggleSelection);

        backgroundImage.color = normalColor;
    }

    public void SetCard(CardData data)
    {
        cardData = data;
        if (nameText != null)
            nameText.text = data.cardName;

        if (valueText != null)
            valueText.text = $"ID: {data.instabilityValue} | {data.GetTypeString()}";

        if (cardImage != null && data.cardImage != null)
            cardImage.sprite = data.cardImage;

        isSelected = false;
        backgroundImage.color = normalColor;
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        backgroundImage.color = isSelected ? selectedColor : normalColor;
        uiController?.OnCardSelected(this, isSelected);
    }

    public bool IsSelected() => isSelected;
}
