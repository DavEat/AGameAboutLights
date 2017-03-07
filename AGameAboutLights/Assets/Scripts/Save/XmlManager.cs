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

public class XmlManager
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

    public XmlManager()
    {
        //DontDestroyOnLoad(gameObject);
        fileLocation = Application.dataPath + "/Save/";
        fileExtention = ".HeiwaSave";
        //fileName = "2017-01-31-01-16-15-12" + fileExtention;
        mySave = new Save();
    }

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

    /// <summary>Load data from the file name in : "fileName"</summary>
    public Save Load()
    {
        LoadXML();
        if (_data.ToString() != "")
        {
            // notice how I use a reference to type (UserData) here, you need this 
            // so that the returned object is converted into the correct type 
            mySave = (Save)DeserializeObject(_data);

            return mySave;
        } else return null;
    }

    /// <summary>Save data in the file name in : "fileName"</summary>
    public void Save()
    {
        fileName = CreateFileName();

		mySave.infos = SaveSceneManager.SortObjToSave();
		mySave.infos.sceneName = SceneManager.GetActiveScene().name;

        //Save save info
        mySave.infos.saveInfos.date = new DateTime();
        mySave.infos.saveInfos.difficulty = -1;

        // Time to creat XML! 
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
        //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
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
        writer.Write(/*Emcryption.Encrypt*/(_data));
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
        _data = /*Emcryption.Decrypt*/(_info);
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

