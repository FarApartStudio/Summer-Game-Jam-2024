using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.UISystem
{
    public enum Visibility
    {
        Visible,
        Invisible
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private List<Menu> _menuPrefabList;

        private List<Menu> _spawnedMenuList = new List<Menu>();
        private CanvasGroup _canvasGroup;
        private Visibility _visibility = Visibility.Visible;

        public Visibility GetVisibility => _visibility;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _canvasGroup = GetComponent<CanvasGroup>();
               // DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private void CreateNewInstance<T>()
        {
            Menu menu = _menuPrefabList.Find(x => x.GetType() == typeof(T));
            if (menu != null)
            {
                menu = Instantiate(menu, transform);
                _spawnedMenuList.Add(menu);
            }
            else Debug.LogError("Menu not found :" + typeof(T).Name);
        }

        public static T InitMenu<T>(bool enable = false) where T : GenericMenu<T>
        {
            GenericMenu<T> menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as GenericMenu<T>;
            if (menu == null)
            {
                Instance.CreateNewInstance<T>();
                menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as GenericMenu<T>;
            }
            return menu.Init(enable);
        }

        public static T OpenMenu<T>() where T : GenericMenu<T>
        {
            GenericMenu<T> menu = GetMenu <T>();
            return menu.Open();
        }

        public static void CloseMenu<T>() where T : GenericMenu<T>
        {
            GenericMenu<T> menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as GenericMenu<T>;
            if (menu != null)
            {
                menu.Close();
                if (menu.GetCloseBehaviour() == Menu.ExitBehaviour.Destory)
                    Instance._spawnedMenuList.Remove(menu);
            }
            else Debug.LogError("Menu not found");
        }

        public static T GetMenu<T>() where T : GenericMenu<T>
        {
            GenericMenu<T> menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as GenericMenu<T>;
            if (menu != null)
            {
                return (T)menu;
            }
            else
            {
                //Debug.LogWarning("Menu not found so creating new instance");
                return InitMenu<T>();
            }
        }

        public void ToggleVisibility(Visibility state)
        {
            _visibility = state;
            _canvasGroup.alpha = state == Visibility.Visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = state == Visibility.Visible;
            _canvasGroup.interactable = state == Visibility.Visible;
        }
    }
}

