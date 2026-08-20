using UnityEngine;
using TMPro;

public class AmmoText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(int currentAmmo,  int maxAmmo, int reserveAmmo, bool isReload)
    {
        if (text == null)
        {
            Debug.LogWarning($"[{name}] has no {nameof(TextMeshProUGUI)} set");
            return;
        }

        text.text = isReload ? $"..." : $"{currentAmmo}/{maxAmmo} \n {reserveAmmo}";
    }
}
