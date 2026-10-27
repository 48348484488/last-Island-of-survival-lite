using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using cfg;
using gs.bag.scmsg;

namespace SC.UI
{
	public class BoxOpenPanel : View
	{
		private Camera _camera;

		private RenderTexture _renderTexture;

		private int _boxInstanceId;

		private int _boxItemId;

		private GameObject _boxGameObject;

		private bool _isPlaying;

		private Animator _ani;

		private Tweener _tweener;

		private GameObject m_model_root;

		private GameObject m_role_bg;

		private GameObject m_hero_tex;

		private GameObject m_model_role;

		private GameObject m_camera;

		private GameObject m_model;

		private GameObject m_bg;

		private GameObject[] m_items;

		private GameObject m_itemsObj;

		protected override void onInit()
		{
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__0;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SUseItem.handler = (SUseItem.Handler)Delegate.Combine(SUseItem.handler, new SUseItem.Handler(SUseItemHandle));
			_boxInstanceId = (int)param;
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(_boxInstanceId);
			if (allItemByInstanceId != null)
			{
				_boxItemId = allItemByInstanceId.itemId;
				_isPlaying = true;
				ShowMode(_boxItemId);
			}
		}

		private void SUseItemHandle(SUseItem msg)
		{
			if (msg.instanceId != _boxInstanceId)
			{
				return;
			}
			List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(msg.dropDetail);
			for (int i = 0; i < m_items.Length; i++)
			{
				if (i < dropDetailInfo.Count)
				{
					m_items[i].SetActiveBetter(true);
					View.SetItemSprite(m_items[i], dropDetailInfo[i].icon);
				}
				else
				{
					m_items[i].SetActiveBetter(false);
				}
			}
		}

		protected override void onHide(string childView = null)
		{
			SUseItem.handler = (SUseItem.Handler)Delegate.Remove(SUseItem.handler, new SUseItem.Handler(SUseItemHandle));
		}

		protected override void onDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
		}

		public void ShowMode(int itemId)
		{
			m_role_bg.SetActiveBetter(false);
			ItemCfg itemCfg = ItemCfg.Get(itemId);
			Debug.LogError("itemCfg.modelPath:" + itemCfg.modelPath);
			ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CShowMode_003Em__1);
		}

		private void OpenBox()
		{
			Singleton<BagMgr>.Ins.UseItem(_boxInstanceId, 1);
			_isPlaying = false;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_model_root = component.GameObjects[0].gameObject;
			m_role_bg = component.GameObjects[1].gameObject;
			m_hero_tex = component.GameObjects[2].gameObject;
			m_model_role = component.GameObjects[3].gameObject;
			m_camera = component.GameObjects[4].gameObject;
			m_model = component.GameObjects[5].gameObject;
			m_bg = component.GameObjects[6].gameObject;
			m_items = component.GameObjects[7].gameObject.GetComponent<UIGameObjectList>().objects;
			m_itemsObj = component.GameObjects[7].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			if (_isPlaying)
			{
				if ((bool)_ani)
				{
					_ani.CrossFade("open", 0.1f, 1);
				}
				if (_tweener != null)
				{
					_tweener.Kill();
				}
				OpenBox();
			}
			else
			{
				Hide();
			}
		}

		[CompilerGenerated]
		private void _003CShowMode_003Em__1(GameObject go)
		{
			if ((bool)_boxGameObject)
			{
				UnityEngine.Object.DestroyImmediate(_boxGameObject);
			}
			go.SetLayerRecursively(LayerMask.NameToLayer("UI"));
			go.transform.SetParent(m_model.transform, true);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = Vector3.zero;
			_boxGameObject = go;
			_ani = _boxGameObject.GetComponent<Animator>();
			_tweener = ShortcutExtensions2.DoWait(duration: _ani.GetCurrentAnimatorStateInfo(0).length, target: base.gameObject).OnComplete(OpenBox);
		}
	}
}
