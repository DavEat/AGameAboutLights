using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


public class AG_DragDrop : MonoBehaviour {
    #region Var
    [SerializeField] private LayerMask layer;

    #region RotationManager    
    [SerializeField] private LayerMask layerRotation;
    [SerializeField] private Transform _rotation;
    private bool _rotating;

    private readonly int[] remarkableAngles = {-180, -135, -90, -45, 0, 45, 90, 135, 180};
    #endregion

    public bool lazerTurnOn = false; 
	private bool onInventory, objectDragged, down;
	private Transform downObject;
	private Vector2 mousePos;

    private AG_LightsManagement _lightsManagement;

	[SerializeField] private AG_Grid grid;	
	[SerializeField] private AG_Inventory inventory;



    #endregion

    #region Struct
    public AG_LightsManagement lightsManagement
    {
        get { return _lightsManagement; }
        set { _lightsManagement = value; }
    }
    #endregion

    public RaycastHit2D RaycastScreenPoint()
	{
		Debug.DrawRay (Input.mousePosition, Vector2.up * 10, Color.red, 10);
		return Physics2D.Raycast(Input.mousePosition, Vector2.up, 0.01f, layer);
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

        RaycastHit2D hit = RaycastScreenPoint();
		if (hit.collider != null && hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.movable)
		{
			if (lazerTurnOn)
				lightsManagement.ToggleLight();
            if (_rotation.gameObject.activeSelf)
                _rotation.gameObject.SetActive(false);

			mousePos = inputPosition;
			downObject = hit.transform;
			DiplayGrid(true);
		}
	}

	private void OnPointer(Vector2 inputPosition)
	{
        if (_rotating && target != null)
        {
            Vector2 dir = inputPosition - (Vector2)_rotation.position;
            float angleZ = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            Debug.Log("angle z : " + angleZ);
            Vector3 angles;
            if (Vector2.Distance(inputPosition, target.position) > _rotation.GetChild(1).localPosition.y * 1.3f)
            {
                angles = new Vector3(0, 0, angleZ);                
            }
            else
            {
                int currentNearest = remarkableAngles[0];
                int currentDifference = Mathf.Abs(currentNearest - Mathf.RoundToInt(angleZ));

                for (int i = 1; i < remarkableAngles.Length; i++)
                {
                    int diff = Mathf.Abs(remarkableAngles[i] - Mathf.RoundToInt(angleZ));
                    if (diff < currentDifference)
                    {
                        currentDifference = diff;
                        currentNearest = remarkableAngles[i];
                    }
                }
                Debug.Log("current nearest : " + currentNearest);
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
        }
    }

	private void OnPointerUp(Vector2 inputPosition)
	{
        if (_rotating)
            _rotating = false;
        else
        {
            RaycastHit2D hit = RaycastScreenPoint();
            if (hit.collider != null && hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.movable)
            {
                if (lazerTurnOn)
                    lightsManagement.ToggleLight();
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

                            /*Debug.Log("hit transform : " + hit.transform.name + " - downObject : " + downObject + " - input mouse pos : " + inputPosition + " - mousepos : " + mousePos);
                            float angle = 45;
                            if (downObject.GetComponent<AG_ElementType>().objectType == ObjectType.prisma)
                                angle = 90;
                            downObject.parent.localEulerAngles = new Vector3 (0, 0, downObject.parent.localEulerAngles.z + angle);*/
                            downObject = null;
                            DiplayGrid(false);
                        }
                    }
                    else
                    {
                        if (inputPosition.x < inventory.inventoryLimite.position.x)
                            downObject.parent.position = ChoseClosestPoint(inventory.listPoints, inputPosition).position;
                        else
                            downObject.parent.position = ChoseClosestPoint(grid.listPoints, inputPosition).position;

                        downObject = null;
                        DiplayGrid(false);
                    }
                }
                else
                {
                    //downObject = null;
                    DiplayGrid(false);
                }
            }
        }
	}

	public Transform ChoseClosestPoint(List<Transform> list, Vector2 pos)
	{
		if (list != null)
		{
			if (list.Count == 1)
				return list[0];
			else if (list != null && list.Count > 1)
			{
				Transform lastSelected = list[0];
				for (int i = 1; i < list.Count; i++)
				{
					if (Vector2.Distance(lastSelected.position, pos) > Vector2.Distance(list[i].position, pos))
						lastSelected = list[i];
				}
				return lastSelected;
			}
		}
		return null;
	}

	public void DiplayGrid(bool value)
	{
		if (!AG_GameSettings.displayGrid)
			grid.gameObject.SetActive(value);
	}
}
