using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControler : MonoBehaviour
{
    //public int sceneIndex;
    public string sceneName;
    public void Start()
    {
        sceneName = "MainMenu";
    }
    public void ChangeScene(string name)
    {
        sceneName = name;
        //sceneIndex = po;
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
        //StartCoroutine(LoadLevel());
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    /*IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(sceneIndex);
        transitionAnim.SetTrigger("Start");
    }*/
}
