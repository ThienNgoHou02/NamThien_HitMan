using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBossAppear : UICanvas
{
    Coroutine _crtHide;
    private void OnEnable()
    {
        if (_crtHide != null)
        {
            StopCoroutine(_crtHide);
        }
        _crtHide = StartCoroutine(Hide(1.1f));
    }
    IEnumerator Hide(float time)
    {
        yield return Cache.GetWFSRealTimeByTime(time);
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        Close(0);
    }
}
