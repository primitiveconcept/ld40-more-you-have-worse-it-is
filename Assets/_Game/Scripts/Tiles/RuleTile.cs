﻿namespace NanoLife
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using UnityEngine.Tilemaps;


	[Serializable]
	[CreateAssetMenu]
	public class RuleTile : TileBase
	{
		[SerializeField]
		private Sprite defaultSprite;

		[SerializeField]
		private Tile.ColliderType defaultColliderType = Tile.ColliderType.Sprite;

		[SerializeField]
		private List<TilingRule> tilingRules;


		#region Properties
		public Tile.ColliderType DefaultColliderType
		{
			get { return this.defaultColliderType; }
			set { this.defaultColliderType = value; }
		}


		public Sprite DefaultSprite
		{
			get { return this.defaultSprite; }
			set { this.defaultSprite = value; }
		}


		public List<TilingRule> TilingRules
		{
			get { return this.tilingRules; }
			set { this.tilingRules = value; }
		}
		#endregion


		public Vector3Int GetMirroredPos(Vector3Int original, bool mirrorX, bool mirrorY)
		{
			return new Vector3Int(original.x * (mirrorX ? -1 : 1), original.y * (mirrorY ? -1 : 1), original.z);
		}


		public Vector3Int GetRotatedPos(Vector3Int original, int rotation)
		{
			switch (rotation)
			{
				case 0:
					return original;
				case 90:
					return new Vector3Int(-original.y, original.x, original.z);
				case 180:
					return new Vector3Int(-original.x, -original.y, original.z);
				case 270:
					return new Vector3Int(original.y, -original.x, original.z);
			}
			return original;
		}


		public override bool GetTileAnimationData(
			Vector3Int position,
			ITilemap tilemap,
			ref TileAnimationData tileAnimationData)
		{
			if (this.tilingRules == null)
				return false;

			foreach (TilingRule rule in this.tilingRules)
			{
				Matrix4x4 transform = Matrix4x4.identity;
				if (RuleMatches(rule, position, tilemap, ref transform)
					&& rule.m_Output == TilingRule.OutputSprite.Animation)
				{
					tileAnimationData.animatedSprites = rule.m_Sprites;
					tileAnimationData.animationSpeed = rule.m_AnimationSpeed;
					return true;
				}
			}
			return false;
		}


		public override void GetTileData(Vector3Int position, ITilemap tileMap, ref TileData tileData)
		{
			tileData.sprite = this.DefaultSprite;
			tileData.colliderType = this.DefaultColliderType;
			tileData.flags = TileFlags.LockTransform;
			tileData.transform = Matrix4x4.identity;

			if (this.tilingRules == null)
				return;

			foreach (TilingRule rule in this.tilingRules)
			{
				Matrix4x4 transform = Matrix4x4.identity;
				if (RuleMatches(rule, position, tileMap, ref transform))
				{
					switch (rule.m_Output)
					{
						case TilingRule.OutputSprite.Single:
						case TilingRule.OutputSprite.Animation:
							tileData.sprite = rule.m_Sprites[0];
							break;
						case TilingRule.OutputSprite.Random:
							int index =
								Mathf.Clamp(
									Mathf.FloorToInt(GetPerlinValue(position, rule.m_PerlinScale, 100000f) * rule.m_Sprites.Length),
									0,
									rule.m_Sprites.Length - 1);
							tileData.sprite = rule.m_Sprites[index];
							if (rule.m_RandomTransform != TilingRule.Transform.Fixed)
								transform = ApplyRandomTransform(rule.m_RandomTransform, transform, rule.m_PerlinScale, position);
							break;
					}
					tileData.transform = transform;
					tileData.colliderType = rule.m_ColliderType;
					break;
				}
			}
		}


		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			if (this.tilingRules != null
				&& this.tilingRules.Count > 0)
			{
				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
					}
				}
			}
			else
			{
				base.RefreshTile(location, tileMap);
			}
		}


		public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
		{
			// Check rule against rotations of 0, 90, 180, 270
			for (int angle = 0; angle <= (rule.m_RuleTransform == TilingRule.Transform.Rotated ? 270 : 0); angle += 90)
			{
				if (RuleMatches(rule, position, tilemap, angle))
				{
					transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
					return true;
				}
			}

			// Check rule against x-axis mirror
			if ((rule.m_RuleTransform == TilingRule.Transform.MirrorX)
				&& RuleMatches(rule, position, tilemap, true, false))
			{
				transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
				return true;
			}

			// Check rule against y-axis mirror
			if ((rule.m_RuleTransform == TilingRule.Transform.MirrorY)
				&& RuleMatches(rule, position, tilemap, false, true))
			{
				transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
				return true;
			}

			return false;
		}


		public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle)
		{
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					if (x != 0
						|| y != 0)
					{
						Vector3Int offset = new Vector3Int(x, y, 0);
						Vector3Int rotated = GetRotatedPos(offset, angle);
						int index = GetIndexOfOffset(rotated);
						TileBase tile = tilemap.GetTile(position + offset);
						if (rule.m_Neighbors[index] == TilingRule.Neighbor.This && tile != this
							|| rule.m_Neighbors[index] == TilingRule.Neighbor.NotThis && tile == this)
						{
							return false;
						}
					}
				}
			}
			return true;
		}


		public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, bool mirrorX, bool mirrorY)
		{
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					if (x != 0
						|| y != 0)
					{
						Vector3Int offset = new Vector3Int(x, y, 0);
						Vector3Int mirrored = GetMirroredPos(offset, mirrorX, mirrorY);
						int index = GetIndexOfOffset(mirrored);
						TileBase tile = tilemap.GetTile(position + offset);
						if (rule.m_Neighbors[index] == TilingRule.Neighbor.This && tile != this
							|| rule.m_Neighbors[index] == TilingRule.Neighbor.NotThis && tile == this)
						{
							return false;
						}
					}
				}
			}

			return true;
		}


		#region Helper Methods
		private static Matrix4x4 ApplyRandomTransform(
			TilingRule.Transform type,
			Matrix4x4 original,
			float perlinScale,
			Vector3Int position)
		{
			float perlin = GetPerlinValue(position, perlinScale, 200000f);
			switch (type)
			{
				case TilingRule.Transform.MirrorX:
					return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(perlin < 0.5 ? 1f : -1f, 1f, 1f));
				case TilingRule.Transform.MirrorY:
					return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, perlin < 0.5 ? 1f : -1f, 1f));
				case TilingRule.Transform.Rotated:
					int angle = Mathf.Clamp(Mathf.FloorToInt(perlin * 4), 0, 3) * 90;
					return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
			}
			return original;
		}


		private static float GetPerlinValue(Vector3Int position, float scale, float offset)
		{
			return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
		}


		private int GetIndexOfOffset(Vector3Int offset)
		{
			int result = offset.x + 1 + (-offset.y + 1) * 3;
			if (result >= 4)
				result--;
			return result;
		}
		#endregion
	}
}