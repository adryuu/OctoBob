using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private PauseMenu gameOverMenu;
    /*[SerializeField]
    SceneManager sceneManager;*/
    //Array de sprites de la barra de salud
    public Animator[] health;
    //Imagen de la barra de salud
    public Animator healthBar;
    //Numero de salud actual
    private int healthcount;
    //Numero de vida actual
    int life;

    [SerializeField]
    private GameObject _player;
    private Color _savedColor;

    [SerializeField]
    private CheckpointManager _checkpointManager;

    public TextMeshProUGUI lifeText;


    // Start is called before the first frame update
    void Awake()
    {
        Color recolor = _player.GetComponent<SpriteRenderer>().color;
        _savedColor = recolor;
        //Numero de vidas y salud iniciales
        PlayerPrefs.SetInt("Vidas", 3);
        PlayerPrefs.SetInt("Salud", 4);
    }

    private void Start()
    {
        //Numero de vidas y salud al iniciar la escena
        healthcount = PlayerPrefs.GetInt("Salud");
        life = PlayerPrefs.GetInt("Vidas");
        lifeText.text = life.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        ActualLife();
        //Sprite actual de la barra de salud
        health[PlayerPrefs.GetInt("Salud")].gameObject.SetActive(true);
        healthBar = health[PlayerPrefs.GetInt("Salud")];

        life = PlayerPrefs.GetInt("Vidas");
        lifeText.text = life.ToString();
    }

    private void ActualLife()
    {
        if (healthcount == 1)
        {
            healthBar = health[healthcount];
            health[0].gameObject.SetActive(false);
            health[1].gameObject.SetActive(false);
            health[2].gameObject.SetActive(false);
        }
        if (healthcount == 2)
        {
            healthBar = health[healthcount];
            health[0].gameObject.SetActive(false);
            health[1].gameObject.SetActive(false);
            health[3].gameObject.SetActive(false);
        }
        if (healthcount == 3)
        {
            healthBar = health[healthcount];
            health[0].gameObject.SetActive(false);
            health[2].gameObject.SetActive(false);
            health[3].gameObject.SetActive(false);
        }
        if (healthcount == 4)
        {
            healthBar = health[healthcount];
            health[1].gameObject.SetActive(false);
            health[2].gameObject.SetActive(false);
            health[3].gameObject.SetActive(false);
        }
    }

    //Método de perder salud
    public void LoseHealth()
    {
        _player.GetComponent<AudioSource>().Play();
        _player.GetComponent<SpriteRenderer>().color = Color.red;
        _player.GetComponent<SpriteRenderer>().color = _savedColor;
        //Salud  actual
        healthcount = PlayerPrefs.GetInt("Salud");
        //Perder salud con más de 1 punto de salud
        if (healthcount > 1)
        {
            
            healthcount--;
            PlayerPrefs.SetInt("Salud", healthcount);
            PlayerPrefs.Save();
            Debug.Log("Salud: " + healthcount);
        }
        //En caso de que tengas 1 punto de salud
        else
        {
            healthcount--;
            PlayerPrefs.SetInt("Salud", healthcount);
            PlayerPrefs.Save();
            LoseLife();
            //ResetScene();
        }
    }

    public void LoseLife()
    {

        life = PlayerPrefs.GetInt("Vidas");
        life--;
        PlayerPrefs.SetInt("Vidas", life);
        PlayerPrefs.Save();
        if (life > 0)
        {
            PlayerPrefs.SetInt("Salud", 4);
            PlayerPrefs.Save();
            _checkpointManager.TeleportToCheckPoint();
        }
        else
        {
            gameOverMenu.GameOver();
            ResetRun();
        }
    }

    public void ResetRun()
    {
        PlayerPrefs.SetInt("Vidas", 3);
        PlayerPrefs.SetInt("Salud", 4);
        PlayerPrefs.Save();
        
    }
}
