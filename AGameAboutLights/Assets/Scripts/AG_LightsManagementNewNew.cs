using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class AG_LightsManagementNewNew : AG_Singleton<AG_LightsManagementNewNew>
{
    #region Var
    public int screenLenght = 1920;
    public LayerMask layer;

    public AG_Receiver[] listReceiver;
    public AG_Emitter[] listEmitter;

    public float maxLightLenght = 1000;

    [SerializeField] private float _lightWidth = 30;

    public GameObject _light, victoryScreen;    
    private List<GameObject> listLight = new List<GameObject>();

    [SerializeField] private Transform lightContener = null;
    public bool lightTurnOn = true;
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
        maxLightLenght = (maxLightLenght / screenLenght) * Screen.width;

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
            inTween.Clear();

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
        ResetRaycast();

        _currentLight = 0;

        for (int i = 0; i < listEmitter.Length; i++)
            SetLights(i);
    }

    public void SetLights(int currentEmitter)
    {
        Vector2 origin = listEmitter[currentEmitter].startLightPos.position;
        float angle = listEmitter[currentEmitter].startLightPos.eulerAngles.z;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
        LightHead lightHead = new LightHead((int)listEmitter[currentEmitter].color, maxLightLenght, origin, direction, listEmitter[currentEmitter]._rect, new RaycastHit2D());

        AddLight(lightHead);
    }

    /// <summary>Calcul how place the lights in the next step</summary>
    /// <param name="LightHead">Light head</param>
    public void AddLight(LightHead lightHead)
    {
        SetRaycastIgnore(lightHead.emitter.gameObject);
        RaycastHit2D hit = Physics2D.Raycast(lightHead.origin, lightHead.direction, lightHead.lightPower, layer);
        SetRaycastEnable(lightHead.emitter.gameObject);
        if (hit.collider != null)
        {
            Debug.DrawLine(lightHead.origin, hit.point, AG_Color.colorList[lightHead.colorIndex], 1);

            if (_currentLight < _maxLineOfLine)
            {
                SpawnLights(new LightHead(lightHead.colorIndex, lightHead.lightPower, lightHead.origin, lightHead.direction, hit.transform, hit));
                _currentLight++;
            }
        }
        else if (_currentLight < _maxLineOfLine)
        {
            Debug.DrawLine(lightHead.origin, lightHead.origin + (lightHead.direction * lightHead.lightPower), AG_Color.colorList[lightHead.colorIndex], 1);
            SpawnLights(new LightHead(lightHead.colorIndex, lightHead.lightPower, lightHead.origin, lightHead.direction, lightHead.emitter, hit));
            _currentLight++;
        }
    }

    private void OnLightTouch(RaycastHit2D hit, LightHead lightHead)
    {
        AG_LightCaster lightCaster = hit.transform.GetComponent<AG_LightCaster>();
        if (lightCaster != null)
        {
            LightHead[] lightHeads = lightCaster.Cast(lightHead.colorIndex, lightHead.lightPower, lightHead.origin, hit.point, hit.normal, _currentLight);
            for (int i = 0; i < lightHeads.Length; i++)
                AddLight(lightHeads[i]);
        }
        else
        {
            AG_Receiver elem = hit.transform.GetComponent<AG_Receiver>();
            if (elem != null && elem.objectType == ObjectType.receiver)
            {
                if ((int)elem.color == lightHead.colorIndex || elem.color == AG_Color.ColorName.none)
                {
                    elem.alimented = true;

                    bool victory = true;
                    foreach (AG_Receiver receiver in listReceiver)
                        if (!receiver.alimented)
                            victory = false;

                    if (victory)
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.AppendInterval(0.3f);
                        seq.AppendCallback(() =>
                        {
                            AG_EndLevel.SaveProgression();
                            AG_EndLevel.inst.SetVictoryScreen(true);
                        });
                        seq.Play();
                    }
                }
                
            }
        }
    }

    private int _currentLight;

    /// <summary>Spawn a new Light</summary>
    /// <param name="lightHead"></param>
    private void SpawnLights(LightHead lightHead)
    {
        GameObject light = listLight[_currentLight];
        light.SetActive(true);
        RectTransform rect = light.GetComponent<RectTransform>();
        rect.position = lightHead.origin;
        Vector2 dir = lightHead.direction;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.eulerAngles = new Vector3(0, 0, angleZ);
        rect.sizeDelta = new Vector2(0, _lightWidth);
        light.GetComponent<UnityEngine.UI.Image>().color = AG_Color.colorList[lightHead.colorIndex];

        LightAnim(rect, 0.1f, lightHead);
    }

    public GameObject mirrorChock;
    private List<Sequence> inTween = new List<Sequence>();
    private void LightAnim(RectTransform light, float timeToWait, LightHead lightHead)
    {
        float distance = lightHead.hit.transform != null ? lightHead.hit.distance : lightHead.lightPower;
        distance = (distance / Screen.width) * screenLenght;
        float duration = 0.0005f * distance;

        Sequence tween = DOTween.Sequence();
        tween.AppendInterval(timeToWait)
             .Append(light.DOSizeDelta(new Vector2(distance, light.sizeDelta.y), duration))
             .OnComplete(() =>
             {
                 if (lightHead.hit.transform != null)
                 {
                     lightHead.lightPower -= lightHead.hit.distance;
                     OnLightTouch(lightHead.hit, lightHead);
                 }

                 if (mirrorChock != null)
                     Instantiate(mirrorChock, lightHead.hit.point, Quaternion.Euler(0, 0, 0));
             });
        tween.Play();
        inTween.Add(tween);
    }

}

public class LightHead
{
    public int colorIndex;
    public float lightPower;
    public Vector2 origin, direction;
    public Transform emitter;
    public RaycastHit2D hit;

    public LightHead(int colorIndex, float lightPower, Vector2 origin, Vector2 direction, Transform emitter)
    {
        this.lightPower = lightPower;
        this.origin = origin;
        this.direction = direction;
        this.colorIndex = colorIndex;
        this.emitter = emitter;
    }
    public LightHead(int colorIndex, float lightPower, Vector2 origin, Vector2 direction, Transform emitter, RaycastHit2D hit)
    {
        this.lightPower = lightPower;
        this.origin = origin;
        this.direction = direction;
        this.colorIndex = colorIndex;
        this.emitter = emitter;
        this.hit = hit;
    }
}