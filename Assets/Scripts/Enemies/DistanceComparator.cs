using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceComparator : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private GiantCrabBoss _giantCrabBoss;

    private void Awake()
    {
        _giantCrabBoss.enabled = false;
    }

    void Update()
    {
        if (transform.position.x - _player.position.x > 4)
        {
            if (_giantCrabBoss.enabled)
            {
                _giantCrabBoss.enabled = false;
            }
        }
        else
        {
            _giantCrabBoss.enabled = true;
        }
    }
}
