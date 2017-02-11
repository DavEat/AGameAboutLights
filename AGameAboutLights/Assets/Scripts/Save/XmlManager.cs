using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

public class XmlManager : MonoBehaviour
{
	#region Var
    [HideInInspector]
    public static string fileLocation, fileExtention;
    private static string _fileName;
    private Save mySave;
    private string _data;

    //public ListObjectToSave listObjToSave;
    #endregion

    public string fileName
    {
        get { return _fileName; }
        private set { _fileName = value; }
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        fileLocation = Application.dataPath + "/Scenes/Dav/";
        fileExtention = ".BMCSave";
        //fileName = "2017-01-31-01-16-15-12" + fileExtention;
        mySave = new Save();
    }

    private void OnLevelWasLoaded()
    {    
    	Load();
    }

    /*void OnGUI()
    {
        // Loading The Player...    
        if (GUI.Button(new Rect(10, 70, 80, 40), "Load"))
        {

        }

        if (SceneManager.GetActiveScene().name != "Menu")
        {
            // Saving The Player... 
            if (GUI.Button(new Rect(10, 140, 80, 40), "Save"))
            {
                if (listObjToSave != null)
                    Save();
            }

            if (GUI.Button(new Rect(10, 210, 80, 40), "Menu"))
            {
                SceneManager.LoadScene("Menu");
            }

            if (SceneManager.GetActiveScene().name != "IntroRoomMerged")
            if (GUI.Button(new Rect(10, 280, 80, 40), "IntroRoomMerged"))
            {
                SceneManager.LoadScene("IntroRoomMerged");
            }
        }
    }*/

    private void SaveExist()
    {
        _data = "";
        // Load our Save into mySave 
        LoadXML();
        if (_data.ToString() != "")
            // notice how I use a reference to type (Save) here, you need this 
            // so that the returned object is converted into the correct type 
            mySave = (Save)DeserializeObject(_data);
    }

    public void LoadSavedScene(string fileName)
    {
        _fileName = fileName;
        SaveExist();
        if (SceneManager.GetActiveScene().name != mySave.infos.sceneName)
            SceneManager.LoadScene(mySave.infos.sceneName, LoadSceneMode.Single);
        else //if (listObjToSave != null)
        {            
            Load();
        }
    }

    /// <summary>
    /// Load data from the file name in : "fileName"
    /// </summary>
    public void Load()
    {
        LoadXML();
        if (_data.ToString() != "")
        {
			SG_SavableObj[] savalbesObj = SG_Collector.CollectSavableObjSorted();

            // notice how I use a reference to type (UserData) here, you need this 
            // so that the returned object is converted into the correct type 
            mySave = (Save)DeserializeObject(_data);
			int currentElemId = -1;

			// set the players position to the data we loaded 
			currentElemId = mySave.infos.human.id;
			savalbesObj[currentElemId].transform.position = mySave.infos.human.transform.position;
			savalbesObj[currentElemId].transform.eulerAngles = mySave.infos.human.transform.rotation;
			if (mySave.infos.human.parentId != -1)
				savalbesObj[currentElemId].transform.parent = savalbesObj[mySave.infos.human.parentId].transform;

            // set the ball position to the data we loaded
			currentElemId = mySave.infos.ball.id;
			savalbesObj[currentElemId].transform.position = mySave.infos.ball.transform.position;
			savalbesObj[currentElemId].transform.eulerAngles = mySave.infos.ball.transform.rotation;
			if (mySave.infos.ball.parentId != -1)
				savalbesObj[currentElemId].transform.parent = savalbesObj[mySave.infos.ball.parentId].transform;

            // set the cubes position to the data we loaded
			if (mySave.infos.cubes != null)
            for (int i = 0; i < mySave.infos.cubes.Length; i++)
            {
				if (i < savalbesObj.Length)
                {
					currentElemId = mySave.infos.cubes[i].id;
					savalbesObj[currentElemId].transform.position = mySave.infos.cubes[i].transform.position;
					savalbesObj[currentElemId].transform.eulerAngles = mySave.infos.cubes[i].transform.rotation;
					savalbesObj[currentElemId].transform.localScale = mySave.infos.cubes[i].transform.scale;
					if (mySave.infos.cubes[i].parentId != -1)
						savalbesObj[currentElemId].transform.parent = savalbesObj[mySave.infos.cubes[i].parentId].transform;
                }
            }
			if (mySave.infos.platforms != null)
			for (int i = 0; i < mySave.infos.platforms.Length; i++)
			{
				if (i < savalbesObj.Length)
				{
					currentElemId = mySave.infos.platforms[i].id;
					savalbesObj[currentElemId].transform.position = mySave.infos.platforms[i].transform.position;
					savalbesObj[currentElemId].transform.eulerAngles = mySave.infos.platforms[i].transform.rotation;
					savalbesObj[currentElemId].transform.localScale = mySave.infos.platforms[i].transform.scale;
				}
			}
        }
    }

