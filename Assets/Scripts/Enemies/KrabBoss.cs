using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GiantCrabBoss : MonoBehaviour
{
    [SerializeField]
    private SceneControler sceneManager;

    [Header("Vida y Fases")]
    [SerializeField]
    private float _maxHealth = 20f;
    [SerializeField]
    private float _tier1PassHealth = 5f;
    [SerializeField]
    private float _tier2PassHealth = 10f;
    [SerializeField]
    private float _currentHealth;
    [SerializeField]
    private Image _healtbar;

    [Header("Movimiento Aleatorio")]
    [SerializeField]
    private float _movementSpeed = 2f;
    [SerializeField]
    private float _idleTime = 2f;
    [SerializeField]
    private float _movementRange = 5f;
    private Vector3 _spawnPosition;
    private bool _isMoving = false;
    private bool _canMove = false; // Inicialmente en false
    private bool _movingRight = true;

    [Header("Ataques")]
    [SerializeField]
    private GameObject[] _attackPhase1LeftPincher;
    [SerializeField]
    private GameObject[] _attackPhase1RightPincher;
    [SerializeField]
    private GameObject[] _attackPhase2LeftPincher;
    [SerializeField]
    private GameObject[] _attackPhase2RightPincher;
    [SerializeField]
    private GameObject[] _attackPhase3LeftPincher;
    [SerializeField]
    private GameObject[] _attackPhase3RightPincher;
    [SerializeField]
    private Transform _leftPincher;
    [SerializeField]
    private Transform _rightPincher;
    [SerializeField]
    private Collider2D _leftPincherCollider;
    [SerializeField]
    private Collider2D _rightPincherCollider;
    [SerializeField]
    private float _attackInterval = 5f;
    [SerializeField]
    private float _pincherMoveSpeed = 3f;

    [Header("Activaci�n de Colliders")]
    [SerializeField]
    private int _phase1ColliderIndex;
    [SerializeField]
    private int _phase2ColliderIndex;
    [SerializeField]
    private int _phase3ColliderIndex;

    [Header("Activaci�n del Jefe")]
    [SerializeField]
    private GameObject _musicManager;
    [SerializeField]
    private AudioClip _bossMusicName;
    [SerializeField]
    private GameObject _extraObject;
    [SerializeField]
    private CameraManager _cameraManager;
    [SerializeField]
    private Transform _bossCameraPoint;
    [SerializeField]
    private float _cameraLerpSpeed = 0.1f;

    private int _currentPhase = 1;
    private bool _isAttacking = false;
    private bool _isBossActive = false;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _spawnPosition = transform.position;

        _leftPincherCollider.enabled = false;
        _rightPincherCollider.enabled = false;

        InvokeRepeating(nameof(ExecuteAttack), _attackInterval, _attackInterval);

        if (_extraObject != null)
        {
            _extraObject.SetActive(false);
        }
    }

    private void Update()
    {
        _healtbar.fillAmount = _currentHealth / _maxHealth;

        if (!_isBossActive)
        {
            ActivateBoss();
        }

        HandlePhases();

        if (!_isAttacking && _canMove)
        {
            if (!_isMoving)
            {
                RandomMovement();
            }
        }
    }

    private void ActivateBoss()
    {
        _isBossActive = true;

        if (_musicManager != null)
        {
            AudioSource audioManager = _musicManager.GetComponent<AudioSource>();
            audioManager.clip = _bossMusicName;
            audioManager.Play();
        }

        if (_extraObject != null)
        {
            _extraObject.SetActive(true);
        }

        if (_cameraManager != null && _bossCameraPoint != null)
        {
            _cameraManager.SetStaticPoint(_bossCameraPoint.position, _cameraLerpSpeed, 5.548842f);
        }

        Debug.Log("El jefe se ha activado. M�sica cambiada, objeto extra activado y c�mara ajustada.");
    }

    private void HandlePhases()
    {
        if (_currentHealth <= _tier2PassHealth && _currentPhase == 1)
        {
            _currentPhase = 2;
            Debug.Log("Cambio a la fase 2.");
        }
        else if (_currentHealth <= _tier1PassHealth && _currentPhase == 2)
        {
            _currentPhase = 3;
            Debug.Log("Cambio a la fase 3.");
        }
    }

    private void RandomMovement()
    {
        Debug.Log("Intentando mover al cangrejo...");
        _isMoving = true;

        float targetX = _movingRight
            ? transform.position.x + Random.Range(1f, _movementRange)
            : transform.position.x - Random.Range(1f, _movementRange);

        targetX = Mathf.Clamp(targetX, _spawnPosition.x - _movementRange, _spawnPosition.x + _movementRange);

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Debug.Log($"Movi�ndose hacia {targetPosition}");

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log($"Cangrejo lleg� a {transform.position}");

        if (Mathf.Abs(transform.position.x - (_spawnPosition.x + _movementRange)) < 0.1f)
        {
            _movingRight = false;
        }
        else if (Mathf.Abs(transform.position.x - (_spawnPosition.x - _movementRange)) < 0.1f)
        {
            _movingRight = true;
        }

        yield return new WaitForSeconds(_idleTime);
        _isMoving = false;
    }

    private void ExecuteAttack()
    {
        if (_isAttacking) return;

        _isAttacking = true;

        _canMove = false;
        Invoke(nameof(StartAttack), 0.5f);
    }

    private void StartAttack()
    {
        GameObject[] leftPincherPath = _currentPhase == 1 ? _attackPhase1LeftPincher :
                                       _currentPhase == 2 ? _attackPhase2LeftPincher : _attackPhase3LeftPincher;

        GameObject[] rightPincherPath = _currentPhase == 1 ? _attackPhase1RightPincher :
                                        _currentPhase == 2 ? _attackPhase2RightPincher : _attackPhase3RightPincher;

        StartCoroutine(MovePinchersToAttack(leftPincherPath, rightPincherPath));
    }

    private IEnumerator MovePinchersToAttack(GameObject[] leftPath, GameObject[] rightPath)
    {
        if (leftPath.Length == 0 || rightPath.Length == 0) yield break;

        int colliderActivationIndex = _currentPhase == 1 ? _phase1ColliderIndex :
                                      _currentPhase == 2 ? _phase2ColliderIndex : _phase3ColliderIndex;

        Vector3 leftTarget = leftPath[0].transform.position;
        Vector3 rightTarget = rightPath[0].transform.position;

        for (int i = 0; i < leftPath.Length && i < rightPath.Length; i++)
        {
            leftTarget = leftPath[i].transform.position;
            rightTarget = rightPath[i].transform.position;

            while (Vector3.Distance(_leftPincher.position, leftTarget) > 0.1f || Vector3.Distance(_rightPincher.position, rightTarget) > 0.1f)
            {
                _leftPincher.position = Vector3.MoveTowards(_leftPincher.position, leftTarget, _pincherMoveSpeed * Time.deltaTime);
                _rightPincher.position = Vector3.MoveTowards(_rightPincher.position, rightTarget, _pincherMoveSpeed * Time.deltaTime);

                yield return null;
            }

            if (i == colliderActivationIndex)
            {
                _leftPincherCollider.enabled = true;
                _rightPincherCollider.enabled = true;
            }
        }

        yield return new WaitForSeconds(0.5f);

        _leftPincherCollider.enabled = false;
        _rightPincherCollider.enabled = false;

        _isAttacking = false;
        _canMove = true;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            StopAllCoroutines();
            _currentHealth = 0;
            Debug.Log("El cangrejo ha muerto.");
            Destroy(this.gameObject, 3f);
            sceneManager.ChangeScene("EndCutScene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bubble"))
        {
            TakeDamage(1);
        }

        if (collision.CompareTag("Player") && !_canMove) // Solo activa _canMove la primera vez
        {
            _canMove = true;
            Debug.Log("El cangrejo ahora puede moverse permanentemente.");
        }
    }
}
