using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class UI_Jucier : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _rectTransform2;
    [SerializeField] private RectTransform _rectTransform3;
    [SerializeField] private bool _isOpened;

    private void OnEnable()
    {
        Open();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(_isOpened)
            {
                Close();
            }
            else
            {
                Open();
            }

            _isOpened = !_isOpened;
        }
    }

    public void Open()
    {
        _rectTransform.transform.localScale = Vector3.zero;
        _rectTransform.transform.JuicyScale(Vector3.one, 0.2f).Start();

        _rectTransform2.transform.localScale = Vector3.zero;
        _rectTransform2.transform.JuicyScale(Vector3.one, 0.2f).SetDelay(.5f).Start();

        _rectTransform3.transform.localPosition = new Vector3(0, -Screen.height);
     //   _rectTransform3.transform.JuicyLocalMoveY(0, 0.5f).SetEase(Ease.EaseInExpo).SetDelay(.5f).Start();
    }

    public void Close()
    {
        _rectTransform.transform.JuicyScale(Vector3.zero, 0.2f).Start();
        _rectTransform2.transform.JuicyScale(Vector3.zero, 0.2f).SetDelay(.5f).Start();
      //  _rectTransform3.transform.JuicyLocalMoveY(-Screen.height, 5f).SetEase(Ease.Linear).SetDelay(.5f).Start();
    }
}
