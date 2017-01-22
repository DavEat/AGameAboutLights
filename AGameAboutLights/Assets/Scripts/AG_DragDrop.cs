using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_DragDrop : MonoBehaviour {

    public LayerMask layer;
    public bool lazerTurnOn = false; 
    private bool onInventory, objectDragged, down;
    private Transform downObject;
    private Vector2 mousePos;

    [SerializeField]
    private AG_Grid grid;
    [SerializeField]
    private AG_Inventory inventory;

    void Update ()
    {
        if (!lazerTurnOn)
        {
            #if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_EDITOR)
            Vector3 inputPosition;
            if (Input.GetMouseButtonUp(0))
            {
                inputPosition = Input.mousePosition;
                OnPointerUp(inputPosition);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Input.mousePosition;
                OnPointerDown(inputPosition);
            }
            else if (Input.GetMouseButton(0))
            {
                inputPosition = Input.mousePosition;
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

    private void OnPointerDown(Vector3 inputPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(inputPosition, new Vector3(inputPosition.x, inputPosition.y, 10), Mathf.Infinity, layer);
        if (hit.collider != null && hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.movable)
        {
            mousePos = inputPosition;
            downObject = hit.transform;
            Debug.Log("hit obj : " + downObject);
            DiplayGrid(true);
        }
    }

    private void OnPointer(Vector3 inputPosition)
    {
        if ((Vector2)inputPosition != mousePos)
        if (downObject != null)
            downObject.parent.position = inputPosition;
    }

    private void OnPointerUp(Vector3 inputPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(inputPosition, new Vector3(inputPosition.x, inputPosition.y, 10), Mathf.Infinity, layer);
        if (hit.collider != null && hit.transform.GetComponent<AG_ElementType>().objectInteractionType == ObjectInteractionType.movable)
        if (downObject != null)
        {
            if (hit.transform == downObject && inputPosition == (Vector3)mousePos)
            {
                float angle = 45;
                if (downObject.GetComponent<AG_ElementType>().objectType == ObjectType.prisma)
                    angle = 90;
                downObject.parent.localEulerAngles = new Vector3(0, 0, downObject.parent.localEulerAngles.z + angle);
                downObject = null;
                DiplayGrid(false);
            }
            else
            {
                if (inputPosition.x < inventory.inventoryLimite.position.x)
                    downObject.parent.position = ChoseClosestPoint(inventory.listPoints, inputPosition).position;
                else downObject.parent.position = ChoseClosestPoint(grid.listPoints, inputPosition).position;

                downObject = null;
                DiplayGrid(false);
            }
        }
        else
        {
            downObject = null;
            DiplayGrid(false);
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
