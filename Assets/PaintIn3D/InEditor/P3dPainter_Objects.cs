#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaintIn3D
{
	public partial class P3dPainter
	{
		class TempObj : P3dShaderTemplate.IHasTemplate
		{
			public bool        Dirty;
			public GameObject  Source;
			public int         Index;
			public P3dShaderTemplate Template;

			public void SetTemplate(P3dShaderTemplate template)
			{
				Template = template;
			}

			public P3dShaderTemplate GetTemplate()
			{
				return Template;
			}
		}

		private List<TempObj> tempObjs = new List<TempObj>();

		private List<P3dShaderTemplate> tempTemplates = new List<P3dShaderTemplate>();

		private TempObj GetTempObj(GameObject source, int index)
		{
			var tempObj = tempObjs.Find(t => t.Source == source && t.Index == index);

			if (tempObj != null)
			{
				tempObj.Dirty = false;
			}
			else
			{
				tempObj = new TempObj();

				tempObj.Source = source;
				tempObj.Index  = index;
			}

			return tempObj;
		}

		private bool DrawAddableObject(GameObject go, Material[] materials)
		{
			var add = false;

			GUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.ObjectField(go, typeof(GameObject), true);
				EditorGUI.EndDisabledGroup();

				tempTemplates.Clear();

				for (var i = 0; i < materials.Length; i++)
				{
					var material  = materials[i];
					var templates = P3dShaderTemplate.GetTemplates(material != null ? material.shader : null);
					var slot      = GetTempObj(go, i);

					if (templates.Contains(slot.Template) == false)
					{
						slot.Template = templates.Count > 0 ? templates[0] : null;
					}

					GUILayout.BeginVertical(EditorStyles.helpBox);
						P3dHelper.BeginLabelWidth(60.0f);
							EditorGUI.BeginDisabledGroup(true);
									EditorGUILayout.ObjectField("Material", material, typeof(Material), true);
							EditorGUI.EndDisabledGroup();
							P3dHelper.BeginColor(slot.Template == null);
								P3dShaderTemplate_Editor.DrawDropdown("Template", material, slot);
							P3dHelper.EndColor();
						P3dHelper.EndLabelWidth();
					GUILayout.EndVertical();

					tempTemplates.Add(slot.Template);
				}

				if (GUILayout.Button("Add") == true)
				{
					add = true;
				}
			GUILayout.EndVertical();

			return add;
		}

		private static HashSet<Transform> roots = new HashSet<Transform>();

		private void RunRoots(Transform t)
		{
			roots.Add(t);

			foreach (Transform child in t)
			{
				RunRoots(child);
			}
		}

		private static List<Vector3> bakedPositions = new List<Vector3>();

		private static void BakeMesh(SkinnedMeshRenderer skinnedMeshRenderer, ref Mesh bakedMesh, bool bakeScale)
		{
			if (bakedMesh == null)
			{
				bakedMesh = new Mesh();
			}

			var transform  = skinnedMeshRenderer.transform;
			var localScale = transform.localScale;

			transform.localScale = Vector3.one;

			skinnedMeshRenderer.BakeMesh(bakedMesh);

			transform.localScale = localScale;

			if (bakeScale == true)
			{
				bakedMesh.GetVertices(bakedPositions);

				var scale = P3dHelper.Reciprocal3(transform.lossyScale);

				for (var i = bakedPositions.Count - 1; i >= 0; i--)
				{
					var position = bakedPositions[i];

					position.x *= scale.x;
					position.y *= scale.y;
					position.z *= scale.z;

					bakedPositions[i] = position;
				}

				bakedMesh.SetVertices(bakedPositions);
			}

			bakedMesh.name = bakeScale == true ? "SCALED" : "UNSCALED";
		}

		private void UpdateObjectsPanel()
		{
			if (Selection.gameObjects.Length == 0 && scene.Objs.Count == 0)
			{
				EditorGUILayout.HelpBox("Select a GameObject with a MesFilter+MeshRenderer or SkinnedMeshRenderer.", MessageType.Info);
			}

			// Mark
			tempObjs.ForEach(t => t.Dirty = true);

			roots.Clear();

			foreach (var transform in Selection.transforms)
			{
				RunRoots(transform);
			}

			foreach (var root in roots)
			{
				if (scene.ObjExists(root) == false)
				{
					var mf  = root.GetComponent<MeshFilter>();
					var mr  = root.GetComponent<MeshRenderer>();
					var smr = root.GetComponent<SkinnedMeshRenderer>();

					if (mf != null && mr != null && mf.sharedMesh != null)
					{
						if (DrawAddableObject(root.gameObject, mr.sharedMaterials) == true)
						{
							scene.AddObj(root, mf.sharedMesh, root.position, root.rotation, root.lossyScale, mr.sharedMaterials, tempTemplates.ToArray(), settings.DefaultTextureSize);
						}
					}
					else if (smr != null && smr.sharedMesh != null)
					{
						if (DrawAddableObject(root.gameObject, smr.sharedMaterials) == true)
						{
							scene.AddObj(root, smr.sharedMesh, root.position, root.rotation, root.lossyScale, smr.sharedMaterials, tempTemplates.ToArray(), settings.DefaultTextureSize);
						}
					}
				}
			}

			// Sweep
			tempObjs.RemoveAll(t => t.Dirty == true);

			EditorGUILayout.Separator();

			for (var i = 0; i < scene.Objs.Count; i++)
			{
				if (i > 0) EditorGUILayout.Space();

				var obj     = scene.Objs[i];
				var objRect =
				EditorGUILayout.BeginVertical(GetSelectableStyle(obj == currentObj, true));
					P3dHelper.BeginLabelWidth(60.0f);
						if (obj == currentObj)
						{
							EditorGUILayout.BeginHorizontal();
								obj.Name = EditorGUILayout.TextField(obj.Name);
								if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)) == true && EditorUtility.DisplayDialog("Are you sure?", "This will delete the current layer from the paint window.", "Delete") == true)
								{
									scene.RemoveObj(obj); i--; P3dHelper.ClearControl();
								}
							EditorGUILayout.EndHorizontal();

							obj.Mesh      = (Mesh)EditorGUILayout.ObjectField("Mesh", obj.Mesh, typeof(Mesh), true);
							obj.Paintable = EditorGUILayout.Toggle("Paintable", obj.Paintable);
							obj.Coord     = (P3dCoord)EditorGUILayout.EnumPopup("Coord", obj.Coord);
							obj.Transform = (Transform)EditorGUILayout.ObjectField("Transform", obj.Transform, typeof(Transform), true);
							obj.Preview   = (Transform)EditorGUILayout.ObjectField("Preview", obj.Preview, typeof(Transform), true);

							if (obj.Transform == null)
							{
								obj.Position = EditorGUILayout.Vector3Field("Position", obj.Position);

								EditorGUI.BeginChangeCheck();
									var newRot = EditorGUILayout.Vector3Field("Rotation", obj.Rotation.eulerAngles);
								if (EditorGUI.EndChangeCheck() == true)
								{
									obj.Rotation = Quaternion.Euler(newRot);
								}

								obj.Scale = EditorGUILayout.Vector3Field("Scale", obj.Scale);
							}
							else
							{
								var smr = obj.Transform.GetComponent<SkinnedMeshRenderer>();

								if (smr != null && smr.sharedMesh != null)
								{
									if (obj.BakedMesh == null)
									{
										if (GUILayout.Button("Bake Pose") == true)
										{
											BakeMesh(smr, ref obj.BakedMesh, false);
										}

										if (GUILayout.Button("Bake Pose + Scale") == true)
										{
											BakeMesh(smr, ref obj.BakedMesh, true);
										}
									}
									else
									{
										if (GUILayout.Button("Update Pose") == true)
										{
											BakeMesh(smr, ref obj.BakedMesh, obj.BakedMesh.name == "SCALED");
										}

										if (GUILayout.Button("Reset Pose") == true)
										{
											DestroyImmediate(obj.BakedMesh);

											obj.BakedMesh = null;
										}
									}
								}
							}

							if (GUILayout.Button("Center Camera") == true)
							{
								cameraOrigin = obj.Position;
							}

							EditorGUILayout.Separator();

							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
								if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)) == true)
								{
									obj.MatIds.Add(-1);
								}
							EditorGUILayout.EndHorizontal();

							for (var j = 0; j < obj.MatIds.Count; j++)
							{
								var matId = obj.MatIds[j];
								var rect  = P3dHelper.Reserve(); rect.xMin += 10;
								var mat   = scene.GetMat(matId);

								if (GUI.Button(rect, mat != null ? mat.Name : "", EditorStyles.popup) == true)
								{
									var menu = new GenericMenu();

									for (var k = 0; k < scene.Mats.Count; k++)
									{
										var setObj = obj;
										var setIdx = j;
										var setMat = scene.Mats[k];

										menu.AddItem(new GUIContent(setMat.Name), setMat == mat, () => setObj.MatIds[setIdx] = setMat.Id);
									}
										
									var remObj = obj;
									var remIdx = j;

									menu.AddSeparator("");
									menu.AddItem(new GUIContent("Remove"), false, () => remObj.MatIds.RemoveAt(remIdx));

									menu.DropDown(rect);
								}
							}
						}
						else
						{
							EditorGUILayout.LabelField(obj.Name);
						}
					P3dHelper.EndLabelWidth();
				EditorGUILayout.EndVertical();

				if (Event.current.type == EventType.MouseDown && objRect.Contains(Event.current.mousePosition) == true)
				{
					currentObj = obj; P3dHelper.ClearControl();
				}
			}
		}
	}
}
#endif