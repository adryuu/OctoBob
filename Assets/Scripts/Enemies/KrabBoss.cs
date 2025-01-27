using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Movimiento Aleatorio")]
    [SerializeField]
    private float _movementSpeed = 2f;
    //Tiempo sin atacar
    [SerializeField]
    private float _idleTime = 2f;
    [SerializeField]
    private float _movementRange = 5f;
    private Vector3 _spawnPosition;
    private bool _isMoving = false;
    private bool _canMove = true;
    //Dirección de movimiento incial
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
    [Header("Activación de Colliders")]
    //Índice para activar los colliders en fase 1
    [SerializeField]
    private int _phase1ColliderIndex;
    //Índice para activar los colliders en fase 2
    [SerializeField]
    private int _phase2ColliderIndex;
    //Índice para activar los colliders en fase 3
    [SerializeField]
    private int _phase3ColliderIndex; 


    private int _currentPhase = 1;
    private bool _isAttacking = false;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _spawnPosition = transform.position;

        // Desactivar colliders de las pinzas al inicio
        _leftPincherCollider.enabled = false;
        _rightPincherCollider.enabled = false;

        InvokeRepeating(nameof(ExecuteAttack), _attackInterval, _attackInterval);
    }

    private void Update()
    {
        HandlePhases();

        // Movimiento aleatorio si no está atacando y puede moverse
        if (!_isAttacking && _canMove)
        {
            if (!_isMoving) // Asegurarse de no iniciar un nuevo movimiento si ya está en movimiento
            {
                RandomMovement();
            }
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
        _isMoving = true;

        // Determinar la dirección y calcular el nuevo objetivo
        float targetX = _movingRight
            ? transform.position.x + Random.Range(1f, _movementRange) // Moverse hacia la derecha
            : transform.position.x - Random.Range(1f, _movementRange); // Moverse hacia la izquierda

        // Asegurar que el objetivo esté dentro del rango permitido
        targetX = Mathf.Clamp(targetX, _spawnPosition.x - _movementRange, _spawnPosition.x + _movementRange);

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        // Iniciar el movimiento hacia el objetivo
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        // Movimiento hacia el objetivo
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Cambiar dirección si alcanza un límite
        if (Mathf.Abs(transform.position.x - (_spawnPosition.x + _movementRange)) < 0.1f)
        {
            _movingRight = false; // Cambiar a moverse hacia la izquierda
        }
        else if (Mathf.Abs(transform.position.x - (_spawnPosition.x - _movementRange)) < 0.1f)
        {
            _movingRight = true; // Cambiar a moverse hacia la derecha
        }

        // Pausar antes de moverse nuevamente
        yield return new WaitForSeconds(_idleTime);

        // Habilitar movimiento nuevamente
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
        GameObject[] leftPincherPath = _currentPhase == 1 ? _attackPhase1LeftPincher :
                                       _currentPhase == 2 ? _attackPhase2LeftPincher : _attackPhase3LeftPincher;

        GameObject[] rightPincherPath = _currentPhase == 1 ? _attackPhase1RightPincher :
                                        _currentPhase == 2 ? _attackPhase2RightPincher : _attackPhase3RightPincher;

        // Activar colliders de las pinzas
        _leftPincherCollider.enabled = true;
        _rightPincherCollider.enabled = true;

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

            // Activar colliders en el índice correcto
            if (i == colliderActivationIndex)
            {
                _leftPincherCollider.enabled = true;
                _rightPincherCollider.enabled = true;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Desactivar los colliders después de finalizar el ataque
        _leftPincherCollider.enabled = false;
        _rightPincherCollider.enabled = false;

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