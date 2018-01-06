using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public class AG_Singleton<Instance> : MonoBehaviour where Instance : AG_Singleton<Instance>
{
    public static string[] methodHooked { get; private set; }
    public static Instance inst;
    //public bool isPersistant;

    public virtual void Awake()
    {
        //if (isPersistant)
        //{
        //    if (!inst)
        //        inst = this as Instance;
        //    else
        //        DestroyObject(gameObject);
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        {
            inst = this as Instance;
        }
    }

    public void SetMethodHooked(string[] methods)
    {
        methodHooked = methods;
    }
}
