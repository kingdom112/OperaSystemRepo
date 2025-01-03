﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.UIControls
{
    public class TreeViewState : ScriptableObject
    {
        public List<DataItem> Items;
    }

    public class DataItem
    {
        public string Name;

        public DataItem Parent;

        public List<DataItem> Children;

        public DataItem()
        { }

        public DataItem(string name)
        {
            Name = name;
            Children = new List<DataItem>();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// In this demo we use custom data items hierarchy as data source 
    /// </summary>
    public class VirtualizingTreeViewDemo_DataItems : MonoBehaviour
    {
        public VirtualizingTreeView TreeView;

        private List<DataItem> m_dataItems;

        /*
        private IProject m_project;*/

        //private IEnumerator Start()
        private void Start()
        {
            /*m_project = IOC.Resolve<IProject>();
            yield return m_project.OpenProject("Test");*/

            TreeView.ItemDataBinding += OnItemDataBinding;
            TreeView.SelectionChanged += OnSelectionChanged;
            TreeView.ItemsRemoved += OnItemsRemoved;
            TreeView.ItemExpanding += OnItemExpanding;
            TreeView.ItemBeginDrag += OnItemBeginDrag;

            TreeView.ItemDrop += OnItemDrop;
            TreeView.ItemBeginDrop += OnItemBeginDrop;
            TreeView.ItemEndDrag += OnItemEndDrag;

            m_dataItems = new List<DataItem>();
            for (int i = 0; i < 100; ++i)
            {
                DataItem dataItem = new DataItem("DataItem " + i);
                m_dataItems.Add(dataItem);
            }

            TreeView.Items = m_dataItems;

            if(m_buttons != null)
            {
                m_buttons.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            TreeView.ItemDataBinding -= OnItemDataBinding;
            TreeView.SelectionChanged -= OnSelectionChanged;
            TreeView.ItemsRemoved -= OnItemsRemoved;
            TreeView.ItemExpanding -= OnItemExpanding;
            TreeView.ItemBeginDrag -= OnItemBeginDrag;
            TreeView.ItemBeginDrop -= OnItemBeginDrop;
            TreeView.ItemDrop -= OnItemDrop;
            TreeView.ItemEndDrag -= OnItemEndDrag;
        }

        private void OnItemExpanding(object sender, VirtualizingItemExpandingArgs e)
        {
            //get parent data item (game object in our case)
            DataItem dataItem = (DataItem)e.Item;
            if (dataItem.Children.Count > 0)
            {
                //Populate children collection
                e.Children = dataItem.Children;
            }

        }

        private void OnSelectionChanged(object sender, SelectionChangedArgs e)
        {
            #if UNITY_EDITOR
            //Do something on selection changed (just syncronized with editor's hierarchy for demo purposes)
           // UnityEditor.Selection.objects = e.NewItems.OfType<GameObject>().ToArray();
            #endif

            if(m_buttons != null)
            {
                m_buttons.SetActive(TreeView.SelectedItem != null);
            }
        }

        private void OnItemsRemoved(object sender, ItemsRemovedArgs e)
        {
            //Destroy removed dataitems
            for (int i = 0; i < e.Items.Length; ++i)
            {
                DataItem dataItem = (DataItem)e.Items[i];
                if(dataItem.Parent != null)
                {
                    dataItem.Parent.Children.Remove(dataItem);
                }
                m_dataItems.Remove(dataItem);
            }
        }

        /// <summary>
        /// This method called for each data item during databinding operation
        /// You have to bind data item properties to ui elements in order to display them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemDataBinding(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            DataItem dataItem = e.Item as DataItem;
            if (dataItem != null)
            {
                //We display dataItem.name using UI.Text 
                TextMeshProUGUI text = e.ItemPresenter.GetComponentInChildren<TextMeshProUGUI>(true);
                text.text = dataItem.Name;

                //Load icon from resources
                Image icon = e.ItemPresenter.GetComponentsInChildren<Image>()[4];
                icon.sprite = Resources.Load<Sprite>("IconNew");

                //And specify whether data item has children (to display expander arrow if needed)

                e.HasChildren = dataItem.Children.Count > 0;
            }
        }

        private void OnItemBeginDrop(object sender, ItemDropCancelArgs e)
        {

        }

        private void OnItemBeginDrag(object sender, ItemArgs e)
        {
            //Could be used to change cursor
        }

        private void OnItemEndDrag(object sender, ItemArgs e)
        {
        }


        private List<DataItem> ChildrenOf(DataItem parent)
        {
            if(parent == null)
            {
                return m_dataItems;
            }
            return parent.Children;
        }

        private void OnItemDrop(object sender, ItemDropArgs args)
        {
            if(args.DropTarget == null)
            {
                return;
            }

            TreeView.ItemDropStdHandler<DataItem>(args,
                (item) => item.Parent,
                (item, parent) => item.Parent = parent,
                (item, parent) => ChildrenOf(parent).IndexOf(item),
                (item, parent) => ChildrenOf(parent).Remove(item),
                (item, parent, i) => ChildrenOf(parent).Insert(i, item),
                (item, parent) => ChildrenOf(parent).Add(item));
        }

        [SerializeField]
        private GameObject m_buttons = null;
        private int m_counter = 0;

        public void ScrollIntoView()
        {
            TreeView.ScrollIntoView(TreeView.SelectedItem);
        }

        public void Add()
        {
            foreach (DataItem parent in TreeView.SelectedItems)
            {
                DataItem item = new DataItem("New Item");
                parent.Children.Add(item);
                item.Parent = parent;

                TreeView.AddChild(parent, item);
                TreeView.Expand(parent);

                DataItem subItem = new DataItem("New Sub Item");
                item.Children.Add(subItem);
                subItem.Parent = item;

                TreeView.AddChild(item, subItem);
                TreeView.Expand(item);

                m_counter++;
            }
        }

        public void Remove()
        {
            foreach (DataItem selectedItem in TreeView.SelectedItems.OfType<object>().ToArray())
            {
                TreeView.RemoveChild(selectedItem.Parent, selectedItem);                
            }
        }

        public void Collapse()
        {
            foreach (DataItem selectedItem in TreeView.SelectedItems)
            {
                TreeView.Collapse(selectedItem);
            }
        }

        public void Expand()
        {
            foreach (DataItem selectedItem in TreeView.SelectedItems)
            {
                TreeView.ExpandAll(selectedItem, item => item.Parent, item => item.Children);
            }
        }

        /*
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(Save());
            }

            if (Input.GetKey(KeyCode.L))
            {
                StartCoroutine(Load());
            }
        }

        private IEnumerator Save()
        {
            TreeViewState state = ScriptableObject.CreateInstance<TreeViewState>();
            state.Items = m_dataItems;

            yield return m_project.SetValue("TreeViewState", state);
            Destroy(state);
        }

        private IEnumerator Load()
        {
            ProjectAsyncOperation<TreeViewState> getValue = m_project.GetValue<TreeViewState>("TreeViewState");
            yield return getValue;

            if (getValue.Error.HasError)
            {
                Debug.LogError(getValue.Error.ToString());
            }
            else
            {
                m_dataItems = getValue.Result.Items;
                for (int i = 0; i < m_dataItems.Count; ++i)
                {
                    SetParents(m_dataItems[i]);
                }
                TreeView.Items = m_dataItems;

                foreach (DataItem dataItem in m_dataItems)
                {
                    TreeView.ExpandAll(dataItem, item => item.Parent, item => item.Children);
                }

                Destroy(getValue.Result);
            }
        }

        private void SetParents(DataItem dataItem)
        {
            if (dataItem.Children != null)
            {
                for (int i = 0; i < dataItem.Children.Count; ++i)
                {
                    dataItem.Children[i].Parent = dataItem;
                    SetParents(dataItem.Children[i]);
                }
            }
            else
            {
                dataItem.Children = new List<DataItem>();
            }
        }
        */

    }
}
