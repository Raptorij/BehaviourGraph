using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
    public class MiniMapView : MiniMap
    {
		new TacticGraphView graphView;
		Vector2 size;

		public MiniMapView(TacticGraphView baseGraphView)
		{
			this.graphView = baseGraphView;
			SetPosition(new Rect(Screen.width - 75, 0, 75, 75));
			this.anchored = true;
			size = new Vector2(75, 75);
		}
	}
}