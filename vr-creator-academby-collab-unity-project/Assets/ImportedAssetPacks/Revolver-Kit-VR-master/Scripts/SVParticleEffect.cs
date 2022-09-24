using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVParticleEffect : MonoBehaviour {
	//------------------------
	// Variables
	//------------------------
	public AudioClip soundEffect;
	public float maxPitch = 1.1f;
	public float minPitch = 0.9f;
	public float volume = 1.0f;

	private ParticleSystem ps;
	private bool didPlaySoundEffect = false;
	private AudioSource audioSource;

	// Use this for initialization
	void Awake () {
		ps = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ps.isPlaying || ps.IsAlive ()) {
			if (soundEffect != null && !didPlaySoundEffect && ps.isPlaying) {
				audioSource = this.gameObject.AddComponent<AudioSource> ();
				audioSource.clip = soundEffect;
				audioSource.volume = volume;
				audioSource.pitch = Random.Range (minPitch, maxPitch);
				audioSource.Play ();
				didPlaySoundEffect = true;
			}
		} else {
			Destroy (gameObject);
		}
	}
}
