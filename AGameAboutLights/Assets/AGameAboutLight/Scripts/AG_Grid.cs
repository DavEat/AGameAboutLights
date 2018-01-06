using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Grid : AG_Singleton<AG_Grid> {

    #region Var
    [SerializeField]
    private int numberX = 20, numberY = 11;
    //private int startgapX = 285, startgapY = 90;
    //[SerializeField] private int gap = 90;
    [SerializeField] private GameObject point;

    private static Transform[] _listPoints;
    #endregion

    public Transform[] listPoints
    {
        get { return _listPoints; }
        private set { _listPoints = value; }
    }

	void Start ()
    {
        if (transform.childCount == 0)
            CreateGried();
        else
        {
            List<Transform> listPoint = new List<Transform>();
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
                listPoint.Add(t);

            listPoints = listPoint.ToArray();
        }


    }

    private void CreateGried()
    {
        List<Transform> listPoint = new List<Transform>();
        for (int i = 0; i < numberX; i++)
        {
            for (int j = 0; j < numberY; j++)
            {
                GameObject _point = Instantiate(point, transform);
                listPoint.Add(_point.GetComponent<RectTransform>());
                //_point.transform.localPosition = new Vector2(i * gap + startgapX, j * gap + startgapY); 
            }
        }
        listPoints = listPoint.ToArray();
        gameObject.SetActive(false);
    }

    public Vector2 ChoseClosestPoint(Vector2 pos)
    {
        if (listPoints != null)
        {
            if (listPoints.Length == 1)
                return listPoints[0].position;
            else if (listPoints != null && listPoints.Length > 1)
            {
                Transform lastSelected = listPoints[0];
                for (int i = 1; i < listPoints.Length; i++)
                {
                    if (Vector2.Distance(lastSelected.position, pos) >Vector2.Distance(listPoints[i].position, pos))
                        lastSelected = listPoints[i];
                }
                return lastSelected.position;
            }
        }
        return Vector2.zero;
    }

    public Vector2 ChoseClosestPoint(List<Transform> list, Vector2 pos)
    {
        if (list != null)
        {
            if (list.Count == 1)
                return list[0].position;
            else if (list != null && list.Count > 1)
            {
                Transform lastSelected = list[0];
                for (int i = 1; i < list.Count; i++)
                {
                    if (Vector2.Distance(lastSelected.position, pos) > Vector2.Distance(list[i].position, pos))
                        lastSelected = list[i];
                }
                return lastSelected.position;
            }
        }
        return Vector2.zero;
    }
}
