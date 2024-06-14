using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Pelumi.UISystem;
using System;

namespace GameProject
{
    public class SelectionWheelMenu : GenericMenu<SelectionWheelMenu>
    {
        //public event Func<List<TrapSO>> GetTrapVisualData;
        //public event Action<TrapSO> OnTrapSelected;


        [SerializeField] private float _gap = 10f;
        [SerializeField] private RingUI _ringUIPrefab;

        private int _numberOfSlots = 8;
        private List<RingUI> _ringUIs = new List<RingUI>();
        //private List<TrapSO> _trapVisualDataList = new List<TrapSO>();

        [FoldoutGroup("Debug")] [ReadOnly] [SerializeField] private int _selectedSlot = -1;
        //[SerializeField] private List<TrapSO> trapSOs;

        private void Start()
        {
            //GetTrapVisualData += () => trapSOs;
            OnOpened();
        }

        protected override void OnCreated()
        {
  
        }

        protected override void OnOpened()
        {
            //_trapVisualDataList = GetTrapVisualData?.Invoke();
            //_numberOfSlots = _trapVisualDataList.Count;

            float step = 360f / _numberOfSlots;
            float _iconDistance = Vector3.Distance(_ringUIPrefab.GetIcon.transform.position, _ringUIPrefab.GetRing.transform.position);

            for (int i = 0; i < _numberOfSlots; i++)
            {
                RingUI ringUI = Instantiate(_ringUIPrefab, transform);
                _ringUIs.Add(ringUI);

                ringUI.transform.localPosition = Vector3.zero;
                ringUI.transform.localRotation = Quaternion.identity;

                ringUI.GetRing.fillAmount = 1f / _numberOfSlots - _gap / 360f;
                ringUI.GetRing.transform.localPosition = Vector3.zero;
                ringUI.GetRing.transform.localRotation = Quaternion.Euler(0f, 0f, -step / 2f + _gap / 2f + i * step);
                ringUI.GetRing.color = new Color(1f, 1f, 1f, 0.5f);

                //ringUI.GetIcon.transform.localPosition = ringUI.GetRing.transform.localPosition + Quaternion.AngleAxis(i * step, Vector3.forward) * Vector3.up * _iconDistance;
                //ringUI.GetIcon.sprite = _trapVisualDataList[i].GetSprite;

                //float openDelay = .25f / _numberOfSlots * i;

                //ringUI.Init(openDelay);

                //TrapSO trapSO = _trapVisualDataList[i];
                //ringUI.OnSelected = ()=>
                //{
                //    OnTrapSelected.Invoke(trapSO);
                //    Close();
                //};
            }

            Cursor.lockState = CursorLockMode.None;
        }

        protected override void OnClosed()
        {
            foreach (RingUI ringUI in _ringUIs)
            {
                Destroy(ringUI.gameObject);
            }

            _ringUIs.Clear();
            //_trapVisualDataList.Clear();

            Cursor.lockState = CursorLockMode.Locked;
        }

        protected override void OnDestoryInvoked()
        {

        }

        private void Update()
        {
            HighlightSlot(HandleSelection());
            HandleInput();
        }

        public int HandleSelection()
        {
            float step = 360f / _numberOfSlots;
            float _mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + step / 2f);
            int _slot = (int)(_mouseAngle / step);
            return _slot;
        }

        public void HighlightSlot(int currentSlot)
        {
            if (_selectedSlot != currentSlot)
            {
                if (_selectedSlot != -1)
                {
                    _ringUIs[_selectedSlot].UnHighlight();
                }

                _selectedSlot = currentSlot;

                _ringUIs[_selectedSlot].Highlight();
            }
        }

        public void HandleInput()
        {
            if (_selectedSlot == -1) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _ringUIs[_selectedSlot].Select();
            }
        }

        private float NormalizeAngle(float angle) => (angle + 360f) % 360f;

        public override void ResetMenu()
        {
            throw new NotImplementedException();
        }
    }
}
