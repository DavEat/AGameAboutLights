﻿using System.Collections;
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

    [SerializeField] private float _lightWidth = 30;

    public GameObject _light, victoryScreen;    
    private List<GameObject> listLight = new List<GameObject>();

    [SerializeField] private Transform lightContener = null;
    public bool lightTurnOn = true;

    private List<LightConstructor> listLightConstructor = new List<LightConstructor>();
    #endregion

    #region listLightHeadOne
    public bool firstListLightHead = true;
    private List<LightHead> _listLightHeadOne = new List<LightHead>(), _listLightHeadTwo = new List<LightHead>();  // TODO : have to list an swith each step between the two list

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
    public void ResetLightHead(bool _firstListLightHead)
    {
        if (_firstListLightHead)
        {
            int x = _listLightHeadOne.Count;
            for (int i = x - 1; i >= 0; i--)
                _listLightHeadOne.RemoveAt(i);
            //_listLightHeadOne = new List<LightHead>();
        }
        else
        {
            int x = _listLightHeadTwo.Count;
            for (int i = x - 1; i >= 0; i--)
                _listLightHeadTwo.RemoveAt(i);
            //_listLightHeadTwo = new List<LightHead>();
        }
    }
    public void SetLightHeadAtIndex(bool _firstListLightHead, int _index, LightHead value)
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

    #region Layer Ignore
    private List<GameObject> _lastHitObjects = new List<GameObject>();
    private const string raycastable = "RaycastableByLight", ignore = "IgnoreLazerRaycast";

    public void ResetRaycast()
    {
        int x = _lastHitObjects.Count;
        if (x > 0)
        {            
            for (int i = x - 1; i >= 0; i--)
            {
                _lastHitObjects[i].layer = LayerMask.NameToLayer(raycastable);
                _lastHitObjects.RemoveAt(i);
            }
        }
    }
    public void AddRaycastIgnore(GameObject _gameObject)
    {
        _lastHitObjects.Add(_gameObject);
        _gameObject.layer = LayerMask.NameToLayer(ignore);
    }

    public void SetRaycastIgnore(GameObject _gameObject)
    {
        _gameObject.layer = LayerMask.NameToLayer(ignore);
    }
    public void SetRaycastEnable(GameObject _gameObject)
    {
        _gameObject.layer = LayerMask.NameToLayer(raycastable);
    }
    #endregion

    private void Start()
    {
        CreateLights();
    }
    private int _maxLineOfLine = 150;
    private void CreateLights()
    {
        for (int i = 0; i < _maxLineOfLine; i++)
        {
            listLight.Add(Instantiate(_light, lightContener));
            listLight[listLight.Count - 1].SetActive(false);
        }
    }

    public void ToggleLight()
    {
        lightTurnOn = !lightTurnOn;
        if (lightTurnOn)
        {
            Init();            
        }
        else
        {
            foreach (Tween tween in inTween)
                tween.Kill();

            foreach (GameObject g in listLight)
                if (g.activeSelf)
                    g.SetActive(false);

            foreach (AG_Receiver receiver in listReceiver)
                if (receiver.alimented)
                    receiver.alimented = false;

            /*foreach (LightConstructor light in listLightConstructor)
                listLightConstructor.Remove(light);*/
            listLightConstructor = new List<LightConstructor>();
        }
    }

    private void Init()
    {
        ResetLightHead(firstListLightHead);
        ResetRaycast();

        _currentLight = 0;

        for (int i = 0; i < listEmitter.Length; i++)
            SetLights(i);

        foreach (LightHead lightHead in GetLightHead(firstListLightHead))
            AddLight(lightHead);
        SetListLightHeadAtIndex();
    }

    public void SetLights(int currentEmitter)
    {
        Vector2 origin = listEmitter[currentEmitter].startLightPos.position;
        float angle = listEmitter[currentEmitter].startLightPos.eulerAngles.z;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

        AddLightHead(firstListLightHead, new LightHead((int)listEmitter[currentEmitter].color, origin, direction, listEmitter[currentEmitter].transform, null, -1));
    }

    /// <summary>Calcul how place the lights in the next step</summary>
    /// <param name="LightHead">Light head</param>
    public void AddLight(LightHead _lightHead)
    {
        SetRaycastIgnore(_lightHead.emitter.gameObject);
        RaycastHit2D hit = Physics2D.Raycast(_lightHead.origin, _lightHead.direction, 3000f, layer);
        SetRaycastEnable(_lightHead.emitter.gameObject);
        if (hit.collider != null)
        {
            Debug.DrawLine(_lightHead.origin, hit.point, AG_Color.colorList[_lightHead.colorIndex], 1);

            if (_currentLight < _maxLineOfLine)
            {
                listLightConstructor.Add(new LightConstructor(_lightHead.colorIndex, _lightHead.origin, hit.point, null));

                //SpawnLights(new LightConstructor(_lightHead.colorIndex, _lightHead.origin, hit.point, _lightHead.nextLightsIndex));
                _currentLight++;
            }

            AG_LightCaster lightCaster = hit.transform.GetComponent<AG_LightCaster>();
            if (lightCaster != null)
                lightCaster.Cast(_lightHead.colorIndex, _lightHead.origin, hit.point, hit.normal, _currentLight);
            else
            {
                AG_Receiver elem = hit.transform.GetComponent<AG_Receiver>();
                if (elem != null && elem.objectType == ObjectType.receiver)
                {
                    if ((int)elem.color == _lightHead.colorIndex || elem.color == AG_Color.ColorName.none)
                        elem.alimented = true;
                }
            }
            Debug.Log("current light : " + _currentLight);
        }
    }

    /// <summary>Main Recursive Function - Check if it's the last object of the list and in that case run the next step</summary>
    public void SetListLightHeadAtIndex() //----- Main Recursive Function -----
    {
        firstListLightHead = !firstListLightHead;
        ResetLightHead(!firstListLightHead);
            
        foreach(LightHead lightHead in GetLightHead(firstListLightHead))
            AddLight(lightHead);
            //call the add light for each head in the list
        ResetRaycast();
        if (GetLightHead(firstListLightHead).Count > 0)
            SetListLightHeadAtIndex();

        Debug.Log("END RECURSIVITY");

        for (int i = 0; i < listEmitter.Length; i++)
            SpawnLights(listLightConstructor[i]);

        bool victory = true;
        foreach (AG_Receiver receiver in listReceiver)
            if (!receiver.alimented)
                victory = false;

        if (victory)
        {
            AG_EndLevel.SaveProgression();
            victoryScreen.SetActive(true);
        }
    }

    private int _currentLight;
    private void SpawnLights(LightConstructor _lightConstructor)
    {
        GameObject light = listLight[_currentLight];
        light.SetActive(true);
        RectTransform rect = light.GetComponent<RectTransform>();
        rect.position = _lightConstructor.origin;
        Vector2 dir = _lightConstructor.origin - _lightConstructor.end;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.eulerAngles = new Vector3(0, 0, 180 + angleZ);
        rect.sizeDelta = new Vector2(0, _lightWidth);
        light.GetComponent<UnityEngine.UI.Image>().color = AG_Color.colorList[_lightConstructor.colorIndex];

        LightAnim(rect, 0, Vector2.Distance(_lightConstructor.origin, _lightConstructor.end), _lightConstructor.animDuration, _lightConstructor.end, _lightConstructor);
    }

    private List<LightConstructor> _listLightConstructor = new List<LightConstructor>();

    public GameObject mirrorChock;
    private List<Sequence> inTween = new List<Sequence>();
    private void LightAnim(RectTransform _rect, float _timeToWait, float _distance, float _duration, Vector2 _endPos, LightConstructor _lightConstructor)
    {
        float duration = 0.0005f * _distance;

        Sequence tween = DOTween.Sequence();
        tween.AppendInterval(_timeToWait)
             .Append(_rect.DOSizeDelta(new Vector2(_distance, _rect.sizeDelta.y), _duration))
             .OnComplete(() =>
             {
                 Debug.Log(" 0 spanw light");
                 if (_lightConstructor.nextLightIndex != null)
                 {
                     foreach (int i in _lightConstructor.nextLightIndex)
                     {
                         Debug.Log(" 1 spanw light : " + i + " listLightConstructor.Count : " + listLightConstructor.Count);
                         //if (i < listLightConstructor.Count)
                             SpawnLights(listLightConstructor[i]);
                     }
                 }else Debug.Log(" 0 spanw light -1");

                 if (mirrorChock != null)
                     Instantiate(mirrorChock, _endPos, Quaternion.Euler(0, 0, 0));
                 //inTween.Remove(tween);
             });
        tween.Play();
        inTween.Add(tween);
    }

}

