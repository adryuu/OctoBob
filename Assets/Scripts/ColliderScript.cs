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
            Debug.Log("Jugador detectado en el �rea de activaci�n.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Detectar si el jugador sale
        {
            _enemy.StopChasing();
            Debug.Log("Jugador sali� del �rea de activaci�n.");
        }
    }
}
