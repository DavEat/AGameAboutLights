using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class AG_LightsManagementNew : AG_Singleton<AG_LightsManagementNew>
{
    #region Var
    public LayerMask layer;

    public AG_Receiver[] listReceiver;
    public AG_Emitter[] listEmitter;

    private List<List<LightConstructor>> _lightToDrawByCouche = new List<List<LightConstructor>>();

    [SerializeField] private float _lightWidth = 30;

    public GameObject _light, victoryScreen;
    [SerializeField] private GameObject lastHitObject;
    private List<GameObject> listLight = new List<GameObject>();

    [SerializeField] private Transform lightContener = null;
    public bool lightTurnOn = true;

    private List<LightConstructor> listLightConstructor = new List<LightConstructor>();

    #endregion

    #region listLightHeadOne
    public bool firstListLightHead = true;
    private List<LightHead> _listLightHeadOne = new List<LightHead>(), _listLightHeadTwo = new List<LightHead>();  // TODO : have to list an swith each step between the two list

    /*public List<LightHead> listLightHead
    {
        get { return _firstListLightHead ? _listLightHeadOne : _listLightHeadTwo; }
        set { if (_firstListLightHead) _listLightHeadOne = value;
            else _listLightHeadTwo = value; }
    }*/
    public List<LightHead> GetLightHead(bool _firstListLightHead)
    {
        return _firstListLightHead ? _listLightHeadOne : _listLightHeadTwo;
    }
    public void AddLightHead(bool _firstListLightHead, LightHead value)
    {
        if (_firstListLightHead)
            _listLightHeadOne.Add(value);
        else _listLightHeadTwo.Add(value);
    }
    public void ResetLightHeadAtIndex(bool _firstListLightHead)
    {
        if (_firstListLightHead)
        {
            //Debug.Log("_listLightHeadOne.Count : " + _listLightHeadOne.Count);
            int x = _listLightHeadOne.Count;
            for (int i = x - 1; i >= 0; i--)
                _listLightHeadOne.RemoveAt(i);
            //Debug.Log("_listLightHeadOne.Count 2 : " + _listLightHeadOne.Count);
        }
        else
        {
            //Debug.Log("_listLightHeadTwo.Count : " + _listLightHeadOne.Count);
            int x = _listLightHeadTwo.Count;
            for (int i = x - 1; i >= 0; i--)
                _listLightHeadTwo.RemoveAt(i);
            //Debug.Log("_listLightHeadTwo.Count 2 : " + _listLightHeadTwo.Count);
        }
    }
    public void _SetLightHeadAtIndex(bool _firstListLightHead, int _index, LightHead value)
    {
        if (_firstListLightHead)
            _listLightHeadOne[_index] = value;
        else _listLightHeadTwo[_index] = value;
    }
    public void RemoveLightHeadAtIndex(bool _firstListLightHead, int _index)
    {
        if (_firstListLightHead)
            _listLightHeadOne.RemoveAt(_index);
        else _listLightHeadTwo.RemoveAt(_index);
    }
    #endregion

    private void Start()
    {
        for (int i = 0; i < listEmitter.Length; i++)
            SetLights(i);

        for (int i = 0; i < GetLightHead(firstListLightHead).Count; i++)
            AddLight(i, GetLightHead(firstListLightHead)[i].colorIndex, GetLightHead(firstListLightHead)[i].origin, GetLightHead(firstListLightHead)[i].direction);
        SetListLightHeadAtIndex(0);
    }

    private void Update()
    {
        //for (int i = 0; i < listEmitter.Length; i++)
        //    SetLights(i);
    }

    public void SetLights(int currentEmitter)
    {
        lastHitObject = gameObject;
        //lastHitObject.layer = LayerMask.NameToLayer("IgnoreLazerRaycast");
        if (lastHitObject != null)
            lastHitObject.layer = LayerMask.NameToLayer("RaycastableByLight");//////////////////////////////////////////////
        ///////////////////////////////////
        //////////////////////////////////

        Vector2 origin = listEmitter[currentEmitter].startLightPos.position;
        float angle = listEmitter[currentEmitter].startLightPos.eulerAngles.z;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

        //<new>
        AddLightHead(firstListLightHead, new LightHead((int)listEmitter[currentEmitter].color, origin, direction));
        //</new>
        //AddLight((int)listEmitter[currentEmitter].color, origin, direction);
    }

    /// <summary>Calcul how place the lights in the next step</summary>
    /// <param name="_currentLightHeadIndex"></param>
    /// <param name="_colorIndex"></param>
    /// <param name="_origin"></param>
    /// <param name="_direction"></param>
    public void AddLight(int _currentLightHeadIndex, int _colorIndex, Vector2 _origin, Vector2 _direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(_origin, _direction, 3000f, layer);
        if (hit.collider != null)
        {
            Debug.DrawLine(_origin, hit.point, AG_Color.colorList[_colorIndex], 10);

            //listLight[currentLight].SetActive(true);
            //listLight[currentLight].GetComponent<AG_Light_Mono>().Init(colorIndex, new AG_Line(_origin, hit.point, _lightWidth));

            listLightConstructor.Add(new LightConstructor(_colorIndex, _origin, hit.point));
            //<new>
            AG_LightCaster lightCaster = hit.transform.GetComponent<AG_LightCaster>();
            if (lightCaster != null)
                lightCaster.Cast(_currentLightHeadIndex, _colorIndex, _origin, hit.point, hit.normal);
            //</new>
            //EndPointLightAction(hit, colorIndex);
        }
    }

    public void RaycastIgnore(Transform _transform)
    {        
        if (lastHitObject != null)
            lastHitObject.layer = LayerMask.NameToLayer("RaycastableByLight");
        lastHitObject = _transform.gameObject;
        lastHitObject.layer = LayerMask.NameToLayer("IgnoreLazerRaycast");
    }

    /// <summary>Check if it's the last object of the list and in that case run the next step</summary>
    /// <param name="_currentLightHeadIndex">current index</param>
    public void SetListLightHeadAtIndex(int _currentLightHeadIndex)
    {
        //Debug.Log("set list light head by index : " + _currentLightHeadIndex + " - " + (GetLightHead(firstListLightHead).Count - 1));
        //if (_currentLightHeadIndex >= GetLightHead(firstListLightHead).Count - 1)
        {
            firstListLightHead = !firstListLightHead;
            ResetLightHeadAtIndex(firstListLightHead);
            Debug.Log("firstListLightHead" + firstListLightHead);
            for (int i = 0; i < GetLightHead(!firstListLightHead).Count; i++)
                AddLight(i, GetLightHead(!firstListLightHead)[i].colorIndex, GetLightHead(!firstListLightHead)[i].origin, GetLightHead(!firstListLightHead)[i].direction);
                //call the add light for each head in the list
            if (GetLightHead(!firstListLightHead).Count > 0)
                SetListLightHeadAtIndex(0);
        }
    }
}

public struct LightConstructor
{
    private int _colorIndex;
    private Vector2 _origin, _end;

    public int colorIndex
    {
        get { return _colorIndex; }
        set { _colorIndex = value; }
    }
    public Vector2 origin
    {
        get { return _origin; }
        set { _origin = value; }
    }
    public Vector2 end
    {
        get { return _end; }
        set { _end = value; }
    }

    public LightConstructor(int _colorIndex, Vector2 _origin, Vector2 _end)
    {
        this._origin = _origin;
        this._end = _end;
        this._colorIndex = _colorIndex;
    }
}

public struct LightHead
{
    private int _colorIndex;
    private Vector2 _origin, _direction;

    public int colorIndex
    {
        get { return _colorIndex; }
        set { _colorIndex = value; }
    }
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

    public LightHead(int _colorIndex, Vector2 _origin, Vector2 _direction)
    {
        this._origin = _origin;
        this._direction = _direction;
        this._colorIndex = _colorIndex;
    }
}