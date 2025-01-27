using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    //Zona de Variables Globales
    [Header("Referencias")]
    public float Speed = 20f;
    public float LifeTime = 5f;
    public Vector2 Direction;

    [SerializeField]
    private float _timeToDestroy = 2f;

    private Animator _anim;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = Direction * Speed;
        _anim = GetComponent<Animator>();
        Destroy(gameObject, LifeTime);
    }

    private void OnTriggerEnter2D(Collider2D infoCollision)
    {
        if (infoCollision.gameObject.CompareTag("Player"))
        {
            return;
        }
        if (infoCollision.gameObject.CompareTag("Enemy"))
        {
            _anim.SetTrigger("Impact");
            Destroy(this.gameObject, _timeToDestroy);
        }
        if (infoCollision.gameObject.CompareTag("Ground"))
        {
            _anim.SetTrigger("Impact");
            Destroy(this.gameObject, _timeToDestroy);
        }
    }
}
