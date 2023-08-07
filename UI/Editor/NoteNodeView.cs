using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RaptorijDevelop.BehaviourGraph
{
    public class NoteNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public System.Action<NoteNodeView> NodeSelected;
        public NoteNode node;
        private Label label;

        public NoteNodeView(NoteNode node) : base(AssetDatabase.GetAssetPath(Resources.Load("UI/NoteNodeView")))
        {
            this.node = node;
            //this.node.NodeUpdated = UpdateView;
            this.title = node.name;
            this.viewDataKey = node.guid;
            node.DateChanged += UpdateView;
            style.left = node.position.x;
            style.top = node.position.y;
            label = this.Q<Label>();
            label.text = node.Description;
            SetupClasses();
        }

		private void UpdateView(NoteNode node)
		{
            label.text = node.Description;
        }

		private void SetupClasses()
        {
            AddToClassList("note");
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
    }
}