namespace NanoLife
{
	using System;
	using System.Text;
	using UnityEngine;


	public class Broth : MonoBehaviour
	{
		[SerializeField]
		private int size;

		private bool[,] currentCells;
		private bool[,] nextCells;
		private int generation;


		#region Properties
		public int Generation
		{
			get { return this.generation; }
		}


		public bool[,] NextCells
		{
			get { return this.nextCells; }
		}


		public int Size
		{
			get { return this.size; }
		}


		public bool this[int x, int y]
		{
			get { return this.currentCells[x, y]; }
			set { this.currentCells[x, y] = value; }
		}
		#endregion


		public void Awake()
		{
			if (this.size < 0)
				throw new ArgumentOutOfRangeException("Size must be greater than zero");

			this.currentCells = new bool[this.size, this.size];
			this.nextCells = new bool[this.size, this.size];
		}


		public void ProcessNextGeneration()
		{
			for (int x = 0; x < this.size; x++)
			{
				for (int y = 0; y < this.size; y++)
				{
					int livingAdjacentCells = GetAdjacent(x, y, -1, 0)
											+ GetAdjacent(x, y, -1, 1)
											+ GetAdjacent(x, y, 0, 1)
											+ GetAdjacent(x, y, 1, 1)
											+ GetAdjacent(x, y, 1, 0)
											+ GetAdjacent(x, y, 1, -1)
											+ GetAdjacent(x, y, 0, -1)
											+ GetAdjacent(x, y, -1, -1);

					bool shouldLive = false;
					bool isAlive = this.currentCells[x, y];

					if (isAlive
						&& (livingAdjacentCells == 2 || livingAdjacentCells == 3))
					{
						shouldLive = true;
					}
					else if (!isAlive
							&& livingAdjacentCells == 3)
					{
						shouldLive = true;
					}

					this.nextCells[x, y] = shouldLive;
				}
			}

			// when a generation has completed
			// now flip the back buffer so we can start processing on the next generation
			bool[,] flip = this.nextCells;
			this.nextCells = this.currentCells;
			this.currentCells = flip;
			this.generation++;
		}


		public void Randomize()
		{
			for (int x = 0; x < this.Size; x++)
			{
				for (int y = 0; y < this.Size; y++)
				{
					int random = UnityEngine.Random.Range(0, 2);
					this[x, y] = random == 1;
				}
			}
		}


		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			for (int x = 0; x < this.Size; x++)
			{
				for (int y = 0; y < this.Size; y++)
				{
					string cell = this[y, x]
									? "X"
									: "-";
					result.Append(cell);
				}
				result.AppendLine();
			}

			return result.ToString();
		}


		#region Helper Methods
		private int GetAdjacent(int x, int y, int offsetx, int offsety)
		{
			int proposedOffsetX = x + offsetx;
			int proposedOffsetY = y + offsety;

			bool outOfBounds =
				proposedOffsetX < 0
				|| proposedOffsetX >= this.size
				| proposedOffsetY < 0
				|| proposedOffsetY >= this.size;

			if (!outOfBounds)
			{
				return this.currentCells[x + offsetx, y + offsety]
					? 1
					: 0;
			}

			return 0;
		}
		#endregion
	}
}