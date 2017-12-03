namespace NanoLife
{
	using UnityEngine;
	using UnityEngine.Tilemaps;


	public class BrothViewControl : MonoBehaviour
	{
		[SerializeField]
		private Broth broth;

		[SerializeField]
		private Tilemap tilemap;

		[SerializeField]
		private TileBase tile;

		[Range(0.5f, 1)]
		[SerializeField]
		private float speed = 0.75f;

		private float timer;


		#region Properties
		public Broth Broth
		{
			get { return this.broth; }
		}


		public float Speed
		{
			get { return this.speed; }
			set { this.speed = value; }
		}


		public Tilemap Tilemap
		{
			get { return this.tilemap; }
		}
		#endregion


		public void Start()
		{
			if (this.broth == null)
				this.broth = GetComponent<Broth>();

			this.broth.Randomize();
			Debug.Log(this.broth);
		}


		public void Update()
		{
			this.timer += Time.deltaTime;
			if (this.timer > (1 - this.speed))
			{
				this.broth.ProcessNextGeneration();
				for (int x = 0; x < this.broth.Size; x++)
				{
					for (int y = 0; y < this.broth.Size; y++)
					{
						ProcessCell(x, y, this.broth[x, y]);
					}
				}

				this.timer = 0;
			}
		}


		#region Helper Methods
		private void ProcessCell(int x, int y, bool isPresent)
		{
			if (isPresent)
			{
				if (this.tilemap.GetTile(new Vector3Int(x, y, 0)) == null)
				{
					this.tilemap.SetTile(
						new Vector3Int(x, y, 0),
						this.tile);
				}
			}
			else
			{
				if (this.tilemap.GetTile(new Vector3Int(x, y, 0)) != null)
				{
					this.tilemap.SetTile(
						new Vector3Int(x, y, 0),
						null);
				}
			}
		}
		#endregion
	}
}