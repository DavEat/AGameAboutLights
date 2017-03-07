using UnityEngine;

public class AG_Menu : MonoBehaviour
{
    public void ToogleScreen(GameObject _obj)
    {
        _obj.SetActive(!_obj.activeSelf);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
