using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGuyBehaviour : MonoBehaviour
{
    private Animator badGuyAnimator;
    private float _spawnTime;
    [SerializeField] private float _spawnLifeTime = 12f;

    private AudioSource source;
    public AudioClip metalHit;


    private void Start()
    {   // Initialize Bad Guys Animator
        badGuyAnimator = GetComponent<Animator>();

        // Initialize Bad Guys 'Up Delay'
        _spawnTime = _spawnLifeTime;

        // Initialize Audio
        source = GetComponent<AudioSource>();
        
     }

    private void Update()
    {
        //Create Randomizer
        float random = Random.Range(0, 5);

        //Wait Random Delay
        random -= Time.deltaTime;
        if(random <= 0)
        {
            //Pop Up
            badGuyAnimator.SetTrigger("PopUp");
        }
        // 'Up Delay' starts   
        _spawnTime -= Time.deltaTime;
        if (_spawnTime <= 0)
        {
            //Bad Guy Resets back to Hiding
            badGuyAnimator.SetTrigger("Reset");
            _spawnTime = _spawnLifeTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            badGuyAnimator.SetTrigger("Hit");
            source.PlayOneShot(metalHit);
        }
    }
}
