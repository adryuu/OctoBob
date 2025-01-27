using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Referencia al jugador.
    [SerializeField]
    private Transform _player;

    // Velocidad de suavizado al seguir al jugador.
    [SerializeField]
    private float _smoothSpeed = 0.125f;

    // Desplazamiento de la c�mara respecto al jugador en el eje X.
    [SerializeField]
    private float _xOffset;

    // Desplazamiento de la c�mara respecto al jugador en el eje Y.
    [SerializeField]
    private float _yOffset;

    // Array de puntos de c�mara est�tica.
    [SerializeField]
    private GameObject[] _staticPoints;

    // Punto de c�mara est�tica actual (si existe).
    private GameObject _currentStaticPoint = null;

    private void LateUpdate()
    {
        // Si hay un punto est�tico activo, la c�mara queda fija ah�.
        if (_currentStaticPoint != null)
        {
            Vector3 staticPosition = _currentStaticPoint.transform.position;
            transform.position = new Vector3(staticPosition.x + _xOffset, staticPosition.y + _yOffset, transform.position.z);
        }
        else
        {
            // Si no hay punto est�tico, la c�mara sigue al jugador.
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Posici�n deseada: posici�n del jugador m�s el desplazamiento en X.
        float desiredX = _player.position.x + _xOffset;

        // Suavizamos el movimiento de la c�mara hacia la posici�n deseada en X.
        float smoothedX = Mathf.Lerp(transform.position.x, desiredX, _smoothSpeed);

        // Posici�n deseada: posici�n del jugador m�s el desplazamiento en Y.
        float desiredY = _player.position.y + _yOffset;

        // Suavizamos el movimiento de la c�mara hacia la posici�n deseada en Y.
        float smoothedY = Mathf.Lerp(transform.position.y, desiredY, _smoothSpeed);

        // Actualizamos la posici�n de la c�mara.
        transform.position = new Vector3(smoothedX, smoothedY, transform.position.z);
    }

    public void CheckStaticPoints()
    {
        // Recorremos todos los puntos est�ticos.
        foreach (GameObject point in _staticPoints)
        {
            // Saltar si el punto ya fue destruido.
            if (point == null) continue;

            // Si el jugador est� dentro del �rea del punto est�tico.
            float distance = Mathf.Abs(_player.position.x - point.transform.position.x);
            if (distance < 1f) // Cambia el rango seg�n lo necesario.
            {
                _currentStaticPoint = point;
                return; // Detenemos la b�squeda una vez encontramos un punto v�lido.
            }
        }

        // Si ning�n punto es v�lido, la c�mara vuelve al modo de seguimiento.
        _currentStaticPoint = null;
    }

    public void RemoveStaticPoint(GameObject point)
    {
        // Destruir el punto y actualizar el array.
        for (int i = 0; i < _staticPoints.Length; i++)
        {
            if (_staticPoints[i] == point)
            {
                _staticPoints[i] = null;
                Destroy(point);
                break;
            }
        }

        // Verificar si todav�a hay puntos est�ticos activos.
        CheckStaticPoints();
    }
}
