using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVSoundEffect : MonoBehaviour {
	public AudioClip soundEffect;
	public float maxPitch = 1.1f;
	public float minPitch = 0.9f;
	public float volume = 1.0f;

	private AudioSource audioSource;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.audioSource == null) {
			audioSource = this.gameObject.AddComponent<AudioSource> ();
			audioSource.clip = soundEffect;
			audioSource.volume = volume;
			audioSource.pitch = Random.Range (minPitch, maxPitch);
			audioSource.Play ();
		}

		if (!this.audioSource.isPlaying) {
			Destroy (gameObject);
		}
	}
}
