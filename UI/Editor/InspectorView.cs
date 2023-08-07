using System;
using UnityEngine.UIElements;
using UnityEditor;

namespace RaptorijDevelop.BehaviourGraph
{
	public class InspectorView : VisualElement
    {
		public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

		Editor editor;
		private IMGUIContainer container;

		public InspectorView()
        {

		}

		public void UpdateSelection(TacticGraph graph)
		{
			Clear();
			if (graph != null)
			{
				UnityEngine.Object.DestroyImmediate(editor);
				editor = Editor.CreateEditor(graph/*, typeof(NodeEditor)*/);
				IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
				Add(container);
			}
		}

		public void UpdateSelection(NodeView nodeView)
		{
			Clear();
			if (nodeView != null)
			{
				UnityEngine.Object.DestroyImmediate(editor);
				if (nodeView.node is BehaviourNode tacticNode)
				{
					editor = Editor.CreateEditor(tacticNode, typeof(BehaviourNodeEditor));
				}
				else
				{
					editor = Editor.CreateEditor(nodeView.node, typeof(NodeEditor));
				}
				container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
				Add(container);
			}
			else
			{
				if (container != null && container.parent == this)
				{
					Remove(container);
				}
				editor = null;
			}
		}

		public void UpdateSelection(NoteNodeView nodeView)
		{
			Clear();
			if (nodeView != null)
			{
				UnityEngine.Object.DestroyImmediate(editor);
				editor = Editor.CreateEditor(nodeView.node, typeof(NoteNodeEditor));
				container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
				Add(container);
			}
			else
			{
				if (container != null && container.parent == this)
				{
					Remove(container);
				}
				editor = null;
			}
		}

		public void UpdateSelection(TransitionView transitionView)
		{
			Clear();
			if (transitionView != null)
			{
				UnityEngine.Object.DestroyImmediate(editor);
				editor = Editor.CreateEditor(transitionView.transition/*, typeof(NodeEditor)*/);
				IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
				Add(container);
			}
		}
	}
}