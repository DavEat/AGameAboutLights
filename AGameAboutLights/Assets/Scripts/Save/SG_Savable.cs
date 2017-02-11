using UnityEngine;

public class SG_Savable : MonoBehaviour {
	
	public enum SavableObjType
	{
		platform,
		cube,
		sphere,
		controllableball,
		human,
		gameplay,
	}
	#region Var
	[SerializeField] private SavableObjType savableObjectType;
	[SerializeField] private int _id = -1;
	private SG_SavableObj _savable;
	#endregion
	#region Struct
	public int id
	{
		get { return _id; }
		set { _id = value; }
	}
	public SavableObjType savableObjType
	{
		get { return savableObjectType; }
		protected set { savableObjectType = value; }
	}
	public SG_SavableObj savable
	{
		get { return _savable; }
		set { _savable = value; }
	}
	#endregion

	public void SetClass()
	{
		if (savable == null)
		switch (savableObjType) 
		{
			case SavableObjType.platform: savable = new SG_Platform (transform, id);
				break;
			case SavableObjType.cube: savable = new SG_Cube (transform, id);
				break;
			case SavableObjType.sphere: savable = new SG_Cube (transform, id);
				break;
			case SavableObjType.human: savable = new SG_Human (transform, id);
				break;
			case SavableObjType.controllableball: savable = new SG_ControllableBall (transform, id);
				break;
			case SavableObjType.gameplay: savable = new SG_Gameplay (transform, id);
				break;
			default:
				Debug.Log ("Uncorrect enter in saves");
				break;
		}
	}
}

public class SG_SavableObj
{
	#region Var
	private SG_Savable.SavableObjType savableObjectType;
    private Transform _transform;
	#endregion
	#region Struct
	public SG_Savable.SavableObjType savableObjType
	{
		get { return savableObjectType; }
		protected set { savableObjectType = value; }
	}
    public Transform transform
    {
        get { return _transform; }
        protected set { _transform = value; }
    }
    #endregion

    public SG_SavableObj() {}
}
public class SG_Gameplay : SG_SavableObj
{
	#region Var
	private Save.GameplayInfos _data = new Save.GameplayInfos();
	#endregion
	#region Struct
	public Save.GameplayInfos data
	{
		get { return _data; }
		private set { _data = value;}
	}
	#endregion

	public SG_Gameplay(Transform _trasform, int _id)
	{
		savableObjType = SG_Savable.SavableObjType.gameplay;

		_data.id = _id;

		_data.haveIt = true;
		_data.haveCall = true;
		_data.haveControl = true;
	}
}
public class SG_Human : SG_SavableObj
{
	#region Var
	private Save.HumanInfos _data = new Save.HumanInfos();
	#endregion
	#region Struct
	public Save.HumanInfos data
	{
		get { return _data; }
		private set { _data = value;}
	}
	#endregion

	public SG_Human(Transform _trasform, int _id)
	{
		savableObjType = SG_Savable.SavableObjType.human;
        transform = _trasform;

		_data.id = _id;
		SG_Savable parentSave = _trasform.parent.GetComponent<SG_Savable>();
		if (parentSave != null)
			_data.parentId = parentSave.id;
		else _data.parentId = -1;

		_data.transform.position  = _trasform.position;
		_data.transform.rotation  = _trasform.eulerAngles;
	}
}
public class SG_ControllableBall : SG_SavableObj
{
	#region Var
	private Save.BallInfos _data = new Save.BallInfos();
	#endregion
	#region Struct
	public Save.BallInfos data
	{
		get { return _data; }
		private set { _data = value;}
	}
	#endregion

