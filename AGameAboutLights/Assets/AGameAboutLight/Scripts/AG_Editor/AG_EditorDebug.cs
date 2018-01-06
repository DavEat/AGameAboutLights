using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AG_EditorDebug : MonoBehaviour {

    private static Text text;
    private static RectTransform _rect;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        text = _rect.GetChild(3).GetComponent<Text>();
    }    

    /// <summary>use to display a debug message on the ag_editor</summary>
    /// <param name="log">the text of the debug</param>
    public static void DebugLog(string log)
    {
        text.text = log;
        OpenMenu(_rect);
    }

    #region Animation Functions
    private static float openTime = 0.4f, closeTime = 0.25f, showingTime = 3f;

    private static void OpenMenu(RectTransform rect)
    {
        Sequence inTween = DOTween.Sequence();
        inTween.Append(rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x, 0), openTime))
               .AppendInterval(showingTime)
               .OnComplete(() => { CloseMenu(rect); });
        inTween.Play();
    }
    private static void CloseMenu(RectTransform rect)
    {
        Debug.Log("close : " + rect.name);
        Sequence inTween = DOTween.Sequence();
        inTween.Append(rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x, rect.sizeDelta.y + 20), closeTime));
        inTween.Play();
    }
    #endregion
}
