using UnityEngine;
using UnityEngine.UI;

public class WaveUI : BaseText
{
    [SerializeField] private Image waveImage;
    [SerializeField] private WaveManager waveManager;

    private void Awake()
    {
        ValidateReference(waveImage, nameof(waveImage));
        ValidateReference(waveManager, nameof(waveManager));
    }

    private void OnEnable()
    {
        if (!ValidateReference(waveManager, nameof(waveManager)))
            return;

        waveManager.OnWaveStarted += UpdateWaveValue;
        waveManager.OnWaveTimeChanged += UpdateImage;
        waveManager.OnBreakTimeChanged += UpdateImage;
        waveManager.OnBreakStarted += UpdateText;
    }

    private void OnDisable()
    {
        if (!ValidateReference(waveManager, nameof(waveManager)))
            return;

        waveManager.OnWaveStarted -= UpdateWaveValue;
        waveManager.OnWaveTimeChanged -= UpdateImage;
        waveManager.OnBreakTimeChanged -= UpdateImage;
        waveManager.OnBreakStarted -= UpdateText;
    }

    private void UpdateWaveValue(int wave)
    {
        if (text == null) return;

        text.SetText($"Wave: {wave}");
    }

    private void UpdateText()
    {
        text.SetText("Break");
    }

    private void UpdateImage(float value)
    {
        if (waveImage == null)
            return;

        waveImage.fillAmount = value;
    }
}