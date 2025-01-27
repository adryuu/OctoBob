using UnityEngine;

public class Goomba : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private Transform _pointA; // Punto A
    [SerializeField]
    private Transform _pointB; // Punto B
    [SerializeField]
    private float _moveSpeed = 2f; // Velocidad de movimiento

    private Transform _currentTarget; // Objetivo actual
    private bool _facingRight = true; // Dirección inicial del sprite

    private void Start()
    {
        // Comenzar moviéndose hacia el punto A
        _currentTarget = _pointA;
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        // Moverse hacia el objetivo actual
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget.position, _moveSpeed * Time.deltaTime);

        // Verificar si hemos llegado al objetivo
        if (Vector3.Distance(transform.position, _currentTarget.position) <= 0.1f)
        {
            // Cambiar el objetivo al otro punto
            _currentTarget = _currentTarget == _pointA ? _pointB : _pointA;
            Flip();
        }
    }

    private void Flip()
    {
        // Voltear el sprite cambiando la escala en X
        _facingRight = !_facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        // Dibujar líneas entre los puntos A y B para visualización
        if (_pointA != null && _pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_pointA.position, _pointB.position);
        }
    }
}
