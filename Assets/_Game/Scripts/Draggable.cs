namespace NanoLife
{
	using UnityEngine;


	public class Draggable : MonoBehaviour
	{
		[SerializeField]
		private bool beingDragged = false;

		[SerializeField]
		private string requiredTool;

		private new Camera camera;
		private GameObject draggedObject;
		private GameSystem gameSystem;

		#region Properties
		public bool BeingDragged
		{
			get { return this.beingDragged; }
			set { this.beingDragged = value; }
		}


		public GameObject DraggedObject
		{
			get { return this.draggedObject; }
			set { this.draggedObject = value; }
		}
		#endregion


		public void Awake()
		{
			this.camera = Camera.main;
			this.gameSystem = FindObjectOfType<GameSystem>();
		}


		public void Update()
		{
			if (this.gameSystem.CurrentTool != null
				&& this.gameSystem.CurrentTool.Name != this.requiredTool)
			{
				return;
			}
			
			Vector2 mousePosition = this.camera.ScreenToWorldPoint(Input.mousePosition);

			if (Input.GetButtonUp("Fire1"))
			{
				this.beingDragged = false;
				this.draggedObject = null;
				return;
			}

			if (Input.GetButtonDown("Fire1"))
			{
				var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
				if (hit.collider != null)
					this.draggedObject = hit.collider.gameObject;
			}
			
			if (Input.GetButton("Fire1")
				&& this.draggedObject != null)
			{
					this.beingDragged = true;
					this.draggedObject.transform.position = mousePosition;
			}
		}
	}
}