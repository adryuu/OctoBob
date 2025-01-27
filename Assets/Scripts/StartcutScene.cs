using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartcutScene : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _images;

    private int _currentImageIndex = 0;

    private void Start()
    {
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].SetActive(i == 0); // Activar las primeras 3 imágenes
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            NextCutscene();
        }
    }

    public void NextCutscene()
    {
        if (_currentImageIndex < _images.Length - 1)
        {
            _currentImageIndex++;
            _images[_currentImageIndex].SetActive(true);

            if (_currentImageIndex >= 3)
            {
                _images[_currentImageIndex - 3].SetActive(false);
            }

            if (_currentImageIndex == 3)
            {
                _images[0].SetActive(false);
                _images[1].SetActive(false);
                _images[2].SetActive(false);
            }
        }
        else
        {
            GoNextScene();
        }
    }

    public void GoNextScene()
    {
        SceneManager.LoadScene("Muelle");
    }
}

