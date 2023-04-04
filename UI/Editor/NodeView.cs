using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RaptorijDevelop.BehaviourGraphs
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> NodeSelected;
        public Node node;
        public Port input;
        public Port output;
		private ProgressBar animationProgressBar;
        //"Assets/Scripts/AnimationDirector/UI/Editor/NodeView.uxml"
		public NodeView(Node node) : base (AssetDatabase.GetAssetPath(Resources.Load("UI/NodeView")))
        {
            this.node = node;
            this.node.NodeUpdated = UpdateView;
            this.title = node.name;
            if (node is BehaviourNode)
            {
                this.title = (node as BehaviourNode).nodeName;
            }
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;
            if (node is BehaviourNode)
            {
                animationProgressBar = this.Q<ProgressBar>();
            }
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

		private void SetupClasses()
        {
            if (node is ActionNode)
            {
                AddToClassList("animation");
            }
            else if (node is EnterNode)
            {
                AddToClassList("enter");
            }
        }

		private void UpdateView(Node obj)
        {
            if (node is BehaviourNode)
            {
                this.title = (node as BehaviourNode).nodeName;
            }
        }

		private void CreateOutputPorts()
		{
            if (node is ActionNode)
            {
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is EnterNode)
            {                

            }

            if (input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
		}

		private void CreateInputPorts()
        {
            if (node is ActionNode)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if (node is EnterNode)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
		}

		public override void OnSelected()
		{
			base.OnSelected();
            NodeSelected?.Invoke(this);

        }

        public void UpdateState()
        {
            if (Application.isPlaying)
            {
                RemoveFromClassList("running");
                RemoveFromClassList("failure");
                RemoveFromClassList("success");
                switch (node.state)
                {
                    case Node.State.Running:
                        if (node.started)
                        {
                            AddToClassList("running");
                        }
                        break;
                    case Node.State.Failure:
                        AddToClassList("failure");
                        break;
                    case Node.State.Success:
                        AddToClassList("success");
                        break;
                }
            }
		}
	}
}