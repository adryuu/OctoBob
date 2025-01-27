using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControler : MonoBehaviour
{
    //public int sceneIndex;
    public string sceneName;
    [SerializeField]
    private Button _button;
    public void Start()
    {
        sceneName = "MainMenu";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _button.onClick.Invoke();
        }

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
