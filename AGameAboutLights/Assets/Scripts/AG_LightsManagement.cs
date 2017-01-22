﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using DG.Tweening;

public class AG_LightsManagement : MonoBehaviour
{

    #region Var
    public LayerMask layer;

    public AG_Receiver[] listReceiver;

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

    private PrismaManagement prismaM;
    private List<LightConstructor> listLightConstructor = new List<LightConstructor>();
    #endregion

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

            lastHitObject.layer = LayerMask.NameToLayer("RaycastableByLight");
            lastHitObject = null;
        }
    }

    private void Start()
    {
        prismaM = new PrismaManagement();
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
        AddLight(0);
    }

    public void AddLight(int colorIndex)
    {
        RaycastHit2D hit = Physics2D.Raycast(_origin, _direction, Mathf.Infinity, layer);
        if (hit.collider != null)
        {
            RaycastIgnore(hit);

            //listLight[currentLight].SetActive(true);
            //listLight[currentLight].GetComponent<AG_Light_Mono>().Init(colorIndex, new AG_Line(_origin, hit.point, lightWidth));

            listLightConstructor.Add(new LightConstructor(colorIndex, _origin, hit.point));

            currentLight++;
            EndPointLightAction(hit, colorIndex);
        }
    }

    private void RaycastIgnore(RaycastHit2D hit)
    {
        if (lastHitObject != null)
            lastHitObject.layer = LayerMask.NameToLayer("RaycastableByLight");
        if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
        {
            lastHitObject = hit.transform.gameObject;
            lastHitObject.layer = LayerMask.NameToLayer("IgnoreLazerRaycast");
        }
    }

    private void RaycastIgnore(Transform _transform)
    {
        if (lastHitObject != null)
            lastHitObject.layer = LayerMask.NameToLayer("RaycastableByLight");
        lastHitObject = _transform.gameObject;
        lastHitObject.layer = LayerMask.NameToLayer("IgnoreLazerRaycast");
    }

    private void EndPointLightAction(RaycastHit2D hit, int colorIndex)
    {
        if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.wall)
        {
            if (currentLight < maxLineOfLine)
                SetWaitingPrismaColor();
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.mirror)
        {
            if (currentLight < maxLineOfLine)
            {
                RaycastIgnore(hit.transform);
                _direction = -Vector2.Reflect(_origin - hit.point, hit.normal);
                _origin = hit.point;
                AddLight(colorIndex);
            }
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.receiver)
        {
            AG_Receiver receiver = hit.transform.GetComponent<AG_Receiver>();
            if (colorIndex == receiver.colorIndex || receiver.colorIndex == -1)
                receiver.alimented = true;
            if (currentLight < maxLineOfLine)
                SetWaitingPrismaColor();
            CheckVictory();
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.prisma)
        {
            if (currentLight < maxLineOfLine)
            {
                //Debug.Log("Prisma");
                InitNewPrisma(hit.transform, colorIndex);
            }
        }
        else if (hit.transform.GetComponent<AG_ElementType>().objectType == ObjectType.filter)
        {
            if (currentLight < maxLineOfLine)
            {
                Debug.Log("color index : " + colorIndex + " -> " + hit.transform.GetComponent<AG_Filter>().colorIndex);
                if (colorIndex == hit.transform.GetComponent<AG_Filter>().colorIndex)
                {
                    RaycastIgnore(hit.transform);
                    _origin = hit.point;
                    AddLight(colorIndex);
                }
                else SetWaitingPrismaColor();
            }
        }
    }

    private void InitNewPrisma(Transform _transform, int _colorIndex)
    {
        AG_PrismaFace prisma = _transform.GetComponent<AG_PrismaFace>();
        RaycastIgnore(prisma.face1);
        _direction = prisma.face1.position - prisma.face1.parent.position;
        _origin = prisma.face1.position;

        prismaM.listPrismaFace.Add(prisma.face2);
        int colorIndex = prismaM.PrismaColorManagement(_colorIndex);

        AddLight(colorIndex);
    }

    private void SetWaitingPrismaColor()
    {
        if (prismaM.listPrismaFace.Count > 0)
        {
            RaycastIgnore(prismaM.listPrismaFace[0]);
            object[] objs = prismaM.PlayPrismaWaitingList();
            _origin = (Vector2)objs[0];
            _direction = (Vector2)objs[1];
            AddLight((int)objs[2]);
        }
        else
        {
            currentLight = 0;
            SpawnLights();
        }
    }

    private bool CheckVictory()
    {
        bool victory = true;
        foreach (AG_Receiver receiver in listReceiver)
            if (!receiver.alimented)
                victory = false;

        if (victory)
        {
            victoryScreen.SetActive(true);
            Debug.Log("YOU WON");
        }
        return victory;
    }

    private void SpawnLights()
    {
        //currentLight = 0;
        foreach (LightConstructor listConstrucor in listLightConstructor)
        //if (currentLight < maxLineOfLine)
        {
            listLight[currentLight].SetActive(true);
            listLight[currentLight].GetComponent<AG_Light_Mono>().Init(listLightConstructor[currentLight].colorIndex, new AG_Line(listLightConstructor[currentLight].origin, listLightConstructor[currentLight].direction, lightWidth));
            //LightAnim();
            currentLight++;
        }
        //else
        {
            currentLight = 0;
            listLightConstructor = new List<LightConstructor>();
        }
    }

    /*private void LightAnim()
    {
        AG_Line line = listLight[currentLight].GetComponent<AG_Light_Mono>().ag_light.GetLightValue();
        float duration = 6f * line.distance;
        RectTransform light = listLight[currentLight].GetComponent<RectTransform>();

        /*Sequence inTween = DOTween.Sequence();
        inTween.Append(light.DOSizeDelta(new Vector2(line.distance, line.width), duration, false))
               .AppendInterval(0.1f)
               .OnComplete(() => { SpawnLights(); });
        inTween.Play();*/
    //}

    /*IEnumerator lightAnim()
    {

    }*/

}

