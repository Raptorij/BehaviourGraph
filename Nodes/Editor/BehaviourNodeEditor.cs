using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
#if UNITY_EDITOR
	[CustomEditor(typeof(BehaviourNode))]
	public class BehaviourNodeEditor : Editor
	{
		private Dictionary<NodeAbility, bool> showTypes = new Dictionary<NodeAbility, bool>();

		SerializedProperty transitions;
		ReorderableList list;

		private void OnEnable()
		{
			if (serializedObject == null)
				return;
			transitions = serializedObject.FindProperty("transitions");
			list = new ReorderableList(serializedObject, transitions, true, true, false, false);

			list.drawElementCallback = DrawListItems;
			list.drawHeaderCallback = DrawHeader;
			list.onSelectCallback = SelectListItem;
		}

		private void SelectListItem(ReorderableList list)
		{

		}

		void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
			var transition = element.objectReferenceValue as System.Object as Transition;
			var buttonRect = new Rect(rect.x, rect.y + 2, rect.width - 25, EditorGUIUtility.singleLineHeight);
			EditorGUI.LabelField(buttonRect, transition.name);
			buttonRect.x += rect.width - 25;
			buttonRect.width = 25;
			var icon = EditorGUIUtility.IconContent("SceneLoadIn");
			if (GUI.Button(buttonRect, icon))
			{
				BehaviourGraphEditorWindow wnd = EditorWindow.GetWindow<BehaviourGraphEditorWindow>();
				wnd.SelectElement(transition);
			}
		}

		void DrawHeader(Rect rect)
		{
			string name = "Transitions";
			EditorGUI.LabelField(rect, name);
		}

		public override void OnInspectorGUI()
		{
			BehaviourNode node = target as BehaviourNode;
			if (target == null)
			{
				return;
			}

			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			GUI.enabled = false;
			EditorGUILayout.TextField("GUID", node.guid);
			GUI.enabled = true;
			if (EditorGUI.EndChangeCheck())
			{
				node.name = node.nodeName;
				var transitions = node.graph.GetTransitions(node);
				for (int i = 0; i < transitions.Count; i++)
				{
					transitions[i].name = $"{node.name}=>{transitions[i].connection.name}";
				}
				node.NodeUpdated?.Invoke(node);
			}
			EditorGUILayout.Space();
			list.DoLayoutList();
			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Node Behaviour", GUILayout.Width(230), GUILayout.Height(23)))
			{
				var types = (typeof(NodeAbility)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(NodeAbility))).ToArray();
				List<string> typesNames = new List<string>();
				for (int i = 0; i < types.Length; i++)
				{
					AddComponentMenu addComponentMenu = (AddComponentMenu) Attribute.GetCustomAttribute(types[i], typeof(AddComponentMenu));
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
				searchProvider.Initialize("Node Behaviours", typesNames.ToArray(), OnTypeSelected);
				SearchWindow.Open(new SearchWindowContext(mouseRect), searchProvider);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			for (int i = 0; i < node.abilities.Count; i++)
			{
				if (node.abilities[i] == null)
				{
					node.abilities.RemoveAt(i);
					showTypes.Clear();
					i--;
					return;
				}
				DrawBehavior(node.abilities[i]);
			}
		}

		void AddMenuItemForBehaviour(GenericMenu menu, string menuPath, Type type)
		{
			menu.AddItem(new GUIContent(menuPath), false, OnTypeSelected, type);
		}

		private void OnTypeSelected(string typeName)
		{
			var types = (typeof(NodeAbility)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(NodeAbility))).ToList();
			var type = types.Find(x => x.Name == typeName);
			//if(type != null)
			OnTypeSelected(type);
		}

		private void OnTypeSelected(object type)
		{
			var selectedType = (Type)type;
			var behaviour = ScriptableObject.CreateInstance(selectedType) as NodeAbility;
			var node = (target as BehaviourNode);
			node.abilities.Add(behaviour);
			behaviour.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(behaviour, node);
			AssetDatabase.SaveAssets();
		}

		private void DrawBehavior(NodeAbility nodeBehaviour)
		{
			var editor = Editor.CreateEditor(nodeBehaviour);
			if (!showTypes.TryGetValue(nodeBehaviour, out var value))
			{
				showTypes.Add(nodeBehaviour, true);
			}
			nodeBehaviour.name = nodeBehaviour.GetType().Name;
			showTypes[nodeBehaviour] = EditorGUILayout.InspectorTitlebar(showTypes[nodeBehaviour], editor);
			if (showTypes[nodeBehaviour])
			{
				editor.OnInspectorGUI();
				EditorGUILayout.Space();
			}
		}
	}
#endif
}