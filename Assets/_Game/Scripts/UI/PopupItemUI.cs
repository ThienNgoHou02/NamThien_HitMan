using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PopupItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemText;
    Coroutine _coroutine;
    private void OnEnable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(Hide(1.0f));        
    }
    public void SetText(string text)
    {
        _itemText.text = text;
    }
    IEnumerator Hide(float time)
    {
        yield return Cache.GetWFSRealTimeByTime(time);
        gameObject.SetActive(false);
    }
}
