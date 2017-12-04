namespace NanoLife
{
	using UnityEngine;


	public class Food : MonoBehaviour
	{
		private BrothViewControl[] broths;
		private Grid grid;


		public void Awake()
		{
			this.broths = FindObjectsOfType<BrothViewControl>();
			this.grid = this.broths[0].Tilemap.layoutGrid;
		}


		public void Update()
		{
			int size = this.broths[0].Broth.Size;
			Vector2 foodPosition = this.transform.position;
			Vector3Int gridPosition = this.grid.WorldToCell(foodPosition);
			if (gridPosition.x < size
				&& gridPosition.x > -1
				&& gridPosition.y < size
				&& gridPosition.y > -1)
			{
				foreach (BrothViewControl broth in this.broths)
				{
					foreach (Vector3Int cell in Direction.Vector)
					{
						Vector3Int occupied = new Vector3Int(
							gridPosition.x + cell.x,
							gridPosition.y + cell.y,
							0);
						if (occupied.x > -1
							&& occupied.x < size
							&& occupied.y > -1
							&& occupied.y < size)
						{
							if (broth.Broth[occupied.x, occupied.y] == true
								|| broth.Broth.NextCells[occupied.x, occupied.y] == true)
							{
								broth.Broth[occupied.x, occupied.y] = true;
								broth.Broth.NextCells[occupied.x, occupied.y] = true;
							}
						}
					}
				}
			}
		}
	}
}