    /// <summary>
    /// Save data in the file name in : "fileName"
    /// </summary>
    public void Save()
    {        

        int chapter = 15, room = 12;
        //Debug.Log("CreateFileName() : " + CreateFileName());
        fileName = CreateFileName();

		mySave.infos = SG_Collector.SortObjToSave();
		mySave.infos.sceneName = SceneManager.GetActiveScene().name;

        //Save save info
        mySave.infos.saveInfo.chapter = chapter; // need class chapter info
        mySave.infos.saveInfo.room = room; // need class room info
        mySave.infos.saveInfo.date = new DateTime();

        /*//Save player
        mySave.infos.player.transform.position = listObjToSave.player.position;
        mySave.infos.player.transform.rotation = listObjToSave.player.localEulerAngles;

        //Save ball
        mySave.infos.ball.transform.position = listObjToSave.ball.position;
        mySave.infos.ball.transform.rotation = listObjToSave.ball.eulerAngles;
        mySave.infos.ball.transform.scale = listObjToSave.ball.localScale;

        //Save cubes
        Save.cube[] cubes = new Save.cube[listObjToSave.arrayCubes.Length];
        for (int i = 0; i < listObjToSave.arrayCubes.Length; i++)
        {
            cubes[i].transform.position = listObjToSave.arrayCubes[i].position;
            cubes[i].transform.rotation = listObjToSave.arrayCubes[i].eulerAngles;
            cubes[i].transform.scale = listObjToSave.arrayCubes[i].localScale;
        }
        mySave.infos.cubes = cubes;*/


        // Time to creat our XML! 
        _data = SerializeObject(mySave);
        // This is the final resulting XML from the serialization process 
        CreateXML();
    }

    /* The following metods came from the referenced URL */
    string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }

    byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    // Here we serialize our Save object of mySave 
    private string SerializeObject(object pObject)
    {
        string XmlizedString = null;
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(typeof(Save));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }

    // Here we deserialize it back into its original form 
    private object DeserializeObject(string pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(Save));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }

    // Finally our save and load methods for the file itself 
    private void CreateXML()
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(fileLocation + "\\" + fileName);
        if (t.Exists)
            t.Delete();
        writer = t.CreateText();
        writer.Write(Emcryption.Encrypt(_data));
        writer.Close();
        Debug.Log("File written.");
    }

    private void LoadXML()
    {
		//Debug.Log ("fileLocation" + "\\" + "fileName : " + fileLocation + "\\" + fileName);
		#if UNITY_STANDALONE_OSX
		StreamReader r = File.OpenText(fileLocation + "/" + fileName);
		#else
		StreamReader r = File.OpenText(fileLocation + "\\" + fileName);
		#endif
        string _info = r.ReadToEnd();
        r.Close();
        _data = Emcryption.Decrypt(_info);
        Debug.Log("File Read");
    }

    private string CreateFileName()
    {
        DateTime date = DateTime.Now;
        int chapter = 15, room = 12;

        string month;
        if (date.Month < 10)
            month = "0" + date.Month.ToString();
        else month = date.Month.ToString();

        string day;
        if (date.Day < 10)
            day = "0" + date.Day.ToString();
        else day = date.Day.ToString();

        string hour;
        if (date.Hour < 10)
            hour = "0" + date.Hour.ToString();
        else hour = date.Hour.ToString();

        string minute;
        if (date.Minute < 10)
            minute = "0" + date.Minute.ToString();
        else minute = date.Minute.ToString();

        return (date.Year.ToString() + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + chapter.ToString() + "-" + room.ToString() + fileExtention);
    }
}

// Save is our custom class that holds our defined objects we want to store in XML format 
public class Save
{
    public Infos infos;

    public Save() {}

    // Anything we want to store in the XML file, we define it here
    public struct Infos
    {
        public int sceneID;
        public string sceneName;
        public SaveInfo saveInfo;
		public GameplayInfos gameplayInfos;
		public HumanInfos human;
        public BallInfos ball;
        public cube[] cubes;
		public platform[] platforms;
    }

    public struct SaveInfo
    {
        public int chapter, room;
        public DateTime date;
    }
     
    public struct HumanInfos
    {
		public int id, parentId;
        public string name;
        public TransformInfos transform;
    }

	public struct GameplayInfos
	{
		public int id;
		public bool haveIt, haveCall, haveControl;
	}

