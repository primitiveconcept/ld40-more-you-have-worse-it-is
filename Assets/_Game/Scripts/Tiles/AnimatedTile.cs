namespace NanoLife
{
	using System;
#if UNITY_EDITOR
	using UnityEditor;
#endif
	using UnityEngine;
	using UnityEngine.Tilemaps;


	[Serializable]
	public class AnimatedTile : TileBase
	{
		[SerializeField]
		private Sprite[] animatedSprites;

		[SerializeField]
		private float minSpeed = 1f;

		[SerializeField]
		private float maxSpeed = 1f;

		[SerializeField]
		private float animationStartTime;


		#region Properties
		public Sprite[] AnimatedSprites
		{
			get { return this.animatedSprites; }
			set { this.animatedSprites = value; }
		}


		public float AnimationStartTime
		{
			get { return this.animationStartTime; }
			set { this.animationStartTime = value; }
		}


		public float MaxSpeed
		{
			get { return this.maxSpeed; }
			set { this.maxSpeed = value; }
		}


		public float MinSpeed
		{
			get { return this.minSpeed; }
			set { this.minSpeed = value; }
		}
		#endregion


#if UNITY_EDITOR
		[MenuItem("Assets/Create/Animated Tile")]
		public static void CreateAnimatedTile()
		{
			string path = EditorUtility.SaveFilePanelInProject(
				"Save Animated Tile",
				"New Animated Tile",
				"asset",
				"Save Animated Tile",
				"Assets");
			if (path == "")
				return;

			AssetDatabase.CreateAsset(CreateInstance<AnimatedTile>(), path);
		}
#endif


		public override bool GetTileAnimationData(
			Vector3Int location,
			ITilemap tileMap,
			ref TileAnimationData tileAnimationData)
		{
			if (this.animatedSprites.Length > 0)
			{
				tileAnimationData.animatedSprites = this.animatedSprites;
				tileAnimationData.animationSpeed = UnityEngine.Random.Range(this.minSpeed, this.maxSpeed);
				tileAnimationData.animationStartTime = this.animationStartTime;
				return true;
			}
			return false;
		}


		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			tileData.transform = Matrix4x4.identity;
			tileData.color = Color.white;
			if (this.animatedSprites != null
				&& this.animatedSprites.Length > 0)
			{
				tileData.sprite = this.animatedSprites[this.animatedSprites.Length - 1];
			}
		}
	}
}