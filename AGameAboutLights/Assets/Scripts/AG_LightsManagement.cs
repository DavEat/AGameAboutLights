using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_LightsManagement : MonoBehaviour {

    public LayerMask layer;

    public GameObject _light, lastHitObject;
    private List<AG_Light_Mono> listLight = new List<AG_Light_Mono>();

    [SerializeField]
    private Transform stratLightPos;
    private Vector3 _origin;
    private float angle;

    private void Start()
    {
        _origin = stratLightPos.position;
        angle = stratLightPos.eulerAngles.z;
        AddLight();
    }

    public void AddLight()
    {
        RaycastHit2D hit = Physics2D.Raycast(_origin, Quaternion.Euler(0,0,angle) * Vector2.up, Mathf.Infinity, layer);
        if (hit.collider != null)
        {
            if(lastHitObject != null)
                lastHitObject.layer = LayerMask.NameToLayer("UI");
            lastHitObject = hit.transform.gameObject;
            lastHitObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            Debug.DrawLine(hit.point, _origin, Color.blue, 20);
            Debug.DrawRay(hit.point, hit.normal, Color.green, 20);

            listLight.Add(Instantiate(_light, transform.parent).GetComponent<AG_Light_Mono>());
            listLight[listLight.Count - 1].Init(Color.red, new AG_Line(_origin, hit.point, 10));            

            Vector2 dir = (Vector2.Reflect(_origin - (Vector3)hit.point, hit.normal) - hit.point);
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            _origin = hit.point;

            Debug.DrawRay(hit.point, -_origin, Color.blue, 20);
            Debug.Log("angle : " + angle + " time : " + Time.frameCount);

            if (listLight.Count < 5 && hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
            {
                AddLight();                
            } else Debug.Log("listLight count : " + listLight.Count + " time : " + Time.frameCount);
        }
        else Debug.Log("go out" + " time : " + Time.frameCount);
    }
}
