using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.SocialPlatforms;
using System.IO;
using System.Linq;
using System;

namespace common
{
    public class FloatingDebug : MonoBehaviour
    {
        public static void Create()
        {
		    if(instance_ == null)
		    {
			    UnityEngine.Object prefab = Resources.Load("P_FloatingDebug");
			    UnityEngine.Object obj = GameObject.Instantiate(prefab);
			    DontDestroyOnLoad(obj);
		    }
        }

        public static FloatingDebug instance_;
        public static FloatingDebug instance
        {
            get
            {
                return instance_;
            }
        }

        /// <summary>
        /// Add button in floating debug by add this attribute to static non-paramter method.
        /// Type.Normal will invoke your method.
        /// Type.TextInfo will get string from your method to display at each update.
        /// </summary>
        public class ItemAttribute : Attribute
        {
            public enum Type
            {
                Normal,
				MoreButtons,
                TextInfo
            }

            public string category { get; private set; }
            public string name { get; private set; }
            public Type type { get; private set; }

            public ItemAttribute(string category, string name, Type type = Type.Normal)
            {
                Debug.Assert(!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(name), "ItemAttribute category and name can't be empty.");
                this.category = category;
                this.name     = name;
                this.type     = type;
            }
        }

        public Text fpsText;
        public GameObject itemRoot;
        public RectTransform itemsPivot;
		public GameObject moreButtonsRoot;
		public RectTransform moreButtonsPivot;
        public GameObject textInfo;
        public Text infoText;

        SortedDictionary<string, IGrouping<string, MethodInfo>> itemMethods;
        MethodInfo textInfoMethod;

        void Awake()
        {
            instance_ = this;
        }

        void Update()
        {
            FpsDisplay();
            UpdateInfoText();
        }

        void FpsDisplay()
        {
			FPS.Update();
            int fps = FPS.fps;
            fpsText.text = string.Format("{0}", fps);
            if (fps >= 30)
                fpsText.color = Color.white;
            else if (fps >= 20)
                fpsText.color = Color.black;
            else 
                fpsText.color = Color.red;
        }

        public void OpenItemRoot()
        {
            if (itemMethods == null)
			{
				CreateItems(itemsPivot);
			}
			if (itemRoot.activeSelf)
			{
				moreButtonsRoot.SetActive(false);
				moreButtonsPivot.DestoryChildren();
			}
            itemRoot.SetActive(!itemRoot.activeSelf);
            textInfo.SetActive(false);
        }

        void CreateItems(RectTransform pivot)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var methodsDict = assemblies.SelectMany(a => a.GetTypes())
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttributes(typeof(ItemAttribute), false).Length > 0)
				.Select(m => new KeyValuePair<string, MethodInfo>((m.GetCustomAttributes(typeof(ItemAttribute), false)[0] as ItemAttribute).name, m))
                .GroupBy(m => (m.Value.GetCustomAttributes(typeof(ItemAttribute), false)[0] as ItemAttribute).category)
                .ToDictionary(kv => kv.Key);

            var itemMethods = new SortedDictionary<string, IGrouping<string, KeyValuePair<string, MethodInfo>>>(methodsDict);
            CreateItemsByMethods(itemMethods, pivot);
        }

		void CreateItemsByMethods(SortedDictionary<string, IGrouping<string, KeyValuePair<string, MethodInfo>>> itemMethods, RectTransform pivot)
		{
			int row = 0, col = 0;
            foreach (var methods in itemMethods)
            {
                CreateText(methods.Key, methods.Key, row, pivot, Color.white, 600f, 70f, 36, TextAnchor.LowerLeft);
                ++row; col = 0;
                foreach (var method in methods.Value)
                {
                    CreateButton(method.Key, row, col, method.Value, pivot);
                    if (col == 0)
                    {
                        ++col;
                    }
                    else
                    {
                        col = 0; ++row;
                    }
                }
                if (col == 1)
                {
                    ++row;
                }
            }
            pivot.sizeDelta = new Vector2(pivot.sizeDelta.x, 80 * row + 80);
		}

        void CreateText(string name, string str, int row, Transform parent, Color color, float width, float height, int fontSize, TextAnchor alignment, bool indent = false)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            Text text = gameObject.AddComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont("Arial", text.fontSize);
            text.rectTransform.anchoredPosition  = new Vector3(indent ? 40f : 1f, -1f * row * 80, 1f);
            text.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            text.rectTransform.anchorMin  = new Vector2(0f, 1f);
            text.rectTransform.anchorMax  = new Vector2(0f, 1f);
            text.rectTransform.pivot      = new Vector2(0f, 1f);
            text.rectTransform.sizeDelta  = new Vector2(width, height);
            text.color                = color;
            text.fontSize             = fontSize;
            text.alignment            = alignment;
            text.horizontalOverflow   = HorizontalWrapMode.Overflow;
            text.verticalOverflow     = VerticalWrapMode.Truncate;
            text.text                 = str;
        }

        void CreateButton(string name, int row, int col, MethodInfo method, RectTransform pivot)
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = pivot;
            Image image = obj.AddComponent<Image>();
            image.rectTransform.anchoredPosition  = new Vector3(1f + col * 320f, -1f * row * 80, 1f);
            image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            image.rectTransform.anchorMin  = new Vector2(0f, 1f);
            image.rectTransform.anchorMax  = new Vector2(0f, 1f);
            image.rectTransform.pivot      = new Vector2(0f, 1f);
            image.rectTransform.sizeDelta  = new Vector2(300f, 70f);
            Button button = obj.AddComponent<Button>();
            CreateText("Text", name, 0, obj.transform, Color.black, 300f, 70f, 34, TextAnchor.MiddleCenter);

            button.onClick.AddListener(() => {
				if (method.GetCustomAttributes(typeof(ItemAttribute), false).Length > 0)
				{
					ItemAttribute item = method.GetCustomAttributes(typeof(ItemAttribute), false)[0] as ItemAttribute;
					if (item.type == ItemAttribute.Type.TextInfo)
					{
						textInfoMethod = method;
						textInfo.SetActive(true);
					}
					else if (item.type == ItemAttribute.Type.MoreButtons)
					{
						moreButtonsRoot.SetActive(true);
						var itemMethods = (SortedDictionary<string, IGrouping<string, KeyValuePair<string, MethodInfo>>>) method.Invoke(null, null);
						CreateItemsByMethods(itemMethods, moreButtonsPivot);
					}
					else
					{
						method.Invoke(null, null);
					}
				}
                else
				{
					method.Invoke(null, new object[] { name });
				}
            });
        }

        void UpdateInfoText()
        {
            if (!infoText.gameObject.activeInHierarchy)
                return;
            string str = (string) textInfoMethod.Invoke(null, null);
            infoText.text = str;
            ((RectTransform) infoText.transform.parent).sizeDelta = infoText.rectTransform.sizeDelta;
        }
    }
}
