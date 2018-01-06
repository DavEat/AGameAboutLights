using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class XmlManager
{
	#region Var
    [HideInInspector]
    public string fileLocation, levelFolderName, editorFolderName, _fileExtention;
    private static string _fileName;
    private Save mySave;
    private string _data;
    #endregion

    public string fileName
    {
        get { return _fileName; }
        set { _fileName = value; }
    }
    public string fileExtention
    {
        get { return _fileExtention; }
        set { _fileExtention = value; }
    }

    /// <summary>Constructor</summary>
    public XmlManager()
    {
        #if UNITY_WEBGL
        fileLocation = Application.streamingAssetsPath + "/Save/";
        #else
        fileLocation = Application.dataPath + "/Save/";
        #endif
        levelFolderName = "/OfficialLevels/";
        editorFolderName = "/CustomLevel/";
        fileExtention = ".HeiwaSave";
        mySave = new Save();
    }

    /// <summary>Check if the save exist</summary>
    private void SaveExist(string _folder)
    {
        _data = "";
        // Load our Save into mySave 
        LoadXML(_folder);
        if (_data.ToString() != "")
            // notice how I use a reference to type (Save) here, you need this 
            // so that the returned object is converted into the correct type 
            mySave = (Save)DeserializeObject(_data);
    }

    /// <summary>Load data from the file name in : "fileName"</summary>
    public Save Load(string _folder, string _fileName)
    {
        fileName = _fileName;
        LoadXML(_folder);
        if (_data.ToString() != "")
        {
            // notice how I use a reference to type (UserData) here, you need this 
            // so that the returned object is converted into the correct type 
            mySave = (Save)DeserializeObject(_data);

            return mySave;
        } else return null;
    }

    /// <summary>Save data in the file name in : "fileName"</summary>
    public void Save(string _folder, string _fileName)
    {
        fileName = _fileName;
        //fileName = SaveSceneManager.inst.CreateFileName();

        mySave.infos = SaveSceneManager.SortObjToSave();
		mySave.infos.sceneName = SceneManager.GetActiveScene().name;
        mySave.infos.saveInfos.developperLevel = AG_SelectLevelManager.inst.developper;

        //Save save info
        mySave.infos.saveInfos.date = new DateTime();
        mySave.infos.saveInfos.difficulty = -1;

        // Time to creat XML! 
        _data = SerializeObject(mySave);
        // This is the final resulting XML from the serialization process 
        CreateXML(_folder);
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

    /// <summary>Here we serialize our Save object of mySave</summary>
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

    /// <summary>Here we deserialize it back into its original form </summary>
    private object DeserializeObject(string pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(Save));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }

    /// <summary>Finally our save and load methods for the file itself </summary>
    private void CreateXML(string _folder)
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(fileLocation  + _folder  + "\\" + fileName);
        if (t.Exists)
            t.Delete();
        writer = t.CreateText();
        writer.Write(/*Emcryption.Encrypt*/(_data));
        writer.Close();
        Debug.Log("File written.");
    }

    private void LoadXML(string _folder)
    {
        string _info;
        #if UNITY_WEBGL
        _info = AG_FileManagerOnServer.inst.GetData("http://davidmestdagh.com/games/AGAL/StreamingAssets" + _folder + "/" + fileName);
        #elif UNITY_STANDALONE_OSX
        StreamReader r = File.OpenText(fileLocation + _folder + "/" + fileName);
        _info = r.ReadToEnd();
        r.Close();
        #else
        StreamReader r = File.OpenText(fileLocation + _folder + "\\" + fileName);
        _info = r.ReadToEnd();
        r.Close();
        #endif

        _data = /*Emcryption.Decrypt*/(_info);
        //Debug.Log("File Read");
    }
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
    public static FileInfo[] FindFiles(string _folder)
    {
        DirectoryInfo dir = new DirectoryInfo(AG_SelectLevelManager.inst.xml.fileLocation + _folder);
        return dir.GetFiles("*" + AG_SelectLevelManager.inst.xml.fileExtention);
    }

    public static void DeleteFile(string _folder, string _filename)
    {
        File.Delete(AG_SelectLevelManager.inst.xml.fileLocation + _folder + _filename);
    }
}

