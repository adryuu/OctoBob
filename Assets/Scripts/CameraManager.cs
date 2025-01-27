using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _smoothSpeed = 0.125f;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _yOffset;
    [SerializeField] private GameObject[] _staticPoints;

    private GameObject _currentStaticPoint = null;
    private bool _isStatic = false;
    private Vector3 _staticPointPosition;
    private float _staticLerpSpeed;
    private float _targetOrthographicSize;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (_isStatic)
        {
            MoveToStaticPoint();
        }
        else if (_currentStaticPoint != null)
        {
            Vector3 staticPosition = _currentStaticPoint.transform.position;
            transform.position = new Vector3(staticPosition.x + _xOffset, staticPosition.y + _yOffset, transform.position.z);
        }
        else
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        float desiredX = _player.position.x + _xOffset;
        float desiredY = _player.position.y + _yOffset;

        float smoothedX = Mathf.Lerp(transform.position.x, desiredX, _smoothSpeed);
        float smoothedY = Mathf.Lerp(transform.position.y, desiredY, _smoothSpeed);

        transform.position = new Vector3(smoothedX, smoothedY, transform.position.z);
    }

    private void MoveToStaticPoint()
    {
        transform.position = Vector3.Lerp(transform.position, _staticPointPosition, _staticLerpSpeed * Time.deltaTime);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrthographicSize, _staticLerpSpeed * Time.deltaTime);

        // Se elimina la condición que desactiva `_isStatic`
        // La cámara permanecerá fija en el punto especificado
    }

    public void SetStaticPoint(Vector3 position, float lerpSpeed, float size)
    {
        _isStatic = true;
        _staticPointPosition = position;
        _staticLerpSpeed = lerpSpeed;
        _targetOrthographicSize = size;
    }

    public void ReleaseStaticPoint()
    {
        // Este método ahora solo se llama explícitamente si deseas liberar el punto fijo
        _isStatic = false;
        _currentStaticPoint = null;
    }

    public void CheckStaticPoints()
    {
        foreach (GameObject point in _staticPoints)
        {
            if (point == null) continue;

            float distance = Mathf.Abs(_player.position.x - point.transform.position.x);
            if (distance < 1f)
            {
                _currentStaticPoint = point;
                return;
            }
        }

        _currentStaticPoint = null;
    }

    public void RemoveStaticPoint(GameObject point)
    {
        for (int i = 0; i < _staticPoints.Length; i++)
        {
            if (_staticPoints[i] == point)
            {
                _staticPoints[i] = null;
                Destroy(point);
                break;
            }
        }

        CheckStaticPoints();
    }
}
