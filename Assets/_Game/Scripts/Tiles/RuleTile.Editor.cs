#if UNITY_EDITOR
namespace NanoLife
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using Object = UnityEngine.Object;


	[CustomEditor(typeof(RuleTile))]
	[CanEditMultipleObjects]
	internal class RuleTileEditor : Editor
	{
		const float DefaultElementHeight = 48f;
		const float LabelWidth = 53f;
		const float PaddingBetweenRules = 13f;
		const float SingleLineHeight = 16f;
		private const string Arrow0 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPzZExDoQwDATzE4oU4QXXcgUFj+YxtETwgpMwXuFcwMFSRMVKKwzZcWzhiMg91jtg34XIntkre5EaT7yjjhI9pOD5Mw5k2X/DdUwFr3cQ7Pu23E/BiwXyWSOxrNqx+ewnsayam5OLBtbOGPUM/r93YZL4/dhpR/amwByGFBz170gNChA6w5bQQMqramBTgJ+Z3A58WuWejPCaHQAAAABJRU5ErkJggg==";
		private const string Arrow1 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPxYzBDYAgEATpxYcd+PVr0fZ2siZrjmMhFz6STIiDs8XMlpEyi5RkO/d66TcgJUB43JfNBqRkSEYDnYjhbKD5GIUkDqRDwoH3+NgTAw+bL/aoOP4DOgH+iwECEt+IlFmkzGHlAYKAWF9R8zUnAAAAAElFTkSuQmCC";
		private const string Arrow2 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAC0SURBVDhPjVE5EsIwDMxPKFKYF9CagoJH8xhaMskLmEGsjOSRkBzYmU2s9a58TUQUmCH1BWEHweuKP+D8tphrWcAHuIGrjPnPNY8X2+DzEWE+FzrdrkNyg2YGNNfRGlyOaZDJOxBrDhgOowaYW8UW0Vau5ZkFmXbbDr+CzOHKmLinAXMEePyZ9dZkZR+s5QX2O8DY3zZ/sgYcdDqeEVp8516o0QQV1qeMwg6C91toYoLoo+kNt/tpKQEVvFQAAAAASUVORK5CYII=";
		private const string Arrow3 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAB2SURBVDhPzY1LCoAwEEPnLi48gW5d6p31bH5SMhp0Cq0g+CCLxrzRPqMZ2pRqKG4IqzJc7JepTlbRZXYpWTg4RZE1XAso8VHFKNhQuTjKtZvHUNCEMogO4K3BhvMn9wP4EzoPZ3n0AGTW5fiBVzLAAYTP32C2Ay3agtu9V/9PAAAAAElFTkSuQmCC";
		private const string Arrow5 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPnY3BCYBADASvFx924NevRdvbyoLBmNuDJQMDGjNxAFhK1DyUQ9fvobCdO+j7+sOKj/uSB+xYHZAxl7IR1wNTXJeVcaAVU+614uWfCT9mVUhknMlxDokd15BYsQrJFHeUQ0+MB5ErsPi/6hO1AAAAAElFTkSuQmCC";
		private const string Arrow6 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACaSURBVDhPxZExEkAwEEVzE4UiTqClUDi0w2hlOIEZsV82xCZmQuPPfFn8t1mirLWf7S5flQOXjd64vCuEKWTKVt+6AayH3tIa7yLg6Qh2FcKFB72jBgJeziA1CMHzeaNHjkfwnAK86f3KUafU2ClHIJSzs/8HHLv09M3SaMCxS7ljw/IYJWzQABOQZ66x4h614ahTCL/WT7BSO51b5Z5hSx88AAAAAElFTkSuQmCC";
		private const string Arrow7 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABQSURBVDhPYxh8QNle/T8U/4MKEQdAmsz2eICx6W530gygr2aQBmSMphkZYxqErAEXxusKfAYQ7XyyNMIAsgEkaYQBkAFkaYQBsjXSGDAwAAD193z4luKPrAAAAABJRU5ErkJggg==";
		private const string Arrow8 =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPxZE9DoAwCIW9iUOHegJXHRw8tIdx1egJTMSHAeMPaHSR5KVQ+KCkCRF91mdz4VDEWVzXTBgg5U1N5wahjHzXS3iFFVRxAygNVaZxJ6VHGIl2D6oUXP0ijlJuTp724FnID1Lq7uw2QM5+thoKth0N+GGyA7IA3+yM77Ag1e2zkey5gCdAg/h8csy+/89v7E+YkgUntOWeVt2SfAAAAABJRU5ErkJggg==";
		private const string MirrorX =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
		private const string MirrorY =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
		private const string Rotated =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";
		private const string XIconString =
			"iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABoSURBVDhPnY3BDcAgDAOZhS14dP1O0x2C/LBEgiNSHvfwyZabmV0jZRUpq2zi6f0DJwdcQOEdwwDLypF0zHLMa9+NQRxkQ+ACOT2STVw/q8eY1346ZlE54sYAhVhSDrjwFymrSFnD2gTZpls2OvFUHAAAAABJRU5ErkJggg==";

		private static Texture2D[] arrows;
		private static Texture2D[] autoTransforms;

		private ReorderableList reorderableList;
		private Rect listRect;


		#region Properties
		public static Texture2D[] Arrows
		{
			get
			{
				if (arrows == null)
				{
					arrows = new Texture2D[10];
					arrows[0] = Base64ToTexture(Arrow0);
					arrows[1] = Base64ToTexture(Arrow1);
					arrows[2] = Base64ToTexture(Arrow2);
					arrows[3] = Base64ToTexture(Arrow3);
					arrows[5] = Base64ToTexture(Arrow5);
					arrows[6] = Base64ToTexture(Arrow6);
					arrows[7] = Base64ToTexture(Arrow7);
					arrows[8] = Base64ToTexture(Arrow8);
					arrows[9] = Base64ToTexture(XIconString);
				}
				return arrows;
			}
		}


		public static Texture2D[] AutoTransforms
		{
			get
			{
				if (autoTransforms == null)
				{
					autoTransforms = new Texture2D[3];
					autoTransforms[0] = Base64ToTexture(Rotated);
					autoTransforms[1] = Base64ToTexture(MirrorX);
					autoTransforms[2] = Base64ToTexture(MirrorY);
				}
				return autoTransforms;
			}
		}


		private RuleTile tile
		{
			get { return (this.target as RuleTile); }
		}
		#endregion


		public void OnEnable()
		{
			if (this.tile.TilingRules == null)
				this.tile.TilingRules = new List<RuleTile.TilingRule>();

			this.reorderableList = new ReorderableList(
				this.tile.TilingRules,
				typeof(RuleTile.TilingRule),
				true,
				true,
				true,
				true);
			this.reorderableList.drawHeaderCallback = OnDrawHeader;
			this.reorderableList.drawElementCallback = OnDrawElement;
			this.reorderableList.elementHeightCallback = GetElementHeight;
			this.reorderableList.onReorderCallback = ListUpdated;
		}


		public override void OnInspectorGUI()
		{
			this.tile.DefaultSprite =
				EditorGUILayout.ObjectField("Default Sprite", this.tile.DefaultSprite, typeof(Sprite), false) as Sprite;
			this.tile.DefaultColliderType =
				(Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", this.tile.DefaultColliderType);
			EditorGUILayout.Space();

			if (this.reorderableList != null
				&& this.tile.TilingRules != null)
				this.reorderableList.DoLayoutList();
		}


		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			if (this.tile.DefaultSprite != null)
			{
				Type t = GetType("UnityEditor.SpriteUtility");
				if (t != null)
				{
					MethodInfo method = t.GetMethod(
						"RenderStaticPreview",
						new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
					if (method != null)
					{
						object ret = method.Invoke(
							"RenderStaticPreview",
							new object[] { this.tile.DefaultSprite, Color.white, width, height });
						if (ret is Texture2D)
							return ret as Texture2D;
					}
				}
			}
			return base.RenderStaticPreview(assetPath, subAssets, width, height);
		}


		#region Helper Methods
		private static Texture2D Base64ToTexture(string base64)
		{
			Texture2D t = new Texture2D(1, 1);
			t.hideFlags = HideFlags.HideAndDontSave;
			t.LoadImage(Convert.FromBase64String(base64));
			return t;
		}


		private static Type GetType(string TypeName)
		{
			Type type = Type.GetType(TypeName);
			if (type != null)
				return type;

			if (TypeName.Contains("."))
			{
				string assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
				Assembly assembly = Assembly.Load(assemblyName);
				if (assembly == null)
					return null;
				type = assembly.GetType(TypeName);
				if (type != null)
					return type;
			}

			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			AssemblyName[] referencedAssemblies = currentAssembly.GetReferencedAssemblies();
			foreach (AssemblyName assemblyName in referencedAssemblies)
			{
				Assembly assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					type = assembly.GetType(TypeName);
					if (type != null)
						return type;
				}
			}
			return null;
		}


		private static void OnSelect(object userdata)
		{
			MenuItemData data = (MenuItemData)userdata;
			data.m_Rule.m_RuleTransform = data.m_NewValue;
		}


		private static void RuleInspectorOnGUI(Rect rect, RuleTile.TilingRule tilingRule)
		{
			float y = rect.yMin;
			EditorGUI.BeginChangeCheck();
			GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Rule");
			tilingRule.m_RuleTransform =
				(RuleTile.TilingRule.Transform)
				EditorGUI.EnumPopup(
					new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
					tilingRule.m_RuleTransform);
			y += SingleLineHeight;
			GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Collider");
			tilingRule.m_ColliderType =
				(Tile.ColliderType)
				EditorGUI.EnumPopup(
					new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
					tilingRule.m_ColliderType);
			y += SingleLineHeight;
			GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Output");
			tilingRule.m_Output =
				(RuleTile.TilingRule.OutputSprite)
				EditorGUI.EnumPopup(
					new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
					tilingRule.m_Output);
			y += SingleLineHeight;

			if (tilingRule.m_Output == RuleTile.TilingRule.OutputSprite.Animation)
			{
				GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Speed");
				tilingRule.m_AnimationSpeed =
					EditorGUI.FloatField(
						new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
						tilingRule.m_AnimationSpeed);
				y += SingleLineHeight;
			}
			if (tilingRule.m_Output == RuleTile.TilingRule.OutputSprite.Random)
			{
				GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Noise");
				tilingRule.m_PerlinScale =
					EditorGUI.Slider(
						new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
						tilingRule.m_PerlinScale,
						0.001f,
						0.999f);
				y += SingleLineHeight;

				GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Shuffle");
				tilingRule.m_RandomTransform =
					(RuleTile.TilingRule.Transform)
					EditorGUI.EnumPopup(
						new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
						tilingRule.m_RandomTransform);
				y += SingleLineHeight;
			}

			if (tilingRule.m_Output != RuleTile.TilingRule.OutputSprite.Single)
			{
				GUI.Label(new Rect(rect.xMin, y, LabelWidth, SingleLineHeight), "Size");
				EditorGUI.BeginChangeCheck();
				int newLength =
					EditorGUI.DelayedIntField(
						new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
						tilingRule.m_Sprites.Length);
				if (EditorGUI.EndChangeCheck())
					Array.Resize(ref tilingRule.m_Sprites, Math.Max(newLength, 1));
				y += SingleLineHeight;

				for (int i = 0; i < tilingRule.m_Sprites.Length; i++)
				{
					tilingRule.m_Sprites[i] =
						EditorGUI.ObjectField(
							new Rect(rect.xMin + LabelWidth, y, rect.width - LabelWidth, SingleLineHeight),
							tilingRule.m_Sprites[i],
							typeof(Sprite),
							false) as Sprite;
					y += SingleLineHeight;
				}
			}
		}


		private static void RuleMatrixOnGUI(Rect rect, RuleTile.TilingRule tilingRule)
		{
			Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
			int index = 0;
			float w = rect.width / 3f;
			float h = rect.height / 3f;

			for (int y = 0; y <= 3; y++)
			{
				float top = rect.yMin + y * h;
				Handles.DrawLine(new Vector3(rect.xMin, top), new Vector3(rect.xMax, top));
			}
			for (int x = 0; x <= 3; x++)
			{
				float left = rect.xMin + x * w;
				Handles.DrawLine(new Vector3(left, rect.yMin), new Vector3(left, rect.yMax));
			}
			Handles.color = Color.white;

			for (int y = 0; y <= 2; y++)
			{
				for (int x = 0; x <= 2; x++)
				{
					Rect r = new Rect(rect.xMin + x * w, rect.yMin + y * h, w - 1, h - 1);
					if (x != 1
						|| y != 1)
					{
						switch (tilingRule.m_Neighbors[index])
						{
							case RuleTile.TilingRule.Neighbor.This:
								GUI.DrawTexture(r, Arrows[y * 3 + x]);
								break;
							case RuleTile.TilingRule.Neighbor.NotThis:
								GUI.DrawTexture(r, Arrows[9]);
								break;
						}
						if (Event.current.type == EventType.MouseDown
							&& r.Contains(Event.current.mousePosition))
						{
							tilingRule.m_Neighbors[index] = (RuleTile.TilingRule.Neighbor)(((int)tilingRule.m_Neighbors[index] + 1) % 3);
							GUI.changed = true;
							Event.current.Use();
						}

						index++;
					}
					else
					{
						switch (tilingRule.m_RuleTransform)
						{
							case RuleTile.TilingRule.Transform.Rotated:
								GUI.DrawTexture(r, AutoTransforms[0]);
								break;
							case RuleTile.TilingRule.Transform.MirrorX:
								GUI.DrawTexture(r, AutoTransforms[1]);
								break;
							case RuleTile.TilingRule.Transform.MirrorY:
								GUI.DrawTexture(r, AutoTransforms[2]);
								break;
						}

						if (Event.current.type == EventType.MouseDown
							&& r.Contains(Event.current.mousePosition))
						{
							tilingRule.m_RuleTransform = (RuleTile.TilingRule.Transform)(((int)tilingRule.m_RuleTransform + 1) % 4);
							GUI.changed = true;
							Event.current.Use();
						}
					}
				}
			}
		}


		private float GetElementHeight(int index)
		{
			if (this.tile.TilingRules != null
				&& this.tile.TilingRules.Count > 0)
			{
				switch (this.tile.TilingRules[index].m_Output)
				{
					case RuleTile.TilingRule.OutputSprite.Random:
						return DefaultElementHeight + SingleLineHeight * (this.tile.TilingRules[index].m_Sprites.Length + 3)
								+ PaddingBetweenRules;
					case RuleTile.TilingRule.OutputSprite.Animation:
						return DefaultElementHeight + SingleLineHeight * (this.tile.TilingRules[index].m_Sprites.Length + 2)
								+ PaddingBetweenRules;
				}
			}
			return DefaultElementHeight + PaddingBetweenRules;
		}


		private void ListUpdated(ReorderableList list)
		{
			SaveTile();
		}


		private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
		{
			RuleTile.TilingRule rule = this.tile.TilingRules[index];

			float yPos = rect.yMin + 2f;
			float height = rect.height - PaddingBetweenRules;
			float matrixWidth = DefaultElementHeight;

			Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
			Rect matrixRect = new Rect(rect.xMax - matrixWidth * 2f - 10f, yPos, matrixWidth, DefaultElementHeight);
			Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, DefaultElementHeight);

			EditorGUI.BeginChangeCheck();
			RuleInspectorOnGUI(inspectorRect, rule);
			RuleMatrixOnGUI(matrixRect, rule);
			SpriteOnGUI(spriteRect, rule);
			if (EditorGUI.EndChangeCheck())
				SaveTile();
		}


		private void OnDrawHeader(Rect rect)
		{
			GUI.Label(rect, "Tiling Rules");
		}


		private void SaveTile()
		{
			EditorUtility.SetDirty(this.target);
			SceneView.RepaintAll();
		}


		private void SpriteOnGUI(Rect rect, RuleTile.TilingRule tilingRule)
		{
			tilingRule.m_Sprites[0] =
				EditorGUI.ObjectField(
					new Rect(rect.xMax - rect.height, rect.yMin, rect.height, rect.height),
					tilingRule.m_Sprites[0],
					typeof(Sprite),
					false) as Sprite;
		}
		#endregion


		private class MenuItemData
		{
			public RuleTile.TilingRule m_Rule;
			public RuleTile.TilingRule.Transform m_NewValue;


			#region Constructors
			public MenuItemData(RuleTile.TilingRule mRule, RuleTile.TilingRule.Transform mNewValue)
			{
				this.m_Rule = mRule;
				this.m_NewValue = mNewValue;
			}
			#endregion
		}
	}
}
#endif