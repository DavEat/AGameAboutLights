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
			/*case SavableObjType.platform: savable = new SG_Platform (transform, id);
				break;
			case SavableObjType.cube: savable = new SG_Cube (transform, id);
				break;
			case SavableObjType.sphere: savable = new SG_Cube (transform, id);
				break;
			case SavableObjType.human: savable = new SG_Human (transform, id);
				break;
			case SavableObjType.controllableball: savable = new SG_ControllableBall (transform, id);
				break;*/
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
	//private Save.GameplayInfos _data = new Save.GameplayInfos();
	#endregion
	#region Struct
	/*public Save.GameplayInfos data
	{
		get { return _data; }
		private set { _data = value;}
	}*/
	#endregion

	public SG_Gameplay(Transform _trasform, int _id)
	{
		savableObjType = SG_Savable.SavableObjType.gameplay;

		/*_data.id = _id;

		_data.haveIt = true;
		_data.haveCall = true;
		_data.haveControl = true;*/
	}
}