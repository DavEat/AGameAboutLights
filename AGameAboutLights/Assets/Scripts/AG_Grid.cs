using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Grid : MonoBehaviour {

    #region Var
    [SerializeField]
    private int numberX = 20, numberY = 11;
    private int startgapX = 105, startgapY = 90;
    [SerializeField]
    private int gap = 90;
    [SerializeField]
    private GameObject point;

    List<Transform> _listPoints = new List<Transform>();
    #endregion

    public List<Transform> listPoints
    {
        get { return _listPoints; }
        private set { _listPoints = value; }
    }

	void Start ()
    {
        CreateGried();
    }

    private void CreateGried()
    {
        for (int i = 0; i < numberX; i++)
        {
            for (int j = 0; j < numberY; j++)
            {
                GameObject _point = Instantiate(point, transform);
                _listPoints.Add(_point.transform);
                _point.transform.localPosition = new Vector2(i * gap + startgapX, j * gap + startgapY); 
            }
        }
        gameObject.SetActive(false);
    }
}
