using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("recty prefab")]
    public GameObject recty_prefab;

    [Space, Header("Spawner variables")]
    public Transform spawning_line_1;
    public Transform spawning_line_2;
    public Transform spawning_line_3;
    public Transform spawning_line_4;
    [Space]
    public float Timer_min;
    public float Timer_max;

    private float _timer;
    void FixedUpdate()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            spawnRecty();
            updateTimer();
        }
    }

    public void spawnRecty()
    {
        Instantiate(recty_prefab, gameObject.transform);
    }

    public void updateTimer()
    {
        _timer = Random.Range(Timer_min, Timer_max);
    }
}
