using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Grid : MonoBehaviour {

    #region Var
    [SerializeField]
    private int numberX = 20, numberY = 11;
    private int startgapX = 285, startgapY = 90;
    [SerializeField]
    private int gap = 90;
    [SerializeField]
    private GameObject point;

    private static List<Transform> _listPoints = new List<Transform>();
    #endregion

    public static List<Transform> listPoints
    {
        get { return _listPoints; }
        private set { _listPoints = value; }
    }

	void Start ()
    {
        if (transform.childCount == 0)
            CreateGried();
        else foreach (Transform t in transform.GetComponentsInChildren<Transform>())
                listPoints.Add(t);
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

    public static Vector2 ChoseClosestPoint(Vector2 pos)
    {
        if (listPoints != null)
        {
            if (listPoints.Count == 1)
                return listPoints[0].position;
            else if (listPoints != null && listPoints.Count > 1)
            {
                Transform lastSelected = listPoints[0];
                for (int i = 1; i < listPoints.Count; i++)
                {
                    if (Vector2.Distance(lastSelected.position, pos) > Vector2.Distance(listPoints[i].position, pos))
                        lastSelected = listPoints[i];
                }
                return lastSelected.position;
            }
        }
        return Vector2.zero;
    }

    public static Vector2 ChoseClosestPoint(List<Transform> list, Vector2 pos)
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
