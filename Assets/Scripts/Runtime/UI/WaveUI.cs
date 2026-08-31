using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    #region 인스펙터
    [Header("웨이브")]
    [SerializeField] private WaveManager _waveManager;

    [Header("UI")]
    [SerializeField] private TMP_Text _waveText;
    #endregion

    private void OnEnable()
    {
        if (_waveManager == null)
        {
            return;
        }

        _waveManager.WaveChanged += UpdateWaveText;
    }

    private void OnDisable()
    {
        if (_waveManager == null)
        {
            return;
        }

        _waveManager.WaveChanged -= UpdateWaveText;
    }

    private void Start()
    {
        UpdateWaveText(_waveManager.CurrentWave);
    }

    private void UpdateWaveText(int wave)
    {
        if (_waveText == null)
        {
            return;
        }

        _waveText.text = $"WAVE {wave}";
    }
}