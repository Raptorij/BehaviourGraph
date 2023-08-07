using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RaptorijDevelop.BehaviourGraph
{
	[CustomEditor(typeof(Transition))]
	public class TransitionEditor : Editor
	{
		private Dictionary<Condition, bool> showTypes = new Dictionary<Condition, bool>();

		private void OnEnable()
		{
		}
		void DrawHeader(Rect rect)
		{
			string name = "Conditions";
			EditorGUI.LabelField(rect, name);
		}

		public override void OnInspectorGUI()
		{
			var transition = target as Transition;
			if (target == null)
			{
				return;
			}
			base.OnInspectorGUI();
			GUI.enabled = false;
			EditorGUILayout.TextField("GUID", transition.guid);
			GUI.enabled = true;

			DrawNodeBehaviourControl(transition);
			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawNodeBehaviourControl(Transition transition)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Condition", GUILayout.Width(230), GUILayout.Height(23)))
			{
				var types = (typeof(Condition)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Condition))).ToArray();
				List<string> typesNames = new List<string>();
				for (int i = 0; i < types.Length; i++)
				{
					AddComponentMenu addComponentMenu = (AddComponentMenu)Attribute.GetCustomAttribute(types[i], typeof(AddComponentMenu));
					if (addComponentMenu != null)
					{
						var componentMenu = addComponentMenu.componentMenu;
						typesNames.Add(componentMenu);
					}
					else
					{
						typesNames.Add(types[i].Name);
					}
				}
				var mouseRect = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

				var searchProvider = ScriptableObject.CreateInstance<StringListSearchProvieder>();
				searchProvider.Initialize("Add Condition", typesNames.ToArray(), OnTypeSelected);
				SearchWindow.Open(new SearchWindowContext(mouseRect), searchProvider);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			for (int i = 0; i < transition.conditions.Count; i++)
			{
				if (transition.conditions[i] == null)
				{
					transition.conditions.RemoveAt(i);
					showTypes.Clear();
					i--;
					return;
				}
				DrawBehavior(transition.conditions[i]);
			}
		}

		private void OnTypeSelected(string typeName)
		{
			var types = (typeof(Condition)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Condition))).ToList();
			var type = types.Find(x => x.Name == typeName);
			//if(type != null)
			OnTypeSelected(type);
		}

		private void OnTypeSelected(object type)
		{
			var selectedType = (Type)type;
			var behaviour = ScriptableObject.CreateInstance(selectedType) as Condition;
			var node = (target as Transition);
			node.conditions.Add(behaviour);
			behaviour.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(behaviour, node);
			AssetDatabase.SaveAssets();
		}

		private void DrawBehavior(Condition condition)
		{
			var editor = Editor.CreateEditor(condition);
			if (!showTypes.TryGetValue(condition, out var value))
			{
				showTypes.Add(condition, true);
			}
			condition.name = condition.GetType().Name;
			showTypes[condition] = EditorGUILayout.InspectorTitlebar(showTypes[condition], editor);
			if (showTypes[condition])
			{
				editor.OnInspectorGUI();
				EditorGUILayout.Space();
			}
		}
	}
}