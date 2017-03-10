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
        }
    }

    private void Init()
    {
        ResetLightHead(firstListLightHead);
        ResetRaycast();

        _currentLight = 0;

        for (int i = 0; i < listEmitter.Length; i++)
            SetLights(i);

        for (int i = 0; i < GetLightHead(firstListLightHead).Count; i++)
            AddLight(i, GetLightHead(firstListLightHead)[i], 0);
        SetListLightHeadAtIndex();
    }

    public void SetLights(int currentEmitter)
    {
        AddRaycastIgnore(listEmitter[currentEmitter].gameObject);

        Vector2 origin = listEmitter[currentEmitter].startLightPos.position;
        float angle = listEmitter[currentEmitter].startLightPos.eulerAngles.z;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

        //<new>
        AddLightHead(firstListLightHead, new LightHead((int)listEmitter[currentEmitter].color, origin, direction, listEmitter[currentEmitter].transform, null));
        //</new>
        //AddLight((int)listEmitter[currentEmitter].color, origin, direction);
    }

    /// <summary>Calcul how place the lights in the next step</summary>
    /// <param name="_currentLightHeadIndex"></param>
    /// <param name="_colorIndex"></param>
    /// <param name="_origin"></param>
    /// <param name="_direction"></param>
    public void AddLight(int _currentLightHeadIndex, LightHead _lightHead, float _previousAnimTime)
    {
        SetRaycastIgnore(_lightHead.emitter.gameObject);
        RaycastHit2D hit = Physics2D.Raycast(_lightHead.origin, _lightHead.direction, 3000f, layer);
        SetRaycastEnable(_lightHead.emitter.gameObject);
        if (hit.collider != null)
        {
            Debug.DrawLine(_lightHead.origin, hit.point, AG_Color.colorList[_lightHead.colorIndex], 0.5f);

            float currentAnimTime = _previousAnimTime;

            if (_currentLight < _maxLineOfLine)
            {
                float animDuration = 0.0005f * Vector2.Distance(_lightHead.origin, hit.point);
                currentAnimTime += animDuration;
                SpawnLights(new LightConstructor(_lightHead.colorIndex, _lightHead.origin, hit.point, _lightHead.nextLightsIndex, animDuration));
                _currentLight++;
            }
            //listLight[currentLight].SetActive(true);
            //listLight[currentLight].GetComponent<AG_Light_Mono>().Init(colorIndex, new AG_Line(_origin, hit.point, _lightWidth));

            //listLightConstructor.Add(new LightConstructor(_colorIndex, _origin, hit.point));
            //<new>
            AG_LightCaster lightCaster = hit.transform.GetComponent<AG_LightCaster>();
            if (lightCaster != null)
                lightCaster.Cast(_currentLightHeadIndex, _lightHead.colorIndex, _lightHead.origin, hit.point, hit.normal, currentAnimTime, _currentLight);
            else
            {
                AG_ElementType elem = hit.transform.GetComponent<AG_ElementType>();
                if (elem != null && elem.objectType == ObjectType.receiver)
                {
                    ((AG_Receiver)elem).alimented = true;
                }
            }
            //</new>
            //EndPointLightAction(hit, colorIndex);
        }
    }

    /// <summary>Check if it's the last object of the list and in that case run the next step</summary>
    /// <param name="_currentLightHeadIndex">current index</param>
    public void SetListLightHeadAtIndex()
    {
        //Debug.Log("set list light head by index : " + _currentLightHeadIndex + " - " + (GetLightHead(firstListLightHead).Count - 1));
        //if (_currentLightHeadIndex >= GetLightHead(firstListLightHead).Count - 1)
        {
            firstListLightHead = !firstListLightHead;
            ResetLightHead(!firstListLightHead);
            
            for (int i = 0; i < GetLightHead(firstListLightHead).Count; i++)
                AddLight(i, GetLightHead(firstListLightHead)[i], 0);
                //call the add light for each head in the list
            ResetRaycast();
            if (GetLightHead(firstListLightHead).Count > 0)
                SetListLightHeadAtIndex();

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
    }

    private int _currentLight;
    private void SpawnLights(LightConstructor _lightConstructor)
    {
        listLight[_currentLight].SetActive(true);
        RectTransform rect = listLight[_currentLight].GetComponent<RectTransform>();
        rect.position = _lightConstructor.origin;
        Vector2 dir = _lightConstructor.origin - _lightConstructor.end;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.eulerAngles = new Vector3(0, 0, 180 + angleZ);
        rect.sizeDelta = new Vector2(0, _lightWidth);
        listLight[_currentLight].GetComponent<UnityEngine.UI.Image>().color = AG_Color.colorList[_lightConstructor.colorIndex];

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
                   if (_lightConstructor.nextLightIndex != null)
                       foreach (int i in _lightConstructor.nextLightIndex)
                       {
                           Debug.Log("next light id : " + i);
                           if (i < listLightConstructor.Count)
                            SpawnLights(listLightConstructor[i]);
                       }

                   if (mirrorChock != null)
                       Instantiate(mirrorChock, _endPos, Quaternion.Euler(0, 0, 0));
                   inTween.Remove(tween);
               });
        tween.Play();
        inTween.Add(tween);
    }

}

public struct LightConstructor
{
    //public int previousIndex, nextindex; //light constructor index 
    private int[] _nextLightIndex;
    private float _animDuration;

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

    /*public LightConstructor(int _colorIndex, Vector2 _origin, Vector2 _end)
    {
        timeToWait = 0;
        this._origin = _origin;
        this._end = _end;
        this._colorIndex = _colorIndex;
    }*/

    public LightConstructor(int _colorIndex, Vector2 _origin, Vector2 _end, int[] _nextLightIndex, float _animDuration)
    {
        this._origin = _origin;
        this._end = _end;
        this._colorIndex = _colorIndex;
        this._nextLightIndex = _nextLightIndex;
        this._animDuration = _animDuration;
    }
}

public struct LightHead
{
    private int _colorIndex;
    private int[] _nextLightsIndex;
    //private float _previousAnimDuration;
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
    /*public float previousAnimDuration
    {
        get { return _previousAnimDuration; }
        set { _previousAnimDuration = value; }
    }*/
    public int[] nextLightsIndex
    {
        get { return _nextLightsIndex; }
        set { _nextLightsIndex = value; }
    }

    public LightHead(int _colorIndex, Vector2 _origin, Vector2 _direction, Transform _emitter,/* float _previousAnimDuration,*/ int[] _nextLightsIndex)
    {
        this._origin = _origin;
        this._direction = _direction;
        this._colorIndex = _colorIndex;
        this._emitter = _emitter;
        //this._previousAnimDuration = _previousAnimDuration;
        this._nextLightsIndex = _nextLightsIndex;
    }
}