using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    [SerializeField]
    private FlyingEnemy _enemy; // Referencia al enemigo volador

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Detectar si el jugador entra
        {
            _enemy.StartChasing();
            Debug.Log("Jugador detectado en el área de activación.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Detectar si el jugador sale
        {
            _enemy.StopChasing();
            Debug.Log("Jugador salió del área de activación.");
        }
    }
}
