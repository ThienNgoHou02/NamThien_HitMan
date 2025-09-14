using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasVictory : UICanvas
{
    [Header("Button")]
    [SerializeField] private Button _continueButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _rewardText;

    Coroutine _crtShow;
    private void OnEnable()
    {
        if (_crtShow != null)
        {
            StopCoroutine(_crtShow);
        }
        _crtShow = StartCoroutine(ShowButton(1f));
    }
    private void BackToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void Reward(int reward)
    {
        _rewardText.text = $"Reward: {reward}$";
    }
    IEnumerator ShowButton(float time)
    {
        yield return Cache.GetWFSByTime(time);
        _continueButton.onClick.AddListener(BackToMenu);
        _continueButton.gameObject.SetActive(true);
    }
}
