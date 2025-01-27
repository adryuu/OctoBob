using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip _damageClip;
    [SerializeField]
    private AudioClip _deathClip;
    [SerializeField]
    private AudioClip _jumpClip;
    private AudioSource _audioSource;

    [Header("Fuerzas")]
    [SerializeField]
    private float _maxRecoilForce = 6f; // Fuerza máxima de impulso
    [SerializeField]
    private float _shootForce = 5f; // Fuerza constante del proyectil
    [SerializeField]
    private Vector2 _shootDirection;
    private Rigidbody2D _rb;

    [Header("Variables de Disparo")]
    [SerializeField]
    private GameObject _proyectilePrefab;
    [SerializeField]
    private Transform _shootPoint;
    [SerializeField]
    private float _cadence;

    [Header("Detección de Suelo")]
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private bool _isOnGround;
    [SerializeField]
    private float _groundCheckRadius = 0.2f;

    private bool _canShoot = true;

    [SerializeField, Range(0, 2)]
    private float _chargeTime = 0f; // Tiempo de carga del disparo (mostrado en Inspector)

    [SerializeField, Range(2, 6)]
    private float _currentRecoilForce = 1f; // Fuerza de retroceso actual (mostrada en Inspector)

    private Animator _anim;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckGround();
        HandleInput();
    }
    public void PlayDeathSound()
    {
        _audioSource.PlayOneShot(_deathClip);
    }
    public void PlayDamageSound()
    {
        _audioSource.PlayOneShot(_damageClip);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            PlayDamageSound();
            this.gameObject.GetComponent<CheckpointManager>().TeleportToCheckPoint();
        }
    }
    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _shootDirection = new Vector2(Mathf.Round(horizontal), Mathf.Round(vertical)).normalized;

        // Iniciar carga al presionar el botón
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _chargeTime = 0f;
           
        }

        // Incrementar carga mientras el botón esté pulsado (máximo 2 segundos)
        if (Input.GetKey(KeyCode.Z))
        {
            _chargeTime += Time.deltaTime;
            _chargeTime = Mathf.Min(_chargeTime, 2f); // Limitar la carga a 2 segundos
            if(_chargeTime >= 0.8f)
            {
                _anim.SetBool("isCharging", true); // Activar animación de carga en bucle
            }
            // Calcular la fuerza actual de retroceso (entre 2 y _maxRecoilForce)
            _currentRecoilForce = Mathf.Lerp(2f, _maxRecoilForce, _chargeTime / 2f);
        }

        // Disparar al soltar el botón
        if (Input.GetKeyUp(KeyCode.Z))
        {
            _anim.SetBool("isCharging", false); // Detener animación de carga
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if (!_canShoot) yield break;
        _canShoot = false;

        // Usar la fuerza de retroceso calculada
        float recoilForce = _currentRecoilForce;

        if (_shootDirection == Vector2.zero)
        {
            _shootDirection = Vector2.right;
        }

        Vector2 recoilDirection = -_shootDirection.normalized;

        // Animaciones según la dirección del disparo
        if (_isOnGround && (_shootDirection == Vector2.down || (_shootDirection.x != 0 && _shootDirection.y < 0)))
        {
            _rb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);

            if (_shootDirection == Vector2.down)
            {
                _audioSource.PlayOneShot(_jumpClip);
                SetTrigger("isJumping");
            }
            else if (_shootDirection.x < 0 && _shootDirection.y < 0)
            {
                _audioSource.PlayOneShot(_jumpClip);
                SetTrigger("isJumpingToRight");
            }
            else if (_shootDirection.x > 0 && _shootDirection.y < 0)
            {
                _audioSource.PlayOneShot(_jumpClip);
                SetTrigger("isJumpingToLeft");
            }
        }
        else
        {
            if (_shootDirection.x < 0 && _shootDirection.y == 0)
            {
                SetTrigger("isShootingPatras");
            }
            else if (_shootDirection.x > 0 && _shootDirection.y == 0)
            {
                SetTrigger("isShootingPalante");
            }
            else if (_shootDirection.x < 0 && _shootDirection.y > 0)
            {
                SetTrigger("isShootingPatrasArriba");
            }
            else if (_shootDirection.x > 0 && _shootDirection.y > 0)
            {
                SetTrigger("isShootingPalanteArriba");
            }
            else if (_shootDirection.x == 0 && _shootDirection.y > 0)
            {
                SetTrigger("isShootingArriba");
            }
        }

        // Instanciar el proyectil
        GameObject projectile = Instantiate(_proyectilePrefab, _shootPoint.position, Quaternion.identity);
        projectile.GetComponent<Proyectile>().Direction = _shootDirection;
        projectile.GetComponent<Proyectile>().Speed = _shootForce;

        yield return new WaitForSeconds(_cadence);
        _canShoot = true;
    }

    private void CheckGround()
    {
        _isOnGround = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        _anim.SetBool("isOnGround", _isOnGround);
    }

    private void SetTrigger(string triggerName)
    {
        // Reiniciar triggers antes de activar uno nuevo
        _anim.ResetTrigger("isShootingPatras");
        _anim.ResetTrigger("isShootingPalante");
        _anim.ResetTrigger("isShootingPatrasArriba");
        _anim.ResetTrigger("isShootingPalanteArriba");
        _anim.ResetTrigger("isShootingArriba");

        _anim.SetTrigger(triggerName);
        Debug.Log($"Trigger activado: {triggerName}");
    }

    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }
}
