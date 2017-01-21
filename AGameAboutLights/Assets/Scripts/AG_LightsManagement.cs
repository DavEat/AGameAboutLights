using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_LightsManagement : MonoBehaviour {

    public LayerMask layer;

    [SerializeField]
    private int maxLineOfLine = 10;

    public GameObject _light, lastHitObject;
    private List<AG_Light_Mono> listLight = new List<AG_Light_Mono>();

    [SerializeField]
    private Transform stratLightPos;
    private Vector2 _origin, _direction;
    private float angle;

    private void Start()
    {
        _origin = stratLightPos.position;        
        angle = stratLightPos.eulerAngles.z;
        _direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
        AddLight();
    }

    public void AddLight()
    {
        RaycastHit2D hit = Physics2D.Raycast(_origin, _direction, Mathf.Infinity, layer);
        if (hit.collider != null)
        {
            if(lastHitObject != null)
                lastHitObject.layer = LayerMask.NameToLayer("UI");
            lastHitObject = hit.transform.gameObject;
            lastHitObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            listLight.Add(Instantiate(_light, transform.parent).GetComponent<AG_Light_Mono>());
            listLight[listLight.Count - 1].Init(Color.red, new AG_Line(_origin, hit.point, 10));
            _direction = -Vector2.Reflect(_origin - hit.point, hit.normal);
            _origin = hit.point;

            if (listLight.Count < maxLineOfLine && hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
                AddLight();
        }
    }
}
