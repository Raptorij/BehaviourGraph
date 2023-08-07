using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
	public class PortsLinkView : UnityEditor.Experimental.GraphView.Node
    {
        public Port input;
        public Port output;
        public PortsLinkView()
        {
            this.title = "";

            //style.left = node.position.x;
            //style.top = node.position.y;
            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateOutputPorts()
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            if (input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
        }

        private void CreateInputPorts()
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }
    }
}