public struct LightConstructor
{
    #region Var
    private int[] _nextLightIndex;
    private float _animDuration;

    private int _colorIndex;
    private Vector2 _origin, _end;
    #endregion
    #region Struct
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
    public int[] nextLightIndex
    {
        get { return _nextLightIndex; }
        set { _nextLightIndex = value; }
    }
    public float animDuration
    {
        get { return _animDuration; }
        set { _animDuration = value; }
    }
    #endregion
    #region Consdtructor
    public LightConstructor(int _colorIndex, Vector2 _origin, Vector2 _end, int[] _nextLightIndex)
    {
        this._origin = _origin;
        this._end = _end;
        this._colorIndex = _colorIndex;
        this._nextLightIndex = _nextLightIndex;
        _animDuration = 0.0005f * Vector2.Distance(_origin, _end);
    }
    public LightConstructor(int _colorIndex, Vector2 _origin, Vector2 _end, int[] _nextLightIndex, float _animDuration)
    {
        this._origin = _origin;
        this._end = _end;
        this._colorIndex = _colorIndex;
        this._nextLightIndex = _nextLightIndex;
        this._animDuration = _animDuration;
    }
    #endregion
}

public struct LightHead
{
    private int _colorIndex, _previousLightIndex;
    private int[] _nextLightsIndex;
    private Vector2 _origin, _direction;
    private Transform _emitter;

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
    public Transform emitter
    {
        get { return _emitter; }
        set { _emitter = value; }
    }
    public int[] nextLightsIndex
    {
        get { return _nextLightsIndex; }
        set { _nextLightsIndex = value; }
    }
    public int previousLightIndex
    {
        get { return _previousLightIndex; }
        set { _previousLightIndex = value; }
    }

    public LightHead(int _colorIndex, Vector2 _origin, Vector2 _direction, Transform _emitter, int[] _nextLightsIndex, int _previousLightIndex)
    {
        this._origin = _origin;
        this._direction = _direction;
        this._colorIndex = _colorIndex;
        this._emitter = _emitter;
        this._nextLightsIndex = _nextLightsIndex;
        this._previousLightIndex = _previousLightIndex;
    }
}