namespace NanoLife
{
	using System;
	using UnityEngine;


	public class Nanomachine : MonoBehaviour
	{
		[SerializeField]
		private int health = 2;
		
		private Draggable draggable;
		private Vector3 directionalForce;
		private new Rigidbody2D rigidbody2D;


		#region Properties
		public Vector3 DirectionalForce
		{
			get { return this.directionalForce; }
			set { this.directionalForce = value; }
		}


		public Draggable Draggable
		{
			get { return this.draggable; }
		}


		public int Health
		{
			get { return this.health; }
			set { this.health = value; }
		}
		#endregion


		public void Awake()
		{
			this.directionalForce = this.transform.localPosition;
			this.draggable = GetComponent<Draggable>();
			this.rigidbody2D = GetComponent<Rigidbody2D>();
		}


		public int[] GetAdjacentBacteria(NanomachineSystem system)
		{
			Grid grid = system.Grid;
			BrothViewControl[] broths = system.Broths;
			Vector3Int gridCoordinate = grid.WorldToCell(this.transform.position);
			Vector3Int left = gridCoordinate + Direction.LeftVector;
			Vector3Int right = gridCoordinate + Direction.RightVector;
			Vector3Int up = gridCoordinate + Direction.UpVector;
			Vector3Int down = gridCoordinate + Direction.DownVector;
			Vector3Int upperLeft = gridCoordinate + Direction.UpperLeftVector;
			Vector3Int upperRight = gridCoordinate + Direction.UpperRightVector;
			Vector3Int lowerLeft = gridCoordinate + Direction.LowerLeftVector;
			Vector3Int lowerRight = gridCoordinate + Direction.LowerRightVector;

			int[] cells = new int[9];
			for (int i = 0; i < broths.Length; i++)
			{
				BrothViewControl broth = broths[i];
				cells[0] += Convert.ToInt32(broth.Tilemap.HasTile(gridCoordinate));
				cells[1] += Convert.ToInt32(broth.Tilemap.HasTile(left));
				cells[2] += Convert.ToInt32(broth.Tilemap.HasTile(right));
				cells[3] += Convert.ToInt32(broth.Tilemap.HasTile(up));
				cells[4] += Convert.ToInt32(broth.Tilemap.HasTile(down));
				cells[5] += Convert.ToInt32(broth.Tilemap.HasTile(upperLeft));
				cells[6] += Convert.ToInt32(broth.Tilemap.HasTile(upperRight));
				cells[7] += Convert.ToInt32(broth.Tilemap.HasTile(lowerLeft));
				cells[8] += Convert.ToInt32(broth.Tilemap.HasTile(lowerRight));
			}

			return cells;
		}


		public void Update()
		{
			this.rigidbody2D.angularVelocity = 360 * 8;

			if (this.draggable.BeingDragged)
			{
				iTween.Stop(this.gameObject);
				this.directionalForce = this.transform.localPosition;
				return;
			}

			if (this.directionalForce == Vector3.zero)
				this.rigidbody2D.velocity = Vector3.zero;
			else
				this.rigidbody2D.AddRelativeForce(this.directionalForce * 1);

			var scale = 0.5f + (0.5f * this.health);
			Vector3 targetScale = new Vector3(
				scale,
				scale,
				1);
			iTween.ScaleTo(this.gameObject, targetScale, 1);
		}
	}
}