#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//#if UNITY_EDITOR
using UnityEditor;
//#endif

public class SG_GeneratID : MonoBehaviour {

	//private int _id;

	public void Generator()
	{
		//_id = 0;
        //SG_Savable[] savables = SG_Collector.CollectSavable();

        //foreach (SG_Savable save in savables)
        //save.id = _id++;
	}
}

//#if UNITY_EDITOR
[CustomEditor(typeof(SG_GeneratID))]
public class SG_GeneratIDEditor : Editor {

	public override void OnInspectorGUI()
	{
		SG_GeneratID myScript = (SG_GeneratID)target;

		DrawDefaultInspector();

		if (GUILayout.Button("Send"))
		{
			myScript.Generator();
		}
	}
}
#endif