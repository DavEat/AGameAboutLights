using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_EditorManager : MonoBehaviour {

    private GameObject _currentOpenMenu;

    public void ToogleObjectsMenu(GameObject menu)
    {
        if (_currentOpenMenu != null && _currentOpenMenu.activeSelf)
        {
            if (menu != _currentOpenMenu)
            {
                _currentOpenMenu.SetActive(false);
                _currentOpenMenu = menu;
                _currentOpenMenu.SetActive(true);
            }
            else _currentOpenMenu.SetActive(false);
        }
        else
        {
            if (menu != _currentOpenMenu)
                _currentOpenMenu = menu;
            _currentOpenMenu.SetActive(true);
        }

    }
}
