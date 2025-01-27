using UnityEngine;
using UnityEngine.SceneManagement;

public class GiantCrabBoss : MonoBehaviour
{
    [SerializeField]
    private SceneControler sceneManager;
    [Header("Vida y Fases")]
    [SerializeField]
    private float _maxHealth = 20f;
    // Vida para pasar a la fase 1
    [SerializeField]
    private float _tier1PassHealth = 5f;
    // Vida para pasar a la fase 2
    [SerializeField]
    private float _tier2PassHealth = 10f;
    [SerializeField]
    private float _currentHealth;

    [Header("Movimiento Aleatorio")]
    // Velocidad de movimiento
    [SerializeField]
    private float _movementSpeed = 2f;
    // Tiempo de pausa antes de cambiar de dirección
    [SerializeField]
    private float _idleTime = 2f;
    // Rango máximo de movimiento en X desde el spawn
    [SerializeField]
    private float _movementRange = 5f; 
    private Vector3 _spawnPosition;
    private bool _isMoving = false;
    // Controla si puede moverse
    private bool _canMove = true; 

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
    // Tiempo entre ataques
    [SerializeField]
    private float _attackInterval = 5f;
    // Velocidad de movimiento de las pinzas
    [SerializeField]
    private float _pincherMoveSpeed = 3f;

    // Fase actual
    private int _currentPhase = 1; 
    private bool _isAttacking = false;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _spawnPosition = transform.position;
        InvokeRepeating(nameof(ExecuteAttack), _attackInterval, _attackInterval);
    }

    private void Update()
    {
        HandlePhases();

        if (!_isAttacking && _canMove)
        {
            RandomMovement();
        }
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
        if (!_isMoving)
        {
            _isMoving = true;

            // Generar un nuevo objetivo dentro del rango permitido
            float targetX = Mathf.Clamp(
                transform.position.x + Random.Range(-_movementRange, 
                                                    _movementRange),
                _spawnPosition.x - _movementRange,
                _spawnPosition.x + _movementRange
            );

            Vector3 targetPosition = new Vector3(targetX, transform.position.y,
                                                 transform.position.z);
            StartCoroutine(MoveToPosition(targetPosition));
        }
    }

    private System.Collections.IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                targetPosition, _movementSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(_idleTime);
        _isMoving = false;
    }

    private void ExecuteAttack()
    {
        if (_isAttacking) return;

        _isAttacking = true;

        // Detener el movimiento medio segundo antes del ataque
        _canMove = false;
        Invoke(nameof(StartAttack), 0.5f);
    }

    private void StartAttack()
    {
        // Seleccionar arrays de ataque según la fase actual
        GameObject[] leftPincherPath = _currentPhase == 1 ? 
                                        _attackPhase1LeftPincher :
                                       _currentPhase == 2 ? 
                                       _attackPhase2LeftPincher : 
                                       _attackPhase3LeftPincher;

        GameObject[] rightPincherPath = _currentPhase == 1 ? 
                                        _attackPhase1RightPincher :
                                        _currentPhase == 2 ? 
                                        _attackPhase2RightPincher : 
                                        _attackPhase3RightPincher;

        StartCoroutine(MovePinchersToAttack(leftPincherPath, rightPincherPath));
    }

    private System.Collections.IEnumerator MovePinchersToAttack(GameObject[] 
        leftPath, GameObject[] rightPath)
    {
        if (leftPath.Length == 0 || rightPath.Length == 0) yield break;

        // Mover pinzas al primer punto del ataque
        Vector3 leftTarget = leftPath[0].transform.position;
        Vector3 rightTarget = rightPath[0].transform.position;

        while (Vector3.Distance(_leftPincher.position, leftTarget) > 0.1f ||
            Vector3.Distance(_rightPincher.position, rightTarget) > 0.1f)
        {
            _leftPincher.position = Vector3.MoveTowards(_leftPincher.position,
                leftTarget, _pincherMoveSpeed * Time.deltaTime);
            _rightPincher.position = Vector3.MoveTowards(_rightPincher.position,
                rightTarget, _pincherMoveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f); 

        // Realizar el ataque animado
        for (int i = 1; i < leftPath.Length && i < rightPath.Length; i++)
        {
            leftTarget = leftPath[i].transform.position;
            rightTarget = rightPath[i].transform.position;

            while (Vector3.Distance(_leftPincher.position, leftTarget) > 0.1f ||
                Vector3.Distance(_rightPincher.position, rightTarget) > 0.1f)
            {
                _leftPincher.position = Vector3.MoveTowards(_leftPincher.position, 
                    leftTarget, _pincherMoveSpeed * Time.deltaTime);
                _rightPincher.position = Vector3.MoveTowards(_rightPincher.position,
                    rightTarget, _pincherMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Esperar medio segundo después de terminar el ataque
        // antes de permitir el movimiento
        yield return new WaitForSeconds(0.5f);

        _isAttacking = false;
        _canMove = true; // Permitir que el cangrejo se mueva de nuevo
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
    }
}
