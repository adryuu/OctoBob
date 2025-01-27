using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyLifeManager : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public SpriteRenderer _sR;
    private Color _savedColor;

    void Start()
    {
        _savedColor = this.gameObject.GetComponent<SpriteRenderer>().color;
        if (this.gameObject.name == "FlyingEnemy")
        {
            maxHealth = 1;
            Debug.Log("El enemigo es volador");
            currentHealth = maxHealth;
        }
        else if (this.gameObject.name == "UpDownEnemy") 
        {
            maxHealth = 1;
            Debug.Log("El enemigo es arriba y abajo");
            currentHealth = maxHealth;
        }
        else if (this.gameObject.name == "MoveStraight")
        {
            maxHealth = 2;
            Debug.Log("El enemigo es rata");
            currentHealth = maxHealth;
        }

        
    }
    public void LoseEnemyLife()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        this.gameObject.GetComponent<SpriteRenderer>().color = _savedColor;
        currentHealth--;
        Debug.Log("Se ha restado vida");
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy is death");
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bubble"))
        {
            Debug.Log("Ha habido choque");
            LoseEnemyLife();
        }
    }
}
