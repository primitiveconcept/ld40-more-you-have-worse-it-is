namespace NanoLife
{
	using UnityEngine;


	public static class Direction
	{
		public const int Center = 0;

		public static readonly Vector3Int CenterVector = Vector3Int.zero;
		public const int Down = 4;
		public static readonly Vector3Int DownVector = new Vector3Int(0, -1, 0);
		public const int Left = 1;
		public static readonly Vector3Int LeftVector = new Vector3Int(-1, 0, 0);
		public const int LowerLeft = 7;
		public static readonly Vector3Int LowerLeftVector = new Vector3Int(-1, -1, 0);
		public const int LowerRight = 8;
		public static readonly Vector3Int LowerRightVector = new Vector3Int(1, -1, 0);
		public const int Right = 2;
		public static readonly Vector3Int RightVector = new Vector3Int(1, 0, 0);
		public const int Up = 3;
		public const int UpperLeft = 5;
		public static readonly Vector3Int UpperLeftVector = new Vector3Int(-1, 1, 0);
		public const int UpperRight = 6;
		public static readonly Vector3Int UpperRightVector = new Vector3Int(1, 1, 0);
		public static readonly Vector3Int UpVector = new Vector3Int(0, 1, 0);

		public static readonly Vector3Int[] Vector =
			new Vector3Int[]
				{
					CenterVector,
					LeftVector,
					RightVector,
					UpVector,
					DownVector,
					UpperLeftVector,
					UpperRightVector,
					LowerLeftVector,
					LowerRightVector
				};
	}
}