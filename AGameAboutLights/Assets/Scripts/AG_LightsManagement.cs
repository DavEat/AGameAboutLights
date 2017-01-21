using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_LightsManagement : MonoBehaviour {

    public LayerMask layer;

    [SerializeField]
    private int maxLineOfLine = 10;
    [SerializeField]
    private float lightidth = 30;

    public GameObject _light;
    private GameObject lastHitObject;
    private List<AG_Light_Mono> listLight = new List<AG_Light_Mono>();

    [SerializeField]
    private Transform stratLightPos;
    private Vector2 _origin, _direction;
    private float angle;

    private void Start()
    {
        lastHitObject = gameObject;
        lastHitObject.layer = LayerMask.NameToLayer("Ignore Raycast");

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
            RaycastIgnore(hit);

            listLight.Add(Instantiate(_light, transform.parent).GetComponent<AG_Light_Mono>());
            listLight[listLight.Count - 1].Init(Color.red, new AG_Line(_origin, hit.point, lightidth));
            _direction = -Vector2.Reflect(_origin - hit.point, hit.normal);
            _origin = hit.point;

            EndPointLightAction(hit);
        }
    }

    private void RaycastIgnore(RaycastHit2D hit)
    {
        if (lastHitObject != null)
            lastHitObject.layer = LayerMask.NameToLayer("UI");
        lastHitObject = hit.transform.gameObject;
        lastHitObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void EndPointLightAction(RaycastHit2D hit)
    {
        if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
        {
            if (listLight.Count < maxLineOfLine)
                AddLight();
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.receiver)
            Debug.Log("YOU WON !");
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.prism)
            Debug.Log("Diffraction");
    }
}
