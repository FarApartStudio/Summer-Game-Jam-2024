using System;
using UnityEngine;

namespace Pelumi.UISystem
{
    public abstract class GenericMenu<T> : Menu where T : GenericMenu<T>
    {
        public event Action OnCreatedEvent;
        public event Action OnOpenedEvent;
        public event Action OnClosedEvent;

        public T Init(bool enable = false)
        {
            InitialiseMenu();
            gameObject.SetActive(enable);
            OnCreatedEvent?.Invoke();
            return (T)Convert.ChangeType(this, typeof(T));
        }

        public T Open()
        {
            if (this == null)
            {
                Debug.LogError("Menu is null");
                return null;
            }

            OpenMenu();
            OnOpenedEvent?.Invoke();
            return (T)Convert.ChangeType(this, typeof(T));
        }

        public virtual void Close()
        {
            CloseMenu();
            OnClosedEvent?.Invoke();
        }
    }
}
