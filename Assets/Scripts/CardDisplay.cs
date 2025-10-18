using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    private bool isSelected = false;
    private Button button;
    private GameUIController uiController;

    [Header("Renk Ayarları")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("UI Referansları")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public Image cardImage;

    void Awake()
    {
        button = GetComponent<Button>();
        uiController = FindFirstObjectByType<GameUIController>();

        if (button != null)
            button.onClick.AddListener(ToggleSelection);

        SetButtonColor(normalColor);
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
        SetButtonColor(normalColor);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        SetButtonColor(isSelected ? selectedColor : normalColor);
        uiController?.OnCardSelected(this, isSelected);
    }

    private void SetButtonColor(Color color)
    {
        if (button == null)
            return;

        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    public bool IsSelected() => isSelected;
}
