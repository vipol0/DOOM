using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    [SerializeField] private PlayerMove player;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        player.PlayerVelocityChanged += UpdateText;
    }

    private void OnDisable()
    {
        player.PlayerVelocityChanged -= UpdateText;
    }

    private void UpdateText(float playerVelocity)
    {
        if (text == null)
        {
            Debug.LogWarning($"[{name}] has no {nameof(TextMeshProUGUI)} set");
            return;
        }

        text.text = $"Speed: {playerVelocity:F1}";
    }
}