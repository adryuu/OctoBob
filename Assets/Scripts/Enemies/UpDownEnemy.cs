using UnityEngine;
using System.Collections;

public class UpDownEnemy : MonoBehaviour
{
    [SerializeField]
    private Transform _pointA; // Punto A
    [SerializeField]
    private Transform _pointB; // Punto B
    [SerializeField]
    private float _moveSpeed = 2f; // Velocidad de movimiento
    [SerializeField]
    private float _waitTimeAtA = 1f; // Tiempo de espera en A
    [SerializeField]
    private float _waitTimeAtB = 1f; // Tiempo de espera en B

    private bool _movingUp = true; // Dirección inicial
    private bool _isWaiting = false; // Estado de espera

    private void Update()
    {
        if (_isWaiting) return;

        if (_movingUp)
        {
            MoveTowardsPoint(_pointB.position, _waitTimeAtB);
        }
        else
        {
            MoveTowardsPoint(_pointA.position, _waitTimeAtA);
        }
    }

    private void MoveTowardsPoint(Vector3 target, float waitTime)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) <= 0.1f)
        {
            _movingUp = !_movingUp; // Cambiar dirección
            StartCoroutine(WaitAtPoint(waitTime));
        }
    }

    private IEnumerator WaitAtPoint(float waitTime)
    {
        _isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        _isWaiting = false;
    }
}
