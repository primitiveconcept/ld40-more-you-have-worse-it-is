namespace NanoLife
{
	using System.Collections.Generic;
	using UnityEngine;


	public class GameSystem : MonoBehaviour
	{
		public const string Magnet = "Magnet";

		[SerializeField]
		private Tool[] tools;

		private Camera camera;
		private Tool currentTool;
		private Dictionary<string, Tool> toolsIndex;


		#region Properties
		public Tool CurrentTool
		{
			get { return this.currentTool; }
			set { this.currentTool = value; }
		}
		#endregion


		public void Awake()
		{
			this.camera = Camera.main;

			this.toolsIndex = new Dictionary<string, Tool>();
			foreach (Tool tool in this.tools)
			{
				this.toolsIndex.Add(tool.Name, tool);
				tool.GameObject.SetActive(false);
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


		public void Update()
		{
			if (this.currentTool != null)
			{
				Vector3 toolPosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
				toolPosition.z = 0;
				this.currentTool.GameObject.transform.position = toolPosition;
			}
		}
	}
}