public class PrismaManagement
{
    #region Var
    private List<Transform> _listPrismaFace;
    private List<int> _listPrismaColorIndex;
    #endregion
    #region Struct
    public List<Transform> listPrismaFace
    {
        get { return _listPrismaFace; }
        set { _listPrismaFace = value; }
    }
    public List<int> listPrismaColorIndex
    {
        get { return _listPrismaColorIndex; }
        set { _listPrismaColorIndex = value; }
    }
    #endregion

    public PrismaManagement()
    {
        _listPrismaFace = new List<Transform>();
        _listPrismaColorIndex = new List<int>();
    }

    public int PrismaColorManagement(int colorIndex)
    {
        int[] arrayToReturn;
        if (colorIndex == 0)
            arrayToReturn = new int[] { 1, 2 };
        else if (colorIndex == 1)
            arrayToReturn = new int[] { 0, 2 };
        else if (colorIndex == 2)
            arrayToReturn = new int[] { 0, 1 };
        else arrayToReturn = null;

        listPrismaColorIndex.Add(arrayToReturn[1]);
        return arrayToReturn[0];
    }

    public object[] PlayPrismaWaitingList()
    {
        if (listPrismaFace.Count > 0)
        {
            Vector2 _direction = listPrismaFace[0].position - listPrismaFace[0].parent.position;
            Vector2 _origin = listPrismaFace[0].position;
            listPrismaFace.RemoveAt(0);

            int colorIndex = listPrismaColorIndex[0];
            listPrismaColorIndex.RemoveAt(0);

            return new object[] { _origin, _direction, colorIndex };
        }
        else return null;
    }
}

public class LightConstructor
{
    private Vector2 _origin, _direction;
    private int _colorIndex;

    public Vector2 origin
    {
        get { return _origin; }
        set { _origin = value; }
    }
    public Vector2 direction
    {
        get { return _direction; }
        set { _direction = value; }
    }
    public int colorIndex
    {
        get { return _colorIndex; }
        set { _colorIndex = value; }
    }

    public LightConstructor(int _colorIndex, Vector2 _origin, Vector2 _direction)
    {
        this._origin = _origin;
        this._direction = _direction;
        this._colorIndex = _colorIndex;
    }
}