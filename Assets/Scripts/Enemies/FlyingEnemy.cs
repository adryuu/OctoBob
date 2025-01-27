using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField]
    private Transform[] _waypoints; // Array de puntos de patrulla
    [SerializeField]
    private float _patrolSpeed = 2f; // Velocidad de patrulla

    [Header("Persecuci�n")]
    [SerializeField]
    private float _chaseSpeed = 4f; // Velocidad de persecuci�n
    [SerializeField]
    private Transform _player; // Referencia al jugador

    private int _currentWaypointIndex = 0; // �ndice actual del waypoint
    private bool _isChasing = false; // Estado de persecuci�n

    private void Update()
    {
        if (_isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (_waypoints.Length == 0) return;

        // Mover hacia el waypoint actual
        Transform targetWaypoint = _waypoints[_currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, _patrolSpeed * Time.deltaTime);

        // Si llega al waypoint, pasar al siguiente
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        if (_player == null) return;

        // Moverse hacia el jugador
        Vector3 direction = (_player.position - transform.position).normalized;
        transform.position += direction * _chaseSpeed * Time.deltaTime;
    }

    // M�todos p�blicos para que DetectionArea interact�e con este script
    public void StartChasing()
    {
        _isChasing = true;
    }

    public void StopChasing()
    {
        _isChasing = false;
    }
}
