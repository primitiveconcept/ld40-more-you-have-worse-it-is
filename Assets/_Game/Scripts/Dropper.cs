namespace NanoLife
{
	using UnityEngine;


	public class Dropper : MonoBehaviour
	{
		private new Camera camera;
		private BrothViewControl[] broths;
		private Grid grid;

		[SerializeField]
		private AudioClip dropSound;

		public void Awake()
		{
			this.camera = Camera.main;
			this.broths = FindObjectsOfType<BrothViewControl>();
			this.grid = this.broths[0].Tilemap.layoutGrid;
		}


		public void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				int size = this.broths[0].Broth.Size;
				Vector2 mousePosition = this.camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int gridPosition = this.grid.WorldToCell(mousePosition);
				if (gridPosition.x < size
					&& gridPosition.x > -1
					&& gridPosition.y < size
					&& gridPosition.y > -1)
				{
					foreach (BrothViewControl broth in this.broths)
					{
						foreach (Vector3Int cell in Direction.Vector)
						{
							if (UnityEngine.Random.Range(0, 2) == 0)
								continue;

							Vector3Int drop = new Vector3Int(
								gridPosition.x + cell.x,
								gridPosition.y + cell.y,
								0);
							if (drop.x > -1
								&& drop.x < size
								&& drop.y > -1
								&& drop.y < size)
							{
								broth.Broth[drop.x, drop.y] = true;
								broth.Broth.NextCells[drop.x, drop.y] = true;
								broth.Tilemap.SetTile(drop, broth.Tile);
							}
						}
					}

					if (this.dropSound != null)
						AudioPlayer.Play(this.dropSound);
				}
			}
		}
	}
}