using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerAudio : MonoBehaviour
	{
		[SerializeField] private PlayerMovement playerMovement;
		[SerializeField] private AudioSource chargeAudioSource;
		[SerializeField] private AudioSource jumpAudioSource;
		[SerializeField] private AudioSource hitAudioSource;
		[SerializeField] private PlayerInput playerInput;

		private void OnEnable()
		{
			playerInput.actions["Jump"].started += OnJumpStarted;
			playerInput.actions["Jump"].canceled += OnJumpCanceled;
		}

		private void OnDisable()
		{
			playerInput.actions["Jump"].started -= OnJumpStarted;
			playerInput.actions["Jump"].canceled -= OnJumpCanceled;
		}

		private void OnJumpStarted(InputAction.CallbackContext context)
		{
			if (playerMovement.IsGrounded && !playerMovement.IsJumping())
				if (chargeAudioSource != null && !chargeAudioSource.isPlaying)
					chargeAudioSource.Play();
		}

		private void OnJumpCanceled(InputAction.CallbackContext context)
		{
			if (playerMovement.IsGrounded && !playerMovement.IsJumping())
			{
				if (chargeAudioSource != null && chargeAudioSource.isPlaying)
					chargeAudioSource.Stop();

				if (jumpAudioSource != null)
					jumpAudioSource.Play();
			}
		}
	}
}