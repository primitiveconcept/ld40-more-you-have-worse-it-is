namespace NanoLife
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;


	public class NanomachineSystem : MonoBehaviour
	{
		[Header("Nanomachine Stats")]
		[SerializeField]
		private GameObject nanomachinePrefab;

		[SerializeField]
		private int startingHealth = 2;

		[SerializeField]
		private int healthToDivide = 4;

		[SerializeField]
		private float processInterval = 1;

		[SerializeField]
		private float heatGeneration = 0.1f;

		[Header("Sounds")]
		[SerializeField]
		private AudioClip divideSound;

		[SerializeField]
		private AudioClip deathSound;

		[SerializeField]
		private AudioClip eatSound;

		[Header("Collections")]
		[SerializeField]
		private List<Nanomachine> active;

		[SerializeField]
		private BrothViewControl[] broths;
		
		private List<Nanomachine> survivers;
		private GameSystem gameSystem;
		private Grid grid;
		private float timer;


		#region Properties
		public BrothViewControl[] Broths
		{
			get { return this.broths; }
		}


		public Grid Grid
		{
			get { return this.grid; }
		}
		#endregion


		public void Awake()
		{
			this.broths = FindObjectsOfType<BrothViewControl>();
			this.gameSystem = FindObjectOfType<GameSystem>();
			this.grid = this.broths[0].Tilemap.layoutGrid;
		}


		public void CreateNanomachine(Vector3 localPosition)
		{
			GameObject newInstance = Instantiate(this.nanomachinePrefab, localPosition, Quaternion.identity);
			Nanomachine nanomachine = newInstance.GetComponent<Nanomachine>();
			nanomachine.Health = this.startingHealth;
			nanomachine.DirectionalForce = nanomachine.transform.localPosition;
			this.survivers.Add(nanomachine);
		}


		public void DivideNanomachine(Nanomachine original)
		{
			int randomDirection = Random.Range(1, 9);
			Vector3 spawnToPosition = original.transform.localPosition
									+ Direction.Vector[randomDirection];

			CreateNanomachine(original.transform.localPosition);
			CreateNanomachine(spawnToPosition);
			int originalIndex = this.active.IndexOf(original);
			this.active[originalIndex] = null;
			Destroy(original.gameObject);

			this.gameSystem.AddEnergy(0.05f);

			if (this.divideSound != null)
				AudioPlayer.Play(this.divideSound);
		}


		public void Update()
		{
			if (this.timer >= this.processInterval)
			{
				this.timer = 0;
				Process();
			}
			else
			{
				this.timer += Time.deltaTime;
			}
		}


		#region Helper Methods
		private bool IsOverwhelmed(int[] cells)
		{
			return cells.Sum() > 4;
		}


		private void Process()
		{
			if (this.active.Count == 0)
			{
				this.gameSystem.ShowGameOver(
					"Your last nanomachine perished\n\n" +
					"Game Over");
			}

			this.survivers = new List<Nanomachine>();
			for (int i = 0; i < this.active.Count; i++)
			{
				if (this.active[i] != null)
					ProcessNanomachine(i, this.active[i].GetAdjacentBacteria(this));

				if (this.active[i] != null)
				{
					this.survivers.Add(this.active[i]);

					Vector3Int gridPosition = this.grid.WorldToCell(this.active[i].transform.position);
					if (gridPosition.x > -1
						&& gridPosition.x < this.broths[0].Broth.Size
						&& gridPosition.y > -1
						&& gridPosition.y < this.broths[0].Broth.Size)
					{
						this.gameSystem.AddHeat(this.active[i].Health * this.heatGeneration);
					}
				}
			}

			this.active = this.survivers;
		}


		private void ProcessNanomachine(int index, int[] adjacentBacteria)
		{
			Nanomachine nanomachine = this.active[index];

			if (nanomachine.Draggable.BeingDragged)
				return;

			if (IsOverwhelmed(adjacentBacteria))
			{
				nanomachine.Health--;
				if (nanomachine.Health < 1)
				{
					if (this.deathSound != null)
						AudioPlayer.Play(this.deathSound);

					Destroy(nanomachine.gameObject);
					this.active[index] = null;
					return;
				}
			}

			if (adjacentBacteria[0] > 0)
			{
				nanomachine.Health += adjacentBacteria[0];
				Vector3Int cell = this.grid.WorldToCell(nanomachine.transform.position);
				Debug.Log("Eating: " + cell);

				if (this.eatSound != null)
					AudioPlayer.Play(this.eatSound);

				foreach (BrothViewControl broth in this.broths)
				{
					broth.Broth[cell.x, cell.y] = false;
					broth.Broth.NextCells[cell.x, cell.y] = false;

					int maxIndex = broth.Broth.Size - 1;
					if (cell.x > 0)
					{
						broth.Broth[cell.x - 1, cell.y] = false;
						broth.Broth.NextCells[cell.x - 1, cell.y] = false;
					}

					if (cell.y > 0)
					{
						broth.Broth[cell.x, cell.y - 1] = false;
						broth.Broth.NextCells[cell.x, cell.y - 1] = false;
					}

					if (cell.x < maxIndex)
					{
						broth.Broth[cell.x + 1, cell.y] = false;
						broth.Broth.NextCells[cell.x + 1, cell.y] = false;
					}

					if (cell.y < maxIndex)
					{
						broth.Broth[cell.x, cell.y + 1] = false;
						broth.Broth.NextCells[cell.x, cell.y + 1] = false;
					}
				}

				if (nanomachine.Health >= this.healthToDivide)
					DivideNanomachine(nanomachine);

				return;
			}

			int largestBacteriaCount = adjacentBacteria.Max();
			int moveDirection = adjacentBacteria.First(count => count == largestBacteriaCount);
			nanomachine.DirectionalForce = Direction.Vector[moveDirection] * (-1);
			//Debug.Log("Moving toward (" + largestBacteriaCount + "): " + Direction.Vector[moveDirection]);
		}
		#endregion
	}
}