    public struct BallInfos
    {
		public int id, parentId;
		public float electricPower, size;
        public TransformInfos transform;
    }

    public struct cube // in lower case to keep the structur in the xml
    {
		public int id, parentId;
        public TransformCompleteInfos transform;
    }

	public struct platform // in lower case to keep the structur in the xml
	{
		public int id;
		public bool active;
		public TransformCompleteInfos transform;
	}

    public struct TransformInfos
    {
        public Vector3 position, rotation;
    }

    public struct TransformCompleteInfos
    {
        public Vector3 position, rotation, scale;
    }
}

public class SG_Collector : MonoBehaviour
{
	public static SG_Savable[] CollectSavable()
	{
		return FindObjectsOfType<SG_Savable> ();
	}

	/// Collects the savables sorted by id.
	public static SG_SavableObj[] CollectSavableObjSorted()
	{
		SG_Savable[] savables = FindObjectsOfType<SG_Savable>();
		savables.OrderBy(x => x.id).ToList();
		SG_SavableObj[] savableObjs = new SG_SavableObj[savables.Length];
		for (int i = 0; i < savables.Length; i++)
		{
			savables[i].SetClass ();
			savableObjs[i] = savables[i].savable;
		}
		return savableObjs;
	}

	public static SG_SavableObj[] CollectSavableObj()
	{
		SG_Savable[] savables = CollectSavable();
		SG_SavableObj[] savableObjs = new SG_SavableObj[savables.Length];
		for (int i = 0; i < savables.Length; i++)
		{
			savables[i].SetClass ();
			savableObjs[i] = savables[i].savable;
		}
		return savableObjs;
	}

	public static Save.Infos SortObjToSave()
	{
		Save.Infos infos = new Save.Infos();
		List<Save.cube> listCubes = new List<Save.cube>();
		List<Save.platform> listPlatform = new List<Save.platform>();

		SG_SavableObj[] savableObjs = CollectSavableObj();

		foreach (SG_SavableObj save in savableObjs)
		{
			if (save.savableObjType == SG_Savable.SavableObjType.cube)
				listCubes.Add (((SG_Cube)save).data);
			else if (save.savableObjType == SG_Savable.SavableObjType.platform)
				listPlatform.Add (((SG_Platform)save).data);
			else if (save.savableObjType == SG_Savable.SavableObjType.human)
				infos.human = ((SG_Human)save).data;
			else if (save.savableObjType == SG_Savable.SavableObjType.controllableball)
				infos.ball = ((SG_ControllableBall)save).data;
			else if (save.savableObjType == SG_Savable.SavableObjType.gameplay)
				infos.gameplayInfos = ((SG_Gameplay)save).data;
		}

		infos.cubes = listCubes.ToArray();
		infos.platforms = listPlatform.ToArray();

		return infos;
	}

    /*public static ToLoad SortObjToLoad()
    {
        ToLoad toLoad = new ToLoad();

        List<Transform> listCubes = new List<Transform>();
        List<Transform> listPlatform = new List<Transform>();

        SG_SavableObj[] savableObjs = CollectSavableObj();

        foreach (SG_SavableObj save in savableObjs)
        {
            if (save.savableObjType == SG_Savable.SavableObjType.cube)
                listCubes.Add(save.transform);
            else if (save.savableObjType == SG_Savable.SavableObjType.platform)
                listPlatform.Add(save.transform);
            else if (save.savableObjType == SG_Savable.SavableObjType.human)
                toLoad.human = save.transform;
            else if (save.savableObjType == SG_Savable.SavableObjType.controllableball)
                toLoad.ball = save.transform;
            else if (save.savableObjType == SG_Savable.SavableObjType.gameplay)
                toLoad.gameplay = save.transform.gameObject;
        }

        listPlatform.Sort();
        
        toLoad.listCubes = listCubes.ToArray();
        toLoad.listPlatform = listPlatform.ToArray();

        return toLoad;
    }

    public struct ToLoad
    {
        public Transform human, ball;
        public GameObject gameplay;

        public Transform[] listCubes, listPlatform;
    }*/
}

public class Emcryption
{
    private const string key = "12345678901234567890123456789012";
   
    public static string Encrypt(string toEncrypt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        // better lang support
        ICryptoTransform cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    public static string Decrypt(string toDecrypt)
    {
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        // better lang support
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
}

public class FilesManager
{
    public static FileInfo[] FindFiles()
    {
        DirectoryInfo dir = new DirectoryInfo(XmlManager.fileLocation);
        return dir.GetFiles("*" + XmlManager.fileExtention);
    }

    public static void DeleteFile(string filename)
    {
        File.Delete(XmlManager.fileLocation + filename);
    }
}