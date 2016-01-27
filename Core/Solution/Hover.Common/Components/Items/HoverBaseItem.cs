﻿using System.Collections.Generic;
using System.Linq;
using Hover.Common.Items;
using Hover.Common.Renderers;
using Hover.Common.Styles;
using Hover.Common.Util;
using UnityEngine;

namespace Hover.Common.Components.Items {

	/*================================================================================================*/
	public abstract class HoverBaseItem : MonoBehaviour {

		public IBaseItem Item { get; private set; }

		public string Id = "";
		public string Label = "";
		public float Width = 1;
		public float Height = 1;

		public bool IsEnabled = true;
		public bool IsVisible = true;

		protected readonly ValueBinder<string> vBindId;
		protected readonly ValueBinder<string> vBindLabel;
		protected readonly ValueBinder<float> vBindWidth;
		protected readonly ValueBinder<float> vBindHeight;

		protected readonly ValueBinder<bool> vBindEnabled;
		protected readonly ValueBinder<bool> vBindVisible;
		
		[HideInInspector]
		protected bool vBlockBaseLabelBinding;

		private BaseItem vFullItem;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected HoverBaseItem() {
			vBindId = new ValueBinder<string>(
				(x => { Item.Id = x; }),
				(x => { Id = x; }),
				ValueBinder.AreStringsEqual
			);

			vBindLabel = new ValueBinder<string>(
				(x => { Item.Label = x; }),
				(x => { Label = x; }),
				ValueBinder.AreStringsEqual
			);

			vBindWidth = new ValueBinder<float>(
				(x => { vFullItem.Width = x; }),
				(x => { Width = x; }),
				ValueBinder.AreFloatsEqual
			);

			vBindHeight = new ValueBinder<float>(
				(x => { vFullItem.Height = x; }),
				(x => { Height = x; }),
				ValueBinder.AreFloatsEqual
			);

			vBindEnabled = new ValueBinder<bool>(
				(x => { Item.IsEnabled = x; }),
				(x => { IsEnabled = x; }),
				ValueBinder.AreBoolsEqual
			);

			vBindVisible = new ValueBinder<bool>(
				(x => { Item.IsVisible = x; }),
				(x => { IsVisible = x; }),
				ValueBinder.AreBoolsEqual
			);
		}

		/*--------------------------------------------------------------------------------------------*/
		protected void Init(BaseItem pItem) {
			Item = pItem;
			vFullItem = pItem;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public virtual void Awake() {
			vFullItem.DisplayContainer = gameObject;
			
			if ( string.IsNullOrEmpty(Id) ) {
				Id = Item.AutoId+"";
			}
			
			if ( string.IsNullOrEmpty(Label) ) {
				Label = gameObject.name;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void Start() {
			UpdateAllValues(true);
		}

		/*--------------------------------------------------------------------------------------------*/
		public virtual void Update() {
			UpdateAllValues();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected virtual void UpdateAllValues(bool pForceUpdate=false) {
			vBindId.UpdateValuesIfChanged(Item.Id, Id, pForceUpdate);
			
			if ( !vBlockBaseLabelBinding ) {
				vBindLabel.UpdateValuesIfChanged(Item.Label, Label, pForceUpdate);
			}
			
			vBindWidth.UpdateValuesIfChanged(Item.Width, Width, pForceUpdate);
			vBindHeight.UpdateValuesIfChanged(Item.Height, Height, pForceUpdate);

			vBindEnabled.UpdateValuesIfChanged(Item.IsEnabled, IsEnabled, pForceUpdate);
			vBindVisible.UpdateValuesIfChanged(Item.IsVisible, IsVisible, pForceUpdate);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IItemStyle GetStyle() {
			return gameObject.GetComponents<MonoBehaviour>()
				.OfType<IItemStyle>()
				.FirstOrDefault();
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public IHoverItemRenderer GetRenderer() {
			return gameObject.GetComponents<MonoBehaviour>()
				.OfType<IHoverItemRenderer>()
				.FirstOrDefault();
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static IBaseItem[] GetChildItems(GameObject pParentGo) {
			Transform tx = pParentGo.transform;
			int childCount = tx.childCount;
			var items = new List<IBaseItem>();

			for ( int i = 0 ; i < childCount ; ++i ) {
				HoverBaseItem hni = tx.GetChild(i).GetComponent<HoverBaseItem>();
				IBaseItem item = hni.Item;

				if ( !item.IsVisible ) {
					continue;
				}

				items.Add(item);
			}

			return items.ToArray();
		}

	}

}
