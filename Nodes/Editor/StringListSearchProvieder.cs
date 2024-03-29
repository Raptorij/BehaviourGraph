using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RaptorijDevelop
{
	public class StringListSearchProvieder : ScriptableObject, ISearchWindowProvider
	{
		private string[] listItems;
		private Texture2D indentationIcon;
		private Action<string> OnSetIndexCallback;
		private string title;

		public void Initialize(string title, string[] items, Action<string> callback)
		{
			listItems = items;
			this.title = title;
			OnSetIndexCallback = callback;

			indentationIcon = new Texture2D(1,1);
			indentationIcon.SetPixel(0,0, Color.clear);
			indentationIcon.Apply();
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var searchList = new List<SearchTreeEntry>();
			searchList.Add(new SearchTreeGroupEntry(new GUIContent(title), 0));

			List<string> sortedListItems = listItems.ToList();
			sortedListItems.Sort((a, b) =>
			{
				string[] splits1 = a.Split('/');
				string[] splits2 = b.Split('/');
				for (int i = 0; i < splits1.Length; i++)
				{
					if (i >= splits2.Length)
					{
						return 1;
					}
					int value = splits1[i].CompareTo(splits2[i]);
					if (value != 0)
					{
						if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
						{
							return splits1.Length < splits2.Length ? 1 : -1;
						}
						return value;
					}
				}
				return 0;
			});


			List<string> groups = new List<string>();
			foreach (var item in sortedListItems)
			{
				string[] entryTitle = item.Split('/');
				string groupName = "";
				for (int i = 0; i < entryTitle.Length - 1; i++)
				{
					groupName += entryTitle[i];
					if (!groups.Contains(groupName))
					{
						searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
						groups.Add(groupName);
					}
					groupName += "/";
				}
				SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last(), indentationIcon));
				entry.level = entryTitle.Length;
				entry.userData = entryTitle.Last();
				searchList.Add(entry);
			}

			return searchList;
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			OnSetIndexCallback?.Invoke((string)SearchTreeEntry.userData);
			ScriptableObject.DestroyImmediate(this);
			return true;
		}
	}
}