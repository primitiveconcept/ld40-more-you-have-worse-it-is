namespace NanoLife
{
	using System;
	using UnityEngine;
	using UnityEngine.Tilemaps;


	[Serializable]
	public class TilingRule
	{
		public Neighbor[] m_Neighbors;
		public Sprite[] m_Sprites;
		public float m_AnimationSpeed;
		public float m_PerlinScale;
		public Transform m_RuleTransform;
		public OutputSprite m_Output;
		public Tile.ColliderType m_ColliderType;
		public Transform m_RandomTransform;


		#region Constructors
		public TilingRule()
		{
			this.m_Output = OutputSprite.Single;
			this.m_Neighbors = new Neighbor[8];
			this.m_Sprites = new Sprite[1];
			this.m_AnimationSpeed = 1f;
			this.m_PerlinScale = 0.5f;
			this.m_ColliderType = Tile.ColliderType.Sprite;

			for (int i = 0; i < this.m_Neighbors.Length; i++)
				this.m_Neighbors[i] = Neighbor.DontCare;
		}
		#endregion


		public enum Neighbor
		{
			DontCare,
			This,
			NotThis
		}


		public enum OutputSprite
		{
			Single,
			Random,
			Animation
		}


		public enum Transform
		{
			Fixed,
			Rotated,
			MirrorX,
			MirrorY
		}
	}
}
