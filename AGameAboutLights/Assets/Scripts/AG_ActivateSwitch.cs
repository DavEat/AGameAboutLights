﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_ActivateSwitch : MonoBehaviour {

    private bool activated;

    private void Start()
    {
        activated = false;
    }

    public void activate()
    {
        if (activated == false)
        {
            GetComponent<Animator>().SetFloat("speed", 2);
            GetComponent<Animator>().Play("SwitchAnimation");
        }
        else
        {
            GetComponent<Animator>().SetFloat("speed", -2);
            GetComponent<Animator>().Play("SwitchAnimation");
        }
        activated = !activated;
    }
}
