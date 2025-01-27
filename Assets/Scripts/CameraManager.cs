using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Referencia al jugador.
    [SerializeField]
    private Transform _player;

    // Velocidad de suavizado al seguir al jugador.
    [SerializeField]
    private float _smoothSpeed = 0.125f;

    // Desplazamiento de la cámara respecto al jugador en el eje X.
    [SerializeField]
    private float _xOffset;

    // Desplazamiento de la cámara respecto al jugador en el eje Y.
    [SerializeField]
    private float _yOffset;

    // Array de puntos de cámara estática.
    [SerializeField]
    private GameObject[] _staticPoints;

    // Punto de cámara estática actual (si existe).
    private GameObject _currentStaticPoint = null;

    // Indica si la cámara está en un punto estático
    private bool _isStatic = false;

    // Posición estática de la cámara y velocidad de suavizado
    private Vector3 _staticPointPosition;
    private float _staticLerpSpeed;

    private void LateUpdate()
    {
        // Si hay un punto estático activo, mover la cámara hacia ese punto.
        if (_isStatic)
        {
            MoveToStaticPoint();
        }
        else if (_currentStaticPoint != null)
        {
            // Si hay un punto estático activo (del array), la cámara queda fija ahí.
            Vector3 staticPosition = _currentStaticPoint.transform.position;
            transform.position = new Vector3(staticPosition.x + _xOffset, staticPosition.y + _yOffset, transform.position.z);
        }
        else
        {
            // Si no hay punto estático, la cámara sigue al jugador.
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Posición deseada: posición del jugador más el desplazamiento en X y Y.
        float desiredX = _player.position.x + _xOffset;
        float desiredY = _player.position.y + _yOffset;

        // Suavizamos el movimiento de la cámara hacia la posición deseada.
        float smoothedX = Mathf.Lerp(transform.position.x, desiredX, _smoothSpeed);
        float smoothedY = Mathf.Lerp(transform.position.y, desiredY, _smoothSpeed);

        // Actualizamos la posición de la cámara.
        transform.position = new Vector3(smoothedX, smoothedY, transform.position.z);
    }

    private void MoveToStaticPoint()
    {
        // Suaviza la posición de la cámara hacia el punto estático definido.
        transform.position = Vector3.Lerp(transform.position, _staticPointPosition, _staticLerpSpeed * Time.deltaTime);
    }

    public void SetStaticPoint(Vector3 position, float lerpSpeed)
    {
        // Configura la cámara para moverse hacia un punto fijo con suavizado.
        _isStatic = true;
        _staticPointPosition = position;
        _staticLerpSpeed = lerpSpeed;
    }

    public void ReleaseStaticPoint()
    {
        // Libera la cámara del punto estático y vuelve al seguimiento del jugador.
        _isStatic = false;
        _currentStaticPoint = null;
    }

    public void CheckStaticPoints()
    {
        // Recorremos todos los puntos estáticos.
        foreach (GameObject point in _staticPoints)
        {
            // Saltar si el punto ya fue destruido.
            if (point == null) continue;

            // Si el jugador está dentro del área del punto estático.
            float distance = Mathf.Abs(_player.position.x - point.transform.position.x);
            if (distance < 1f) // Cambia el rango según lo necesario.
            {
                _currentStaticPoint = point;
                return; // Detenemos la búsqueda una vez encontramos un punto válido.
            }
        }

        // Si ningún punto es válido, la cámara vuelve al modo de seguimiento.
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

        // Verificar si todavía hay puntos estáticos activos.
        CheckStaticPoints();
    }
}
