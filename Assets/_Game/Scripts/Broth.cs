namespace NanoLife
{
	using System;
	using System.Text;
	using UnityEngine;


	public class Broth : MonoBehaviour
	{
		private Action<bool[,]> nextGenerationCompleted;

		[SerializeField]
		private int size;

		private bool[,] broth;
		private bool[,] nextGeneration;
		private Coroutine processTask;


		#region Properties
		public bool[,] NextGeneration
		{
			get { return this.nextGeneration; }
		}


		public int Generation { get; private set; }


		public int Size
		{
			get { return this.size; }
		}


		public bool this[int x, int y]
		{
			get { return this.broth[x, y]; }
			set { this.broth[x, y] = value; }
		}
		#endregion


		public void Awake()
		{
			if (this.size < 0)
				throw new ArgumentOutOfRangeException("Size must be greater than zero");

			this.broth = new bool[this.size, this.size];
			this.nextGeneration = new bool[this.size, this.size];
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


		public bool ToggleCell(int x, int y)
		{
			bool currentValue = this.broth[x, y];
			return this.broth[x, y] = !currentValue;
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
		private static int IsNeighborAlive(bool[,] world, int size, int x, int y, int offsetx, int offsety)
		{
			int result = 0;

			int proposedOffsetX = x + offsetx;
			int proposedOffsetY = y + offsety;
			bool outOfBounds = proposedOffsetX < 0 || proposedOffsetX >= size | proposedOffsetY < 0 || proposedOffsetY >= size;
			if (!outOfBounds)
			{
				result = world[x + offsetx, y + offsety] ? 1 : 0;
			}
			return result;
		}


		public void ProcessNextGeneration()
		{
			for (int x = 0; x < this.size; x++)
			{
				for (int y = 0; y < this.size; y++)
				{
					int numberOfNeighbors = IsNeighborAlive(this.broth, this.Size, x, y, -1, 0)
											+ IsNeighborAlive(this.broth, this.Size, x, y, -1, 1)
											+ IsNeighborAlive(this.broth, this.Size, x, y, 0, 1)
											+ IsNeighborAlive(this.broth, this.Size, x, y, 1, 1)
											+ IsNeighborAlive(this.broth, this.Size, x, y, 1, 0)
											+ IsNeighborAlive(this.broth, this.Size, x, y, 1, -1)
											+ IsNeighborAlive(this.broth, this.Size, x, y, 0, -1)
											+ IsNeighborAlive(this.broth, this.Size, x, y, -1, -1);

					bool shouldLive = false;
					bool isAlive = this.broth[x, y];

					if (isAlive && (numberOfNeighbors == 2 || numberOfNeighbors == 3))
					{
						shouldLive = true;
					}
					else if (!isAlive
							&& numberOfNeighbors == 3) // zombification
					{
						shouldLive = true;
					}

					this.nextGeneration[x, y] = shouldLive;
				}
			}
			
			// when a generation has completed
			// now flip the back buffer so we can start processing on the next generation
			bool[,] flip = this.nextGeneration;
			this.nextGeneration = this.broth;
			this.broth = flip;
			this.Generation++;
		}
		#endregion
	}
}