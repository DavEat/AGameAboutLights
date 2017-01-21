using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_LightsManagement : MonoBehaviour {

    public GameObject _light;
    private List<AG_Light_Mono> listLight = new List<AG_Light_Mono>();

    [SerializeField]
    private Transform stratLightPos;
    private Vector3 pos;
    private float angle;

    private void Start()
    {
        pos = stratLightPos.position;
        angle = stratLightPos.eulerAngles.z;
        Debug.Log("angle : " + stratLightPos.eulerAngles.z);
        AddLight();
    }

    public void AddLight()
    {
        //GameObject game = Instantiate(_light, transform.parent);
        //listLight.Add(game.GetComponent<AG_Light_Mono>());
        //listLight[listLight.Count - 1].Init(Color.red, new AG_Line(pos, hit.point, 1));

        RaycastHit2D hit = Physics2D.Raycast(pos - (Quaternion.Euler(0, 0, angle) * Vector2.up * -10), Quaternion.Euler(0,0,angle) * Vector2.up, Mathf.Infinity);
        if (hit.collider != null)
        {
            listLight.Add(Instantiate(_light, transform.parent).GetComponent<AG_Light_Mono>());
            listLight[listLight.Count - 1].Init(Color.red, new AG_Line(pos, hit.point, 3));
            pos = hit.point;
            angle = Mathf.Atan2(hit.normal.y, hit.normal.y) * Mathf.Rad2Deg;
            if (listLight.Count < 5)
                AddLight();
        }
    }
}
