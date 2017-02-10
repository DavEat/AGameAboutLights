using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_RotationManager : MonoBehaviour {

    #region RotationManager
    #region Var
    private Transform _target;
    private GameObject _thisG;
    private AG_DragDrop _dragdrop;
    private bool _rotating;
    [SerializeField] private LayerMask _layer;
    #endregion

    #region Struct
    public Transform target
    {
        get { return _target; }
        set { _target = value; }
    }
    public GameObject thisG
    {
        get { return _thisG; }
        private set { _thisG = value; }
    }
    public AG_DragDrop dragdrop
    {
        get { return _dragdrop; }
        private set { _dragdrop = value; }
    }
    public LayerMask layer
    {
        get { return _layer; }
        private set { _layer = value; }
    }
    #endregion

    private void Start()
    {
        thisG = gameObject;
        dragdrop = FindObjectOfType<AG_DragDrop>();
        thisG.SetActive(false);
    }

    private void Update ()
    {
        if (thisG.activeSelf && _target != null)
        {
            if (Input.GetMouseButtonUp(0))
                _rotating = false;
            else if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, target.rotation * Vector2.up, 0.01f, layer);
                if (hit.collider != null)
                    _rotating = true;
            }
            
            if (_rotating)
            {
                Vector2 dir = Input.mousePosition - transform.position;
                float angleZ = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                Vector3 angles = new Vector3(0, 0, angleZ);
                transform.localEulerAngles = angles;
                target.localEulerAngles = angles;
            }
        }
	}
    
    public void ToogleActive()
    {
        thisG.SetActive(!thisG.activeSelf);
    }
    #endregion
}
