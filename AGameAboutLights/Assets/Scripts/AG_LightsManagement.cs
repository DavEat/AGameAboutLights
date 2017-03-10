using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class AG_LightsManagement : MonoBehaviour
{

    #region Var
    //public AG_Color.ColorName startColor;

    public LayerMask layer;

    public AG_Receiver[] listReceiver;
    public AG_Emitter[] listEmitter;
    private int currentEmitterIndex = 0, currenttotalLight;

    [SerializeField]
    private int maxLineOfLine = 20;
    private int currentLight = 0;
    [SerializeField]
    private float lightWidth = 30;

    public GameObject _light, victoryScreen;
    private GameObject lastHitObject;
    private List<GameObject> listLight = new List<GameObject>();

    [SerializeField]
    private Transform lightContener = null;
    private Vector2 _origin, _direction;
    private float angle;
    public bool lightTurnOn = true;
    [SerializeField] private AG_DragDrop dragDrop;

    private PrismaManagement prismaM;
    private List<LightConstructor>[] listLightConstructor;
    #endregion

    public void ToggleLight()
    {
        lightTurnOn = !lightTurnOn;
        if (lightTurnOn)
        {
            dragDrop.lazerTurnOn = true;
            currenttotalLight = 0;

            listLightConstructor = new List<LightConstructor>[listEmitter.Length];
            for (int i = 0; i < listEmitter.Length; i++)
                listLightConstructor[i] = new List<LightConstructor>();

            for (currentEmitterIndex = 0; currentEmitterIndex < listEmitter.Length; currentEmitterIndex++)
                SetLights();            
        }
        else
        {
            dragDrop.lazerTurnOn = false;            

            inTween.Kill();
            //Debug.Log("destroy");
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

        _origin = listEmitter[currentEmitterIndex].startLightPos.position;
        angle = listEmitter[currentEmitterIndex].startLightPos.eulerAngles.z;
        _direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
        AddLight((int)listEmitter[currentEmitterIndex].color);
    }

    public void AddLight(int colorIndex)
    {
        RaycastHit2D hit = Physics2D.Raycast(_origin, _direction, Mathf.Infinity, layer);
        if (hit.collider != null)
        {
            RaycastIgnore(hit);

            //listLight[currentLight].SetActive(true);
            //listLight[currentLight].GetComponent<AG_Light_Mono>().Init(colorIndex, new AG_Line(_origin, hit.point, lightWidth));

            listLightConstructor[currentEmitterIndex].Add(new LightConstructor(colorIndex, _origin, hit.point, 0, 0));

            currentLight++;
            currenttotalLight++;
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
        ObjectType objectType = hit.transform.GetComponent<AG_ElementType>().objectType;

        if (objectType == ObjectType.wall || objectType == ObjectType.emitter)
        {
            Debug.Log("Wall - emitter");
            if (currenttotalLight < maxLineOfLine)
                SetWaitingPrismaColor();
        }
        else if (objectType == ObjectType.mirror)
        {
            if (currenttotalLight < maxLineOfLine)
            {
                RaycastIgnore(hit.transform);
                _direction = -Vector2.Reflect(_origin - hit.point, hit.normal);
                _origin = hit.point;
                AddLight(colorIndex);
            }
        }
        else if (objectType == ObjectType.receiver)
        {
            AG_Receiver receiver = hit.transform.GetComponent<AG_Receiver>();
            if (colorIndex == (int)receiver.color || receiver.color == AG_Color.ColorName.none)
                receiver.alimented = true;

            if (currenttotalLight < maxLineOfLine)
                SetWaitingPrismaColor();

            CheckVictory();
        }
        else if (objectType == ObjectType.prisma)
        {            
            if (currenttotalLight < maxLineOfLine)
            {
                Debug.Log("Prisma");
                InitNewPrisma(hit.transform, colorIndex);
            }
        }
        else if (objectType == ObjectType.filter)
        {
            if (currenttotalLight < maxLineOfLine)
            {
                //Debug.Log("colorIndex : " + colorIndex + " filter color index" + (int)hit.transform.GetComponent<AG_Filter>().color);
                //Debug.Log("color index : " + colorIndex + " -> " + hit.transform.GetComponent<AG_Filter>().color);
                if (colorIndex == (int)hit.transform.GetComponent<AG_Filter>().color)
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
            currenttotalLight = 0;
            currentEmitterIndex = 0;
            SpawnLights();
        }
    }
    private bool victory;
    private bool CheckVictory()
    {
        victory = true;
        foreach (AG_Receiver receiver in listReceiver)
            if (!receiver.alimented)
                victory = false;

        if (victory)
        {
            //victoryScreen.SetActive(true);
            Debug.Log("YOU WON");
            AG_EndLevel.SaveProgression();
        }
        return victory;
    }

    /*private int TotalLightToGenerate()
    {
        int value = 0;
        for (int i = 0; i < listLightConstructor.Length; i++)
        {
            value += listLightConstructor[i].Count;
        }
        return value;
    }*/

    private void SpawnLights()
    {
        Debug.Log(currenttotalLight);
        //currentLight = 0;
        //foreach (LightConstructor listConstrucor in listLightConstructor)
        if (currenttotalLight < maxLineOfLine && currentLight < listLightConstructor[currentEmitterIndex].Count)
        {
            listLight[currenttotalLight].SetActive(true);
            listLight[currenttotalLight].GetComponent<AG_Light_Mono>().Init(
                listLightConstructor[currentEmitterIndex][currentLight].colorIndex,
                new AG_Line(listLightConstructor[currentEmitterIndex][currentLight].origin,
                listLightConstructor[currentEmitterIndex][currentLight].end, lightWidth), false);

            LightAnim();
            currentLight++;
            currenttotalLight++;
        }
        else if (currentEmitterIndex < listLightConstructor.Length)
            currentEmitterIndex++;
        else
        {
            currentLight = 0;

            listLightConstructor = new List<LightConstructor>[listEmitter.Length];
            for (int i = 0; i < listEmitter.Length; i++)
                listLightConstructor[i] = new List<LightConstructor>();

            if (victory)
            {
                //victoryScreen.SetActive(true);
                //Debug.Log("YOU WON");
                VictoryAnim();
            }
        }
    }


    public GameObject mirrorChock;
    public Transform listObject;
    private Sequence inTween;
    private void LightAnim()
    {
        AG_Line line = listLight[currenttotalLight].GetComponent<AG_Light_Mono>().ag_light.GetLightValue();
        float duration = 0.0005f * line.distance;
        RectTransform light = listLight[currenttotalLight].GetComponent<RectTransform>();

        inTween = DOTween.Sequence();
        inTween.Append(light.DOSizeDelta(new Vector2(line.distance, line.width), duration))
               .AppendCallback(() => {
                   if (mirrorChock != null && listObject != null)
                       Instantiate(mirrorChock, line.end, Quaternion.Euler(0, 0, 0), listObject); })
               .AppendInterval(0.005f)
               .OnComplete(() => { SpawnLights(); });
        inTween.Play();
    }

    private void VictoryAnim()
    {
        Sequence inTween = DOTween.Sequence();
        inTween.AppendInterval(0.3f).OnComplete(() => { victoryScreen.SetActive(true); }).Play();
    }

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
        if (colorIndex == 1)
            arrayToReturn = new int[] { 6, 2 };
        else if (colorIndex == 2)
            arrayToReturn = new int[] { 1, 2 };
        else if (colorIndex == 3)
            arrayToReturn = new int[] { 2, 4 };
        else if (colorIndex == 4)
            arrayToReturn = new int[] { 3, 5 };
        else if (colorIndex == 5)
            arrayToReturn = new int[] { 4, 6 };
        else if (colorIndex == 6)
            arrayToReturn = new int[] { 1, 5 };
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
/*
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
}*/