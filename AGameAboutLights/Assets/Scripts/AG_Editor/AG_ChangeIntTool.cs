using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AG_ChangeIntTool : MonoBehaviour {

    public AG_InventoryObjectManager target;

    public RectTransform _rect { get; private set; }

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform _transform)
    {
        AG_InventoryObjectManager inventortObj = _transform.GetComponent<AG_InventoryObjectManager>();
        if (inventortObj != null)
        {
            if (target != inventortObj)
            {
                _rect.position = inventortObj.text.transform.position;
                target = inventortObj;

                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
            } else gameObject.SetActive(!gameObject.activeSelf);            
        }        
    }

    public void Click(int axis)
    {
        int value = (int)target.maxNumber;

        value += axis;
        if (value >= 0 && value <= 99)
        {
            target.maxNumber = value;
            target.UpdateDisplayNumber();
        }
    }
}
