using UnityEngine;

public class AG_ScaleToolManager : MonoBehaviour {

    #region Var
    [SerializeField] private LayerMask layer;	
    [SerializeField] private Transform[] corners;
    private int selectedCornerIndex = -1, xCornerIndex, yCornerIndex;
    private GameObject _gameObject;
    private AG_EditorManager editorM;	
    #endregion

    private void Start()
    {
        _gameObject = gameObject;
        _gameObject.SetActive(false);
        editorM = FindObjectOfType<AG_EditorManager>();
    }

    public bool ActiveSelf()
    {
        return _gameObject.activeSelf;
    }

    public void InitScaleTool(RectTransform rect)
    {
        _gameObject.SetActive(true);
        corners[2].position = rect.position;
        corners[0].position = new Vector2(rect.position.x, rect.sizeDelta.y + rect.position.y);
        corners[3].position = new Vector2(rect.sizeDelta.x + rect.position.x, rect.position.y);
        corners[1].position = new Vector2(rect.sizeDelta.x + rect.position.x, rect.sizeDelta.y + rect.position.y);
    }

    private RaycastHit2D RaycastScreenPoint(Vector2 inputPosition)
    {
        return Physics2D.Raycast(inputPosition, Vector2.up, 0.01f, layer);
    }

    public void OnPointerDown(Vector2 inputPosition)
    {
        RaycastHit2D hit = RaycastScreenPoint(inputPosition);
        if (hit.transform != null)
        {
            for (int i = 0; i < corners.Length; i++)
                if (corners[i] == hit.transform)
                    selectedCornerIndex = i;

            if (selectedCornerIndex == 0)
            {
                yCornerIndex = 1;
                xCornerIndex = 2;
            }
            else if (selectedCornerIndex == 1)
            {
                yCornerIndex = 0;
                xCornerIndex = 3;
            }
            else if (selectedCornerIndex == 2)
            {
                yCornerIndex = 3;
                xCornerIndex = 0;
            }
            else if (selectedCornerIndex == 3)
            {
                yCornerIndex = 2;
                xCornerIndex = 1;
            }
        } else _gameObject.SetActive(false);
    }

    public bool OnPointer(Vector2 inputPosition, AG_ElementType elem)
    {
        if (selectedCornerIndex != -1)
        {
            ManageCornersPosition(inputPosition, elem);
            return true;
        } else return false;
    }

    public void OnPointerUp(Vector2 inputPosition, AG_ElementType elem)
    {
        if (editorM.snap)
            ManageCornersPosition(AG_Grid.ChoseClosestPoint(inputPosition), elem);

        selectedCornerIndex = -1;

        if (elem.objectType == ObjectType.wall)
        {
            ((AG_Wall)elem).RestCollider();
            elem.transform.GetChild(0).GetComponent<AG_Wall>().RestCollider();
        }
    }

    private void ManageCornersPosition(Vector2 inputPosition, AG_ElementType elem)
    {
        corners[selectedCornerIndex].position = inputPosition;
        corners[xCornerIndex].position = new Vector2(inputPosition.x, corners[xCornerIndex].position.y);
        corners[yCornerIndex].position = new Vector2(corners[yCornerIndex].position.x, inputPosition.y);

        if (elem.objectType == ObjectType.wall)
            ((AG_Wall)elem).DoScale(corners[2].position, -(corners[2].position - corners[1].position));
    }
}
