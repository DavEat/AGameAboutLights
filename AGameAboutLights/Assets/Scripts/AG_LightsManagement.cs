using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_LightsManagement : MonoBehaviour {

    public LayerMask layer;

    [SerializeField]
    private int maxLineOfLine = 20;
    private int currentLight = 0;
    [SerializeField]
    private float lightWidth = 30;

    public GameObject _light, victoryScreen;
    private GameObject lastHitObject;
    private List<GameObject> listLight = new List<GameObject>();

    [SerializeField]
    private Transform stratLightPos, lightContener;
    private Vector2 _origin, _direction;
    private float angle;
    public bool lightTurnOn = true;
    [SerializeField]
    private AG_DragDrop dragDrop;

    public void ToggleLight()
    {
        lightTurnOn = !lightTurnOn;
        if (lightTurnOn)
        {
            SetLights();
            dragDrop.lazerTurnOn = true;
        }
        else
        {
            dragDrop.lazerTurnOn = false;
            Debug.Log("destroy");
            foreach (GameObject g in listLight)
                if (g.activeSelf)
                    g.SetActive(false);

            lastHitObject.layer = LayerMask.NameToLayer("UI");
            lastHitObject = null;
        }      
    }

    private void Start()
    {
        CreateLights();
    }

    private void CreateLights()
    {
        for (int i = 0; i < maxLineOfLine; i++)
        {
            listLight.Add(Instantiate(_light, lightContener));
            listLight[listLight.Count - 1].SetActive(false);
        }
    }

    public void SetLights()
    {
        currentLight = 0;
        lastHitObject = gameObject;
        lastHitObject.layer = LayerMask.NameToLayer("IgnoreLazerRaycast");

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

            listLight[currentLight].SetActive(true);
            listLight[currentLight].GetComponent<AG_Light_Mono>().Init(Color.red, new AG_Line(_origin, hit.point, lightWidth));
            _direction = -Vector2.Reflect(_origin - hit.point, hit.normal);
            _origin = hit.point;

            currentLight++;
            EndPointLightAction(hit);            
        }
    }

    private void RaycastIgnore(RaycastHit2D hit)
    {
        if (lastHitObject != null)
            lastHitObject.layer = LayerMask.NameToLayer("UI");
        if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
        {
            lastHitObject = hit.transform.gameObject;
            lastHitObject.layer = LayerMask.NameToLayer("IgnoreLazerRaycast");
        }
    }

    private void EndPointLightAction(RaycastHit2D hit)
    {
        if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
        {
            if (currentLight < maxLineOfLine)
                AddLight();
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.receiver)
        {
            victoryScreen.SetActive(true);
            Debug.Log("YOU WON !");
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.prisma)
            Debug.Log("Diffraction");
    }
}
