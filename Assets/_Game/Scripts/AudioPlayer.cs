namespace NanoLife
{
	using UnityEngine;


	public class AudioPlayer : MonoBehaviour
	{
		private static AudioPlayer instance;
		private AudioSource audioSource;


		public static void Play(AudioClip clip)
		{
			instance.audioSource.PlayOneShot(clip);
		}


		public void Awake()
		{
			instance = this;
			this.audioSource = GetComponent<AudioSource>();
		}
	}
}