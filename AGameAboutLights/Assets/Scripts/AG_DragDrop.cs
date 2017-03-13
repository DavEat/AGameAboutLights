using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AG_DragDrop : AG_Singleton<AG_DragDrop> {
    #region Var
    [SerializeField] private LayerMask layer;

    #region RotationManager    
    [SerializeField] private LayerMask layerRotation;
    [SerializeField] private Transform _rotation;
    private bool _rotating;

    private readonly int[] remarkableAngles45 = {-180, -135, -90, -45, 0, 45, 90, 135, 180};
    private readonly int[] remarkableAngles90 = { -180, -90, 0, 90, 180 };
    #endregion

    [HideInInspector] public bool lazerTurnOn = false; 
	private bool onInventory, objectDragged;
	private Transform _downObject;
	private Vector2 mousePos;

    //private AG_LightsManagement _lightsManagement;

	[SerializeField] private AG_Grid grid;	
	[SerializeField] private AG_Inventory inventory;

    [SerializeField] public UnityEvent toggleLight;

    #region Inventory Var
    private bool _creatingNewObject;
    private Transform _enterObj;
    #endregion
    #endregion

    #region Struct
    public Transform downObject
    {
        get { return _downObject; }
        private set { _downObject = value; }
    }
    #endregion

    public RaycastHit2D RaycastScreenPoint(Vector2 inputPosition)
	{
		Debug.DrawRay (inputPosition, Vector2.up * 10, Color.red, 10);
		return Physics2D.Raycast(inputPosition, Vector2.up, 0.01f, layer);
	}

	void Update ()
	{
		if (true)
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
        if (_rotation.gameObject.activeSelf && !_rotating && target != null && _rotation != null)
        {
            RaycastHit2D hitRotation = Physics2D.Raycast(Input.mousePosition, target.rotation * Vector2.up, 0.01f, layerRotation);
            if (hitRotation.collider != null)
                _rotating = true;
            else _rotation.gameObject.SetActive(false);
        }

        if (!_rotating)
        {
            RaycastHit2D hit = RaycastScreenPoint(inputPosition);
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
                    DiplayGrid(true);
                }
                else if (hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.inventory)
                {
                    mousePos = inputPosition;
                    _enterObj = hit.transform;
                    _creatingNewObject = true;                    
                }
            }
        }
	}
    
	private void OnPointer(Vector2 inputPosition)
	{
        if (_rotating && target != null)
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
                if (downObject != null)
                    downObject.parent.position = inputPosition;

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
                    DiplayGrid(true);
                }
            }
        }
    }

	private void OnPointerUp(Vector2 inputPosition)
	{
        inventory.SetScroll(true);

        if (_rotating)
            _rotating = false;        
        else
        {
            RaycastHit2D hit = RaycastScreenPoint(inputPosition);
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
                            target = downObject.parent;
                            _rotation.transform.eulerAngles = hit.transform.parent.eulerAngles;
                            _rotation.transform.position = hit.transform.parent.position;
                            _rotation.gameObject.SetActive(!_rotation.gameObject.activeSelf);
                            
                            downObject = null;
                            DiplayGrid(false);
                        }
                    }
                    else
                    {
                        if (inputPosition.x < inventory.inventoryLimite.position.x)
                            inventory.AddToInventory(downObject);
                        else
                            downObject.parent.position = AG_Grid.inst.ChoseClosestPoint(inputPosition);

                        downObject = null;
                        DiplayGrid(false);
                    }
                }
                else
                {
                    DiplayGrid(false);
                }
            }
        }
	}

	public void DiplayGrid(bool value)
	{
		if (!AG_GameSettings.displayGrid)
			grid.gameObject.SetActive(value);
	}
}
