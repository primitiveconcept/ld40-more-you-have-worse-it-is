namespace NanoLife
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;


	public class GameSystem : MonoBehaviour
	{
		[SerializeField]
		private GameObject endGameMenu;

		[SerializeField]
		private Image fader;

		[SerializeField]
		private float energy;

		[SerializeField]
		private Image energyMeter;

		[Header("Heat")]
		[SerializeField]
		private float heat;
		
		[SerializeField]
		private float heatLossAmount = 0.06f;

		[SerializeField]
		private Image heatMeter;

		[SerializeField]
		private Tool[] tools;

		private Camera camera;
		private Tool currentTool;
		private Dictionary<string, Tool> toolsIndex;
		private BrothViewControl[] broths;
		private Coroutine endGameCoroutine;
		

		#region Properties
		public BrothViewControl[] Broths
		{
			get { return this.broths; }
		}


		public Tool CurrentTool
		{
			get { return this.currentTool; }
			set { this.currentTool = value; }
		}


		public float Heat
		{
			get { return this.heat; }
		}


		public float HeatLossAmount
		{
			get { return this.heatLossAmount; }
		}
		#endregion


		public void AddHeat(float amount)
		{
			this.heat += amount;
			if (this.heat < 0)
				this.heat = 0;
			if (this.heat > 2)
				this.heat = 2;

			float bacteriaSpeed = HeatToSpeed(this.heat);

			foreach (BrothViewControl broth in this.broths)
			{
				broth.Speed = bacteriaSpeed;
			}
		}


		public void AddEnergy(float amount)
		{
			this.energy += amount;
			if (this.energy < 0)
				this.energy = 0;
			if (this.energy > 1)
				this.energy = 1;

			if (this.energy >= 1)
				ShowWinScreen();
		}


		public void RemoveEnergy(float amount)
		{
			this.energy -= amount;
			if (this.energy < 0)
				this.energy = 0;
			if (this.energy > 1)
				this.energy = 1;

			// TODO: WIN at 1.
		}


		public void Awake()
		{
			this.endGameMenu.gameObject.SetActive(false);
			this.fader.color = Color.black;
			StartCoroutine(
				FadeImage(
					this.fader,
					1,
					Color.clear,
					() =>
						{
							this.fader.gameObject.SetActive(false);
						}));

			this.camera = Camera.main;
			this.broths = FindObjectsOfType<BrothViewControl>();

			this.toolsIndex = new Dictionary<string, Tool>();
			foreach (Tool tool in this.tools)
			{
				this.toolsIndex.Add(tool.Name, tool);
				tool.GameObject.SetActive(false);
			}

			SelectTool("Dropper");
		}


		public void RemoveHeat(float amount)
		{
			this.heat -= amount;
			if (this.heat < 0)
				this.heat = 0;
			if (this.heat > 2)
				this.heat = 2;

			this.heatMeter.fillAmount = this.heat / 2;

			float bacteriaSpeed = HeatToSpeed(this.heat);

			foreach (BrothViewControl broth in this.broths)
			{
				broth.Speed = bacteriaSpeed;
			}
		}


		public void SelectTool(string tool)
		{
			if (this.currentTool != null)
				this.currentTool.GameObject.SetActive(false);

			this.currentTool = this.toolsIndex[tool];
			this.currentTool.GameObject.SetActive(true);
			Vector3 toolPosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
			toolPosition.z = 0;
			this.currentTool.GameObject.transform.position = toolPosition;
		}


		public void ShowGameOver(string text)
		{
			if (this.endGameCoroutine == null)
			{
				this.fader.gameObject.SetActive(true);
				this.endGameCoroutine = StartCoroutine(
					FadeImage(
						this.fader,
						3,
						Color.black,
						() =>
							{
								ShowEndText(text);
							}));
			}
		}


		public void Restart()
		{
			SceneManager.LoadScene(0);
		}


		private void ShowEndText(string text)
		{
			this.endGameMenu.SetActive(true);
			this.endGameMenu.GetComponentInChildren<Text>().text = text;
		}


		/// <summary>
		/// Fades the specified image to the target opacity and duration.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="opacity">Opacity.</param>
		/// <param name="duration">Duration.</param>
		public IEnumerator FadeImage(Image target, float duration, Color color, Action callback)
		{
			target.gameObject.SetActive(true);
			if (target == null)
				yield break;

			float alpha = target.color.a;

			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
			{
				if (target == null)
					yield break;
				Color newColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
				target.color = newColor;
				yield return null;
			}
			target.color = color;
			if (callback != null)
				callback();
		}


		public void Update()
		{
			if (this.currentTool != null)
			{
				Vector3 toolPosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
				toolPosition.z = 0;
				this.currentTool.GameObject.transform.position = toolPosition;
			}

			RemoveHeat(Time.deltaTime * this.heatLossAmount);
			this.heatMeter.fillAmount =
				Mathf.Lerp(this.heatMeter.fillAmount, this.heat / 2, 0.5f);

			this.energyMeter.fillAmount =
				Mathf.Lerp(this.energyMeter.fillAmount, this.energy / 1, 0.5f);

			if (Input.GetKeyDown(KeyCode.Alpha1))
				SelectTool("Dropper");
			if (Input.GetKeyDown(KeyCode.Alpha2))
				SelectTool("Magnet");
			if (Input.GetKeyDown(KeyCode.Alpha3))
				SelectTool("Forceps");
		}


		private void ShowWinScreen()
		{
			ShowGameOver("You managed to fully charge the device\n\n" +
						"You Win!");
		}


		#region Helper Methods
		private float HeatToSpeed(float heat)
		{
			return -(this.heat * this.heat) + (2 * this.heat);
		}
		#endregion
	}
}