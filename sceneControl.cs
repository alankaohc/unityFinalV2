using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneControl : MonoBehaviour
{
    // Start is called before the first frame update
    public void quitGame()
    {
        Application.Quit();
    }
    public void switchSceneHome()
    {
        SceneManager.LoadScene(0);
    }
    public void switchGameMeun()
    {
        SceneManager.LoadScene(1);
    }
    public void switchSceneGame1()
    {
        SceneManager.LoadScene(2);
    }
    public void switchSceneGame2()
    {
        SceneManager.LoadScene(3);
    }
    public void switchSceneGame3()
    {
        SceneManager.LoadScene(4);
    }

}
