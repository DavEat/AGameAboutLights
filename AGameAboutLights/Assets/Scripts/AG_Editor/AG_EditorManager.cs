using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class AG_EditorManager : MonoBehaviour {

    #region Var
    public bool _editing;
    
    private Transform _selectedObj;

    [SerializeField] private AG_ChangeIntTool _changeIntTool;
    [SerializeField] private AG_ScaleToolManager _scaleTool;
    public bool snap = true;

    #endregion

    private void Start()
    {
        AG_DragDrop.inst.enabled = false;
    }

    #region Struct
    public AG_ScaleToolManager scaleTool
    {
        get { return _scaleTool; }
        private set { _scaleTool = value; }
    }
    #endregion

    public void ToggleSnap()
    {
        snap = !snap;
    }

    #region Graphics & Animations of the editor

    private RectTransform _currentOpenMenu;
    private float openTime = 0.5f, closeTime = 0.3f;

    public void ToogleObjectsMenu(RectTransform menu)
    {
        if (_currentOpenMenu != null/* && _currentOpenMenu.anchoredPosition.x <= 0*/)
        {
            if (true)
            {
                if (menu != _currentOpenMenu)
                {
                    OpenCloseMenu(menu, _currentOpenMenu);
                    _currentOpenMenu = menu;
                }
                else
                {
                    inTweenOpen.Kill();
                    CloseMenu(_currentOpenMenu);
                }
            }
        }
        else
        {
            if (menu != _currentOpenMenu)
                _currentOpenMenu = menu;
            OpenMenu(_currentOpenMenu);
        }
    }
    Sequence inTweenOpen;
    private void OpenMenu(RectTransform rect)
    {
        inTweenOpen = DOTween.Sequence();
        inTweenOpen.Append(rect.DOAnchorPos(new Vector2(0, 0), openTime));
        inTweenOpen.Play();
    }
    private void OpenCloseMenu(RectTransform rectOpen, RectTransform rectClose)
    {
        Sequence inTween = DOTween.Sequence();
        inTween.Append(rectClose.DOAnchorPos(new Vector2(rectClose.sizeDelta.x * 0.8f, 0), closeTime * 0.8f))
               .Append(rectClose.DOAnchorPos(new Vector2(rectClose.sizeDelta.x, 0), closeTime * 0.2f))
               .Join(rectOpen.DOAnchorPos(new Vector2(0, 0), openTime));
        inTween.Play();
    }
    private void CloseMenu(RectTransform rect)
    {
        Debug.Log("close : " + rect.name);
        Sequence inTween = DOTween.Sequence();
        inTween.Append(rect.DOAnchorPos(new Vector2(rect.sizeDelta.x + 20, 0), closeTime))
               .OnComplete(() => { _currentOpenMenu = null; });
        inTween.Play();
    }
    #endregion


    #region Drag & Drop editor object

    #region Var
    [SerializeField] private LayerMask layer;

    #region RotationManager    
    [SerializeField] private LayerMask layerRotation;
    [SerializeField] private Transform _rotation;
    private bool _rotating;

    private readonly int[] remarkableAngles45 = { -180, -135, -90, -45, 0, 45, 90, 135, 180 };
    private readonly int[] remarkableAngles90 = { -180, -90, 0, 90, 180 };
    #endregion

    [HideInInspector] public bool lazerTurnOn = false;
    private bool _onInventory, _objectDragged;
    private Transform _downObject;
    private Vector2 _downPosDiff;
    private Vector2 mousePos;

    //private AG_LightsManagement _lightsManagement;

    [SerializeField] private AG_Grid grid;
    [SerializeField] private AG_Inventory inventory;

    #region Inventory Var
    private bool _creatingNewObject;
    private Transform _enterObj;
    #endregion
    #endregion

    [SerializeField] private UnityEvent toggleLight;

    #region Struct
    public Transform downObject
    {
        get { return _downObject; }
        private set { _downObject = value; }
    }
    #endregion

    public RaycastHit2D RaycastScreenPoint()
	{
		Debug.DrawRay (Input.mousePosition, Vector2.up * 10, Color.red, 10);
		return Physics2D.Raycast(Input.mousePosition, Vector2.up, 0.01f, layer);
	}

	void Update ()
	{
		if (_editing)
		{
			#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_EDITOR)
            Vector2 inputPosition = Input.mousePosition;
			if (Input.GetMouseButtonUp(0))
			{				
				OnPointerUp(inputPosition);
			}
			else if (Input.GetMouseButtonDown(0))
			{
				OnPointerDown(inputPosition);
			}
			else if (Input.GetMouseButton(0))
			{
				OnPointer(inputPosition);
			}
			#else
			if (Input.touchCount == 1)
			{
			Vector3 inputPosition = Input.touches[0].position;
			if (Input.touches[0].phase == TouchPhase.Ended)
			{
			OnPointerUp(inputPosition);
			}
			else if (Input.touches[0].phase == TouchPhase.Began)
			{
			OnPointerDown(inputPosition);
			}
			else if (Input.touches[0].phase == TouchPhase.Moved)
			{
			OnPointer(inputPosition);
			}
			}
			#endif
		}
	}

    private Transform target;
	private void OnPointerDown(Vector2 inputPosition)
    {
        if (scaleTool.ActiveSelf())
            scaleTool.OnPointerDown(inputPosition);

        if (_rotation.gameObject.activeSelf && !_rotating && target != null && _rotation != null)
        {
            RaycastHit2D hitRotation = Physics2D.Raycast(Input.mousePosition, target.rotation * Vector2.up, 0.01f, layerRotation);
            if (hitRotation.collider != null)
                _rotating = true;
            else _rotation.gameObject.SetActive(false);
        }

        if (!_rotating)
        {
            RaycastHit2D hit = RaycastScreenPoint();
            if (hit.collider != null)
            {
                if (AG_LightsManagementNew.inst.lightTurnOn)
                    toggleLight.Invoke();

                if (hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.movable)
                {
                    if (lazerTurnOn)
                        toggleLight.Invoke();
                    if (_rotation.gameObject.activeSelf)
                        _rotation.gameObject.SetActive(false);

                    mousePos = inputPosition;
                    downObject = hit.transform;
                    _downPosDiff = (Vector2)downObject.parent.position - inputPosition;
                    //DiplayGrid(true);
                }
                else if (hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.inventory)
                {                    
                    if (inputPosition.x > inventory.inventoryLimite.position.x)
                    {
                        mousePos = inputPosition;
                        _enterObj = hit.transform;
                        _creatingNewObject = true;
                    }
                    else _changeIntTool.SetTarget(hit.transform);
                }
            }
        }        
    }
    
	private void OnPointer(Vector2 inputPosition)
	{
        if (_selectedObj != null && scaleTool.ActiveSelf() && scaleTool.OnPointer(inputPosition, _selectedObj.parent.GetComponent<AG_EditorElement>()))
        {
            //DiplayGrid(true);
        }
        else if (_rotating && target != null)
        {
            Vector2 dir = inputPosition - (Vector2)_rotation.position;
            float angleZ = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            Vector3 angles;
            if (Vector2.Distance(inputPosition, target.position) < _rotation.GetChild(1).localPosition.y)
            {
                angles = new Vector3(0, 0, angleZ);
            }
            else
            {
                int[] snapAngle;
                if (target.GetComponent<AG_ElementType>().objectType == ObjectType.prisma)
                    snapAngle = remarkableAngles90;
                else snapAngle = remarkableAngles45;

                int currentNearest = snapAngle[0];
                int currentDifference = Mathf.Abs(currentNearest - Mathf.RoundToInt(angleZ));

                for (int i = 1; i < snapAngle.Length; i++)
                {
                    int diff = Mathf.Abs(snapAngle[i] - Mathf.RoundToInt(angleZ));
                    if (diff < currentDifference)
                    {
                        currentDifference = diff;
                        currentNearest = snapAngle[i];
                    }
                }
                angles = new Vector3(0, 0, currentNearest);
            }
            _rotation.eulerAngles = angles;
            target.eulerAngles = angles;
        }
        else
        {
            if ((Vector2)inputPosition != mousePos)
            {
                if (downObject != null)
                {
                    downObject.parent.position = inputPosition + _downPosDiff;
                }
            }

            if (_creatingNewObject && inputPosition.x > inventory.inventoryLimite.position.x)
            {
                Transform obj = _enterObj.GetComponent<AG_InventoryObjectManager>().OnSelect();
                if (obj != null)
                {
                    _creatingNewObject = false;
                    inventory.SetScroll(false);
                    if (lazerTurnOn)
                        toggleLight.Invoke();
                    if (_rotation.gameObject.activeSelf)
                        _rotation.gameObject.SetActive(false);
            
                    //mousePos = inputPosition;
                    downObject = obj.GetChild(0);

                    AG_Emitter emitter = obj.GetComponent<AG_Emitter>();
                    if (emitter != null)
                        listEmitter.Add(emitter);
                    else
                    {
                        AG_Receiver receiver = obj.GetComponent<AG_Receiver>();
                        if (receiver != null)
                            listReceiver.Add(receiver);
                    }
                    //DiplayGrid(true);
                }
            }
        }
    }

	private void OnPointerUp(Vector2 inputPosition)
	{
        inventory.SetScroll(true);

        if (scaleTool.ActiveSelf() && _selectedObj != null)
            scaleTool.OnPointerUp(inputPosition, _selectedObj.parent.GetComponent<AG_EditorElement>());

        if (_rotating)
            _rotating = false;        
        else
        {
            RaycastHit2D hit = RaycastScreenPoint();
            if (hit.collider != null && hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.movable)
            {
                if (lazerTurnOn)
                    toggleLight.Invoke();
                if (downObject != null)
                {
                    if (inputPosition == mousePos)
                    {
                        if (hit.transform == downObject)
                        {
                            //DiplayGrid(false);
                            _selectedObj = downObject;

                            if (_selectedObj.GetComponent<AG_ElementType>().objectType == ObjectType.wall)
                                scaleTool.InitScaleTool(_selectedObj.parent.GetComponent<RectTransform>());

                            //---- add hightlight on selectedobj

                            downObject = null;
                        }
                    }
                    else
                    {
                        if (inputPosition.x > inventory.inventoryLimite.position.x)
                        {
                            inventory.AddToInventory(downObject);

                            AG_Emitter emitter = downObject.GetComponent<AG_Emitter>();
                            if (emitter != null)
                                listEmitter.Remove(emitter);
                            else
                            {
                                AG_Receiver receiver = downObject.GetComponent<AG_Receiver>();
                                if (emitter != null)
                                    listReceiver.Remove(receiver);
                            }
                        }
                        else if (snap)
                            downObject.parent.position = AG_Grid.inst.ChoseClosestPoint(downObject.parent.position);

                        downObject = null;
                        //DiplayGrid(false);
                    }
                }
                else
                {
                    //DiplayGrid(false);
                }
            }
        }
	}

	public void DiplayGrid(bool value)
	{
		if (!AG_GameSettings.displayGrid && grid.gameObject.activeSelf != value)
			grid.gameObject.SetActive(value);
	}

    public void EnableRotation()
    {
        if (_selectedObj != null)
        {
            target = _selectedObj.parent;
            _rotation.transform.eulerAngles = _selectedObj.parent.eulerAngles;
            _rotation.transform.position = _selectedObj.parent.position;
            _rotation.gameObject.SetActive(!_rotation.gameObject.activeSelf);
        }
    }

    #endregion

    #region Change Selected Object Color Function

    public void ChangeColor(int _color)
    {
        if (_selectedObj != null)
        {
            AG_ElementType elem = _selectedObj.parent.GetComponent<AG_ElementType>();
            if (elem != null && elem.objectType != ObjectType.wall)
            {
                if (elem.objectType == ObjectType.emitter && _color == 0)
                {
                    AG_EditorDebug.DebugLog("A Emitter can't have the white color");
                }
                else
                {
                    ((AG_ElementType_Color)elem).color = (AG_Color.ColorName)_color;

                    Image img = _selectedObj.parent.GetComponent<Image>();
                    if (img != null)
                        img.color = AG_Color.colorList[_color];
                }
            }
            else Debug.Log("You can't change de color of that element");
        }
    }

    public List<AG_Emitter> listEmitter;
    public List<AG_Receiver> listReceiver;
    [SerializeField] private GameObject switchButton, objectlist, optionslist, objectlistBut, optionslistBut;
    public void Testlevel()
    {
        if (_editing)
        {
            foreach (AG_Emitter emitter in AG_LightsManagementNew.inst.listEmitter)
                listEmitter.Add(emitter);
            AG_LightsManagementNew.inst.listEmitter = listEmitter.ToArray();
            foreach (AG_Receiver receiver in AG_LightsManagementNew.inst.listReceiver)
                listReceiver.Add(receiver);
            AG_LightsManagementNew.inst.listReceiver = listReceiver.ToArray();

            listEmitter.RemoveAll(xx => xx == xx);
            listReceiver.RemoveAll(xx => xx == xx);
        }
        //else if (AG_LightsManagementNew.inst.lightTurnOn)
        //    AG_DragDrop.inst.toggleLight.Invoke();

        _editing = !_editing;
        AG_DragDrop.inst.enabled = !AG_DragDrop.inst.enabled;
        switchButton.SetActive(!switchButton.activeSelf);
        objectlist.SetActive(!objectlist.activeSelf);
        optionslist.SetActive(!optionslist.activeSelf);
        objectlistBut.SetActive(!objectlistBut.activeSelf);
        optionslistBut.SetActive(!optionslistBut.activeSelf);
    }
    #endregion
}
