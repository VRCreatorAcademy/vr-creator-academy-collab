using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBadGuys : MonoBehaviour
{
    public Transform[] spawnLocation;
    public GameObject badGuyPrefab;
    private float _spawnTime;
    [SerializeField] private float _spawnLifeTime = 30f;
    public bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        _spawnTime = _spawnLifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Game Timer
        if (gameStarted == true)
        {
            _spawnTime -= Time.deltaTime;
            if (_spawnTime <= 0)
            {
                EndGame();
                _spawnTime = _spawnLifeTime;
            }
        }
    }

    public void ActivateBadGuys()
    {
        if (gameStarted == false)
        {
            // The clumsiest way to spawn... ever

            Instantiate(badGuyPrefab, spawnLocation[0]);
            Instantiate(badGuyPrefab, spawnLocation[1]);
            Instantiate(badGuyPrefab, spawnLocation[2]);
            
            gameStarted = true;
        }
    }

    public void EndGame()
    {
        //End game here
        //Post score?
    }
}
