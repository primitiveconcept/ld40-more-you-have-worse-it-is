namespace NanoLife
{
	using UnityEngine;


	public class Draggable : MonoBehaviour
	{
		[SerializeField]
		private bool beingDragged = false;

		private Camera camera; //For storing the camera
		private RaycastHit2D draggedObject; //For storing the puzzle piece we are dragging


		#region Properties
		public bool BeingDragged
		{
			get { return this.beingDragged; }
			set { this.beingDragged = value; }
		}

		public RaycastHit2D DraggedObject
		{
			get { return this.draggedObject; }
			set { this.draggedObject = value; }
		}
		#endregion


		public void Awake()
		{
			this.camera = Camera.main;
		}


		#region Helper Methods
		public void Update()
		{
			//Store our mouse position at the beginning of the frame for use later
			Vector2 mousePos = this.camera.ScreenToWorldPoint(Input.mousePosition);

			//Did we mouse click? "Fire1" is set to use Mouse0 in Edit > Project Settings > Input Manager
			if (Input.GetButtonDown("Fire1"))
			{
				//Shoot a ray at the exact position of our mouse, and store the returned result into draggedObject
				this.draggedObject = Physics2D.Raycast(mousePos, Vector2.zero);
			}

			//Are we holding the mouse button down?
			if (Input.GetButton("Fire1"))
			{
				//Is the collider of our draggedObject RaycastHit2D variable NOT null?
				if (this.draggedObject.collider != null)
				{
					this.beingDragged = true;
					//Set the position of our draggedObject to be equal to our mouse position
					this.draggedObject.collider.transform.position = mousePos;

					//Optional: If using Z-Axis to determine sprite render order, use these lines instead
					//Transform puzzTrans = draggedObject.collider.transform;
					//puzzTrans.position = new Vector3(mousePos.x, mousePos.y, puzzTrans.position.z);
				}
			}

			//Did we let go of the mouse button?
			if (Input.GetButtonUp("Fire1"))
			{
				//Reset the draggedObject to null
				this.draggedObject = new RaycastHit2D();
				this.beingDragged = false;
			}
		}
		#endregion
	}
}