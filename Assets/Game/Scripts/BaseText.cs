using TMPro;
using UnityEngine;

public class BaseText : BaseMonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;

    protected virtual void Awake()
    {
        if (!ValidateReference(text, nameof(text)))
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    protected virtual void UpdateText(string newText)
    {
        if (text == null) return;
        if (text.text == newText) return;
        text.SetText(newText);
    }
}