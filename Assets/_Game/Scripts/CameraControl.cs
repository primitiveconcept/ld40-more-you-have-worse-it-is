namespace NanoLife
{
	using System;
	using UnityEngine;
	using UnityEngine.PostProcessing;


	public class CameraControl : MonoBehaviour
	{
		[Header("Ranges")]
		[SerializeField]
		private float maxPanDistance = 10;

		[SerializeField]
		private float minZoomDistance = 5;

		[SerializeField]
		private float maxZoomDistance = 15;

		[Header("Speeds")]
		[Range(0, 100)]
		[SerializeField]
		private float panSpeed = 15;

		[Range(0, 100)]
		[SerializeField]
		private float zoomSpeed = 15;

		[Range(1, 5)]
		[SerializeField]
		private float unfocusSpeed = 3;

		[Range(0, 2)]
		[SerializeField]
		private float refocusSpeed = 0.5f;

		[Header("Components")]
		[SerializeField]
		private new Camera camera;

		[SerializeField]
		private PostProcessingProfile effects;

		private Vector3 originalCameraPosition;


		public void OnEnable()
		{
			if (this.camera == null)
				this.camera = GetComponent<Camera>();
			PostProcessingBehaviour postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();

			this.effects = Instantiate(postProcessingBehaviour.profile);
			postProcessingBehaviour.profile = this.effects;

			this.camera.orthographicSize = (this.minZoomDistance);
			this.originalCameraPosition = this.transform.localPosition;
		}


		public void Update()
		{
			float horizontalAxis = Input.GetAxis("Horizontal") * Time.deltaTime * this.panSpeed;
			float verticalAxis = Input.GetAxis("Vertical") * Time.deltaTime * this.panSpeed;
			float zoomAxis = Input.GetAxis("Mouse ScrollWheel");

			


			if (horizontalAxis != 0
				|| verticalAxis != 0)
			{
				Vector3 currentPosition = this.transform.localPosition;

				currentPosition.x = Mathf.Clamp(
					currentPosition.x + horizontalAxis,
					this.originalCameraPosition.x - this.maxPanDistance,
					this.originalCameraPosition.x + this.maxPanDistance);
				currentPosition.y = Mathf.Clamp(
					currentPosition.y + verticalAxis,
					this.originalCameraPosition.y - this.maxPanDistance,
					this.originalCameraPosition.y + this.maxPanDistance);

				this.transform.localPosition = currentPosition;
			}

			DepthOfFieldModel.Settings depthOfField = this.effects.depthOfField.settings;

			if (zoomAxis != 0)
			{
				float newZoom = this.camera.orthographicSize;
				newZoom -= (zoomAxis * this.zoomSpeed);
				
				if (newZoom > this.maxZoomDistance)
					newZoom = this.maxZoomDistance;
				if (newZoom < this.minZoomDistance)
					newZoom = this.minZoomDistance;

				if (newZoom != this.camera.orthographicSize)
				{
					depthOfField.focusDistance -= 
						Math.Abs(newZoom - this.camera.orthographicSize) 
						//* this.zoomSpeed 
						* this.unfocusSpeed;
					this.camera.orthographicSize = newZoom;
				}
			}

			if (depthOfField.focusDistance < 10)
			{
				depthOfField.focusDistance += this.refocusSpeed;
				if (depthOfField.focusDistance > 10)
					depthOfField.focusDistance = 10;
			}

			this.effects.depthOfField.settings = depthOfField;
		}
	}
}