	public SG_ControllableBall(Transform _trasform, int _id)
	{
		savableObjType = SG_Savable.SavableObjType.controllableball;
        transform = _trasform;

        _data.id = _id;
		SG_Savable parentSave = _trasform.parent.GetComponent<SG_Savable>();
		if (parentSave != null)
			_data.parentId = parentSave.id;
		else _data.parentId = -1;

		_data.electricPower = 0;
		_data.size = 0;
		_data.transform.position  = _trasform.position;
		_data.transform.rotation  = _trasform.eulerAngles;
	}
}
public class SG_Platform : SG_SavableObj
{
	#region Var
	private Save.platform _data =  new Save.platform();
	#endregion
	#region Struct
	public Save.platform data
	{
		get { return _data; }
		private set { _data = value;}
	}
	#endregion

	public SG_Platform(Transform _trasform, int _id)
	{
		savableObjType = SG_Savable.SavableObjType.platform;
        transform = _trasform;

        _data.id = _id;

		_data.active = true;
		_data.transform.position  = _trasform.position;
		_data.transform.rotation  = _trasform.eulerAngles;
		_data.transform.scale  = _trasform.localScale;
	}
}
public class SG_Cube : SG_SavableObj
{
	#region Var
	private Save.cube _data = new Save.cube();
	#endregion
	#region Struct
	public Save.cube data
	{
		get { return _data; }
		private set { _data = value;}
	}
	#endregion

	public SG_Cube(Transform _trasform, int _id)
	{
		savableObjType = SG_Savable.SavableObjType.cube;
        transform = _trasform;

		_data.id = _id;
		SG_Savable parentSave = _trasform.parent.GetComponent<SG_Savable>();
		if (parentSave != null)
			_data.parentId = parentSave.id;
		else _data.parentId = -1;

		_data.transform.position  = _trasform.position;
		_data.transform.rotation  = _trasform.eulerAngles;
		_data.transform.scale  = _trasform.localScale;
	}
}
/*
#if UNITY_EDITOR

[CustomEditor(typeof(SG_Savable))]
public class SG_SavableEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		
		SG_Savable myScript = (SG_Savable)target;
		//if (myScript.savableObjType == Savable.SavableObjType.cube)
		//	Debug.Log ("mmmm");

		DrawDefaultInspector();

		GUILayout.BeginArea ();
		GUI.Te
		GUILayout.EndArea ();
	}
}

#endif*/
/*
#region old
[CustomEditor(typeof(Savable)), CanEditMultipleObjects]
public class PropertyHolderEditor : Editor {

	public SerializedProperty 
	state_Prop,
	valForAB_Prop,
	valForA_Prop,
	valForC_Prop,
	controllable_Prop;

	void OnEnable () {
		// Setup the SerializedProperties
		state_Prop = serializedObject.FindProperty ("state");
		valForAB_Prop = serializedObject.FindProperty("valForAB");
		valForA_Prop = serializedObject.FindProperty ("valForA");
		valForC_Prop = serializedObject.FindProperty ("valForC");
		controllable_Prop = serializedObject.FindProperty ("controllable");        
	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();

		EditorGUILayout.PropertyField( state_Prop );

		Savable.SavableObjType st = (Savable.SavableObjType)state_Prop.enumValueIndex;

		switch( st ) {
		case Savable.SavableObjType.platform:            
			EditorGUILayout.PropertyField( controllable_Prop, new GUIContent("controllable") );            
			EditorGUILayout.IntSlider ( valForA_Prop, 0, 10, new GUIContent("valForA") );
			EditorGUILayout.IntSlider ( valForAB_Prop, 0, 100, new GUIContent("valForAB") );
			break;

		case Savable.SavableObjType.door:            
			EditorGUILayout.PropertyField( controllable_Prop, new GUIContent("controllable") );    
			EditorGUILayout.IntSlider ( valForAB_Prop, 0, 100, new GUIContent("valForAB") );
			break;

		case Savable.SavableObjType.human:            
			EditorGUILayout.PropertyField( controllable_Prop, new GUIContent("controllable") );    
			EditorGUILayout.IntSlider ( valForC_Prop, 0, 100, new GUIContent("valForC") );
			break;

		}


		serializedObject.ApplyModifiedProperties ();
	}
}
#endregion

#endif*/

