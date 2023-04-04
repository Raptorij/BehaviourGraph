using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;

namespace RaptorijDevelop.BehaviourGraphs
{
    public class BehaviourGraphEditorWindow : EditorWindow
    {
        BehaviourGraphView graphView;
        InspectorView inspectorView;
        Label graphPathLabel;
		private VisualElement contentViewContainer;
		private bool selectionIsNull;
        private bool isInitializedView;

        [MenuItem("Window/Behaviour Graph/Behaviour Graph Editor")]
        public static void OpenWindow()
        {
            BehaviourGraphEditorWindow wnd = GetWindow<BehaviourGraphEditorWindow>();
            var texture = Resources.Load<Texture>("Icons/AnimationGraphIcon");
            wnd.titleContent = new GUIContent("Behaviour Graph", texture);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviourGraphBase)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

		private void OnInspectorUpdate()
        {
            if (isInitializedView && graphView != null && graphView.graph != null)
            {
                graphView.graph.rect.position = contentViewContainer.transform.position;
                graphView.graph.rect.size = contentViewContainer.transform.scale;
            }
            graphView?.UpdateNodeStates();
        }

		public void CreateGUI()
        {
            try
            {
                VisualElement root = rootVisualElement;
                //TODO Find style
                //var visualTreePath = AssetDatabase.GetAssetPath();
                var visualTree = Resources.Load<VisualTreeAsset>("UI/TacticGraphView");
                visualTree.CloneTree(root);

                //TODO Find style
                var styleSheetPath = AssetDatabase.GetAssetPath(Resources.Load<StyleSheet>("UI/TacticGraphViewStyle"));
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
                root.styleSheets.Add(styleSheet);

                graphView = root.Q<BehaviourGraphView>();
                inspectorView = root.Q<InspectorView>();
                graphPathLabel = root.Q<Label>("graph-path-label");
                contentViewContainer = graphView.Q<VisualElement>("contentViewContainer");
                graphView.NodeSelected = OnNodeSelectionChange;
                graphView.NoteSelected = OnNoteSelectionChange;
                graphView.EdgeSelected = OnEdgeSelectionChange;
                graphView.GraphSelected = OnGraphSelectionChange;
                OnSelectionChange();
                graphView.Initialized += UpdateBackgroundView;
            }
            catch (Exception e)
            {
                BehaviourGraphEditorWindow wnd = GetWindow<BehaviourGraphEditorWindow>();
                if (wnd != null)
                {
                    wnd.Close();
                }
                Debug.LogException(e);
            }
        }

		private void UpdateBackgroundView()
        {
            contentViewContainer.style.left = graphView.graph.rect.position.x;
            //contentViewContainer.transform.scale = graphView.graph.rect.size;
            isInitializedView = true;
        }

		private void OnEnable()
		{
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        public void SelectElement(ScriptableObject scriptableObject)
        {
            graphView.SelectElement(scriptableObject);
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
			switch (obj)
			{
				case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
					break;
				case PlayModeStateChange.ExitingEditMode:
					break;
				case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
				case PlayModeStateChange.ExitingPlayMode:
					break;
			}
		}

        private void OnSelectionChange()
		{
            BehaviourGraphBase graph = Selection.activeObject as BehaviourGraphBase;
            if (!graph)
            {
                if (Selection.activeGameObject)
                {
                    BehaviourDirector director = Selection.activeGameObject.GetComponent<BehaviourDirector>();
                    if (director)
                    {
                        graph = director.BehaviourGraph;
                    }
                }                
            }
            if (Application.isPlaying)
            {
                if (graph)
                {
                    if (graphView != null)
                    {
                        graphView.PopulateView(graph);
                    }
                }
            }
            else
            {
                if (graph && AssetDatabase.CanOpenAssetInEditor(graph.GetInstanceID()))
                {                    
                    graphView.PopulateView(graph);
                    UpdateBackgroundView();
                }
                graphPathLabel.text = AssetDatabase.GetAssetPath(graphView.graph);
            }
        }

        private void OnGraphSelectionChange(BehaviourGraphBase obj)
        {
            if (selectionIsNull)
            {
                selectionIsNull = false;
                //inspectorView.UpdateSelection(obj);
            }
        }

        private void OnNodeSelectionChange(NodeView nodeView)
        {
            selectionIsNull = true;
            inspectorView.UpdateSelection(nodeView);
        }

        private void OnNoteSelectionChange(NoteNodeView obj)
        {
            selectionIsNull = true;
            inspectorView.UpdateSelection(obj);
        }

        private void OnEdgeSelectionChange(TransitionView transitionView)
        {
            selectionIsNull = true;
            inspectorView.UpdateSelection(transitionView);
        }
    }
}