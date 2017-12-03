
#if UNITY_EDITOR

namespace NanoLife
{
	using System;
	using UnityEditor;
	using UnityEngine;


	[CustomEditor(typeof(AnimatedTile))]
	public class AnimatedTileEditor : Editor
	{
		#region Properties
		private AnimatedTile tile
		{
			get { return (this.target as AnimatedTile); }
		}
		#endregion


		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			int count = EditorGUILayout.DelayedIntField(
				"Number of Animated Sprites",
				this.tile.AnimatedSprites != null ? this.tile.AnimatedSprites.Length : 0);
			if (count < 0)
				count = 0;

			if (this.tile.AnimatedSprites == null
				|| this.tile.AnimatedSprites.Length != count)
			{
				Sprite[] animatedSpriteArray = this.tile.AnimatedSprites;
				Array.Resize<Sprite>(ref animatedSpriteArray, count);
				this.tile.AnimatedSprites = animatedSpriteArray;
			}

			if (count == 0)
				return;

			EditorGUILayout.LabelField("Place sprites shown based on the order of animation.");
			EditorGUILayout.Space();

			for (int i = 0; i < count; i++)
			{
				this.tile.AnimatedSprites[i] =
					(Sprite)
					EditorGUILayout.ObjectField("Sprite " + (i + 1), this.tile.AnimatedSprites[i], typeof(Sprite), false, null);
			}

			float minSpeed = EditorGUILayout.FloatField("Minimum Speed", this.tile.MinSpeed);
			float maxSpeed = EditorGUILayout.FloatField("Maximum Speed", this.tile.MaxSpeed);
			if (minSpeed < 0.0f)
				minSpeed = 0.0f;

			if (maxSpeed < 0.0f)
				maxSpeed = 0.0f;

			if (maxSpeed < minSpeed)
				maxSpeed = minSpeed;

			this.tile.MinSpeed = minSpeed;
			this.tile.MaxSpeed = maxSpeed;

			this.tile.AnimationStartTime = EditorGUILayout.FloatField("Start Time", this.tile.AnimationStartTime);
			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(this.tile);
		}
	}
}

#endif