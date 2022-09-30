using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Animator				animator;
	// Start is called before the first frame update
	void Start()
	{
	animator = GetComponent<Animator>();
	}


	// Update is called once per frame
	void Update()
	{
		
	}


	public void OnLeftGrip(InputAction.CallbackContext context)
	{
		float grip = context.ReadValue<float>();
		animator.SetFloat("Left Grip", grip);
	}


	public void OnRightGrip(InputAction.CallbackContext context)
	{
		float grip = context.ReadValue<float>();
		animator.SetFloat("Right Grip", grip);
	}
}
