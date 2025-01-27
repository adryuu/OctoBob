using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [System.Serializable]
    private class ParallaxLayer
    {
        public Transform layerTransform;
        public float parallaxFactor;
    }

    [SerializeField]
    private ParallaxLayer[] _parallaxLayers;

    [SerializeField]
    private Camera _mainCamera;

    private Vector3 _previousCameraPosition;

    private void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        _previousCameraPosition = _mainCamera.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = _mainCamera.transform.position - _previousCameraPosition;

        foreach (var layer in _parallaxLayers)
        {
            if (layer.layerTransform == null) continue;

            Vector3 parallaxMovement = new Vector3(cameraDelta.x * layer.parallaxFactor, cameraDelta.y * layer.parallaxFactor, 0);

            layer.layerTransform.position += parallaxMovement;
        }

        _previousCameraPosition = _mainCamera.transform.position;
    }
}
