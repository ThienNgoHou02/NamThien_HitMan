using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasDefeat : UICanvas
{
    [Header("Button")]
    [SerializeField] private Button _continueButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private TextMeshProUGUI _enemyDefeatText;

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
    public void Reward(int reward, int enemyDefeat)
    {
        _rewardText.text = $"Reward: {reward}$";
        _enemyDefeatText.text = $"Enemies defeated: {enemyDefeat}";
    }
    IEnumerator ShowButton(float time)
    {
        yield return Cache.GetWFSByTime(time);
        _continueButton.onClick.AddListener(BackToMenu);
        _continueButton.gameObject.SetActive(true);
    }
}
