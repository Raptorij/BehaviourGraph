using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;
using Unity.VisualScripting;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Reflection;

namespace RaptorijDevelop.BehaviourGraphs
{
    public class BehaviourGraphView : GraphView
    {
        public Action<NodeView> NodeSelected;
        public Action<NoteNodeView> NoteSelected;
        public Action<TransitionView> EdgeSelected;
        public Action<BehaviourGraphBase> GraphSelected;
        public new class UxmlFactory : UxmlFactory<BehaviourGraphView, GraphView.UxmlTraits> { }

        public BehaviourGraphBase graph;
		private Vector3 screenMousePosition;
        public event System.Action Initialized;

		public BehaviourGraphView()
        {
            Insert(0, new GridBackground());

            var zoomer = new ContentZoomer();
            this.AddManipulator(zoomer);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            //TODO Find style
            var styleSheet = Resources.Load<StyleSheet>("UI/TacticGraphViewStyle");
            styleSheets.Add(styleSheet);
            Initialized?.Invoke();
        }

		public override void HandleEvent(EventBase evt)
        {
            base.HandleEvent(evt);
            //var pv = 
            //graph.rect = 
        }

		private NodeView FindNodeView(Node node)
        {
            var graphNode = GetNodeByGuid(node.guid) as NodeView;
            return graphNode;
        }

        private TransitionView FindTransitionView(Transition transition)
        {
			foreach (var item in graphElements)
			{
                if (item.viewDataKey == transition.guid)
                {
                    return item as TransitionView;
                }
			}
            return null;
        }

        public void SelectElement(ScriptableObject scriptableObject)
        {
            if (scriptableObject is Transition transition)
            {
                var viewElement = FindTransitionView(transition);
                if (viewElement != null)
                {
                    viewElement.Select(this, false);
                }
            }
        }

		public void PopulateView(BehaviourGraphBase graph)
		{
            this.graph = graph;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (graph.rootNode == null)
            {
                graph.rootNode = graph.CreateNode((typeof(EnterNode))) as EnterNode;
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }

            this.graph.nodes.RemoveAll(n => n == null);
            this.graph.nodes.ForEach(n => graph.GetTransitions(n).RemoveAll(t => t == null));

            this.graph.nodes.ForEach(n => {
                if (n != null)
                {
                    if (n is NoteNode)
                    {
                        CreateNoteNodeView(n as NoteNode, false);
                    }
                    else
                    {
                        CreateNodeView(n, false);
                    }
                }
            });

            this.graph.nodes.ForEach(n =>
            {
                var transitions = graph.GetTransitions(n);
                NodeView parentView = FindNodeView(n);
                transitions.ForEach(c =>
                {
                    CreateTransitionView(c);
                });
            });
        }

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList().Where(endPort => 
            endPort.direction != startPort.direction && 
            endPort.node != startPort.node).ToList();
		}

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        graph.DeleteNode(nodeView.node);
                        NodeSelected?.Invoke(null);
                    }

                    NoteNodeView noteView = elem as NoteNodeView;
                    if (noteView != null)
                    {
                        graph.DeleteNode(noteView.node);
                        NoteSelected?.Invoke(null);
                    }

                    TransitionView edge = elem as TransitionView;
                    if (edge != null)
                    {
                        if (edge.edge != null)
                        {
                            RemoveElement(edge.edge);
                        }
                        if (edge.transition.parent is BehaviourNode tacticNode)
                        {
                            tacticNode.transitions.Remove(edge.transition);
                        }
                        if (edge.transition.parent is EnterNode enterNode)
                        {
                            enterNode.transition = null;
                        }
                        graph.RemoveTransition(edge.transition);
                        EdgeSelected?.Invoke(null);
                    }
                });
            }
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    edge.SetEnabled(false);
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView chieldView = edge.input.node as NodeView;
                    var transition = graph.AddConnection(parentView.node, chieldView.node);
                    var transitionView = new TransitionView(transition);
                    transitionView.viewDataKey = transition.guid;
                    transitionView.edge = edge;
                    transitionView.output = parentView.output;
                    transitionView.input = chieldView.input;
                    transitionView.output.AddToClassList("port-color");
                    transitionView.input.Connect(transitionView);
                    transitionView.output.Connect(transitionView);
                    var viewKey = transitionView.output.viewDataKey;
                    schedule.Execute(() => {
                        transitionView.UpdateEdgeControl();
                    }).ExecuteLater(1);

                    AddElement(transitionView);
                    transitionView.EdgeSelected = EdgeSelected;
                });
            }
            return graphViewChange;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
            //base.BuildContextualMenu(evt);

            var position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            screenMousePosition = position/* + evt.localMousePosition*/;
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
			foreach (var type in types)
			{
                if (!type.GetTypeInfo().IsAbstract)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, true));
                }
            }
            //evt.menu.AppendAction($"Ports Link", (a) => CreatePortsLink());
            evt.menu.AppendSeparator();
            evt.menu.AppendAction($"[Note Node] Note", (a) => CreateNoteNode(true));
		}

        void CreatePortsLink()
        {
            PortsLinkView portsLinkView = new PortsLinkView();
            AddElement(portsLinkView);
        }

        void CreateNode(System.Type type, bool setPosition = false)
        {
            Node node = graph.CreateNode(type);
            CreateNodeView(node, setPosition);
        }

		void CreateNodeView(Node node, bool setPosition)
        {
            NodeView nodeView = new NodeView(node);
            if (setPosition)
            {
                var rect = nodeView.GetPosition();
                rect.position = screenMousePosition;
                nodeView.SetPosition(rect);
            }
            nodeView.NodeSelected = NodeSelected;
            AddElement(nodeView);
        }

        void CreateNoteNode(bool setPosition = false)
        {
            NoteNode node = graph.CreateNode(typeof(NoteNode)) as NoteNode;
            CreateNoteNodeView(node, setPosition);
        }

        void CreateNoteNodeView(NoteNode node, bool setPosition)
        {
            NoteNodeView nodeView = new NoteNodeView(node);
            if (setPosition)
            {
                var rect = nodeView.GetPosition();
                rect.position = screenMousePosition;
                nodeView.SetPosition(rect);
            }
            nodeView.NodeSelected = NoteSelected;
            AddElement(nodeView);
        }

        void CreateTransitionView(Transition transition)
        {
            var transitionView = new TransitionView(transition);
            transitionView.viewDataKey = transition.guid;
            var parentView = FindNodeView(transition.parent);
            var chieldView = FindNodeView(transition.connection);
            transitionView.output = parentView.output;
            transitionView.input = chieldView.input;
            transitionView.input.Connect(transitionView);
            transitionView.output.Connect(transitionView);
            transitionView.EdgeSelected = EdgeSelected;
            AddElement(transitionView);
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView nodeView = n as NodeView;
                if (nodeView != null)
                {
                    nodeView.UpdateState();
				}
            });
        }
	}
}