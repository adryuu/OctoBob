using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Transform _actualCheckpoint;

    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _zero;
    [SerializeField]
    private Transform _firstCheckpoint;
    [SerializeField]
    private Transform _secondCheckpoint;
    [SerializeField]
    private Transform _thirdCheckpoint;
    [SerializeField]
    private Transform _fourthCheckpoint;
    [SerializeField]
    private Transform _fifthCheckpoint;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }
    private void Update()
    {
        CheckPointChecker();
    }

    private void SetCheckpoint(Transform checkpoint)
    {
        _actualCheckpoint = checkpoint;
        Debug.Log($"Checkpoint set to: {_actualCheckpoint.name}");
    }

    private void CheckPointChecker()
    {
        if (_player.position.x < _firstCheckpoint.position.x)
        {
            SetCheckpoint(_zero);
        }
        else if (_player.position.x > _firstCheckpoint.position.x && _player.position.x < _secondCheckpoint.position.x)
        {
            SetCheckpoint(_firstCheckpoint);
        }
        else if (_player.position.x > _secondCheckpoint.position.x && _player.position.x < _thirdCheckpoint.position.x)
        {
            SetCheckpoint(_secondCheckpoint);
        }
        else if (_player.position.x > _thirdCheckpoint.position.x && _player.position.x < _fourthCheckpoint.position.x)
        {
            SetCheckpoint(_thirdCheckpoint);
        }
        else if (_player.position.x > _fourthCheckpoint.position.x)
        {
            SetCheckpoint(_fifthCheckpoint);
        }
    }

    public void TeleportToCheckPoint()
    {
        if (_actualCheckpoint != null)
        {
            _player.position = new Vector3(_actualCheckpoint.position.x, _actualCheckpoint.position.y, transform.position.z);
            Debug.Log($"Teleported to: {_actualCheckpoint.name}");
        }
        else
        {
            Debug.LogWarning("No checkpoint set to teleport to.");
        }
    }
}

