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
    //Imagen de la barra de salud
    public Animator healthBar;
    //Numero de salud actual
    private int healthcount;
    //Numero de vida actual
    int life;

    private float _actualHealth;
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
        healthBar.SetInteger("Health", PlayerPrefs.GetInt("Salud"));
        _actualHealth = PlayerPrefs.GetInt("Salud");
        life = PlayerPrefs.GetInt("Vidas");
        lifeText.text = life.ToString();
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
