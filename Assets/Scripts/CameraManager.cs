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

    // Indica si la c�mara est� en un punto est�tico
    private bool _isStatic = false;

    // Posici�n est�tica de la c�mara y velocidad de suavizado
    private Vector3 _staticPointPosition;
    private float _staticLerpSpeed;

    // Variables para cambiar progresivamente el tama�o de la c�mara
    private float _targetOrthographicSize; // Tama�o final deseado
    private float _initialOrthographicSize; // Tama�o inicial
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _initialOrthographicSize = _camera.orthographicSize;
    }

    private void LateUpdate()
    {
        // Si hay un punto est�tico activo, mover la c�mara hacia ese punto.
        if (_isStatic)
        {
            MoveToStaticPoint();
        }
        else if (_currentStaticPoint != null)
        {
            // Si hay un punto est�tico activo (del array), la c�mara queda fija ah�.
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
        // Posici�n deseada: posici�n del jugador m�s el desplazamiento en X y Y.
        float desiredX = _player.position.x + _xOffset;
        float desiredY = _player.position.y + _yOffset;

        // Suavizamos el movimiento de la c�mara hacia la posici�n deseada.
        float smoothedX = Mathf.Lerp(transform.position.x, desiredX, _smoothSpeed);
        float smoothedY = Mathf.Lerp(transform.position.y, desiredY, _smoothSpeed);

        // Actualizamos la posici�n de la c�mara.
        transform.position = new Vector3(smoothedX, smoothedY, transform.position.z);
    }

    private void MoveToStaticPoint()
    {
        // Suaviza la posici�n de la c�mara hacia el punto est�tico definido.
        transform.position = Vector3.Lerp(transform.position, _staticPointPosition, _staticLerpSpeed * Time.deltaTime);

        // Cambia progresivamente el tama�o de la c�mara hacia el tama�o deseado.
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrthographicSize, _staticLerpSpeed * Time.deltaTime);

        // Detener el movimiento una vez que alcance el objetivo
        if (Vector3.Distance(transform.position, _staticPointPosition) < 0.01f &&
            Mathf.Abs(_camera.orthographicSize - _targetOrthographicSize) < 0.01f)
        {
            _isStatic = false; // Finalizar el movimiento y cambio de tama�o
        }
    }

    public void SetStaticPoint(Vector3 position, float lerpSpeed, float size)
    {
        // Configura la c�mara para moverse hacia un punto fijo con suavizado y cambiar el tama�o.
        _isStatic = true;
        _staticPointPosition = position;
        _staticLerpSpeed = lerpSpeed;
        _targetOrthographicSize = size;
    }

    public void ReleaseStaticPoint()
    {
        // Libera la c�mara del punto est�tico y vuelve al seguimiento del jugador.
        _isStatic = false;
        _currentStaticPoint = null;
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
