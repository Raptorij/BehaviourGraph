
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;

namespace RaptorijDevelop.BehaviourGraphs
{
    public class TransitionView : Edge
    {
        public Action<TransitionView> EdgeSelected;
        public Transition transition;
        readonly string edgeStyle = "UI/TransitionStyle";
        public Edge edge;

        public TransitionView(Transition transition)
        {
            this.transition = transition;
            this.viewDataKey = transition.guid;
            if (transition.parent is EnterNode)
            {
                this.SetEnabled(false);
            }
            styleSheets.Add(Resources.Load<StyleSheet>(edgeStyle));
            CreateEdgeControl();          
        }

		public override void OnSelected()
		{
			base.OnSelected();
            EdgeSelected?.Invoke(this);
        }
	}
}