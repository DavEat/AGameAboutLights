using UnityEngine;
using DG.Tweening;

public class AG_MenuEditorManager : MonoBehaviour
{
    #region Var
    private RectTransform _rect;
    public RectTransform gear;    
	#endregion

	#region Struct

	#endregion

	#region MonoFunction
	void Awake ()
	{
		
	}

	void Start ()
	{
        _rect = GetComponent<RectTransform>();
    }
	
	void Update () 
	{
		
	}
    #endregion

    #region Function

    #endregion

    #region Animations
    private bool menuOpen;
    public float menuOpenLenght, menuCloseLenght, openTime = 0.6f;
    public void ToogleMenu()
    {
        Sequence inTween = DOTween.Sequence();
        if (menuOpen)
        {
            if (saveOpen)
            {
                inTween.Append(_rect.DOSizeDelta(new Vector2(_rect.sizeDelta.x, saveCloseLenght), openSaveTime * 0.3f));
                saveOpen = false;

                inTween.Append(gear.DORotate(new Vector3(0, 0, 0), openTime * 0.7f));
                inTween.Join(_rect.DOSizeDelta(new Vector2(menuCloseLenght, saveCloseLenght), openTime * 0.7f));
            }
            else
            {
                inTween.Append(gear.DORotate(new Vector3(0, 0, 0), openTime * 0.7f));
                inTween.Join(_rect.DOSizeDelta(new Vector2(menuCloseLenght, _rect.sizeDelta.y), openTime * 0.7f));
            }
            menuOpen = false;
        }
        else
        {
            inTween.Append(gear.DORotate(new Vector3(0, 0, 270), openTime));
            inTween.Join(_rect.DOSizeDelta(new Vector2(menuOpenLenght, _rect.sizeDelta.y), openTime));
            menuOpen = true;            
        }
        
    }
    private bool saveOpen;
    public float saveOpenLenght, saveCloseLenght, openSaveTime = 0.6f;
    public void ToogleSaveMenu()
    {
        Sequence inTween = DOTween.Sequence();
        if (saveOpen)
        {
            inTween.Append(_rect.DOSizeDelta(new Vector2(_rect.sizeDelta.x, saveCloseLenght), openSaveTime * 0.7f));
            saveOpen = false;
        }
        else
        {
            inTween.Append(_rect.DOSizeDelta(new Vector2(_rect.sizeDelta.x, saveOpenLenght), openSaveTime));
            saveOpen = true;
        }

    }
    #endregion
}
