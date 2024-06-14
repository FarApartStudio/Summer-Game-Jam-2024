using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public enum Direction
    {
        Forward,
        Backward
    }

    [SerializeField] float flipSpeed = .5f;
    [SerializeField] Page[] pages;
    [SerializeField] private int currentPage = 0;

    JuicerRuntime juicerRuntime;

    public void FlipPage(Direction direction)
    {
        if (juicerRuntime != null && !juicerRuntime.IsFinished)
        {
            return;
        }

        float rotation = (direction == Direction.Backward) ? 180 : 0;
        juicerRuntime = pages[currentPage].transform.JuicyRotate(Vector3.up * rotation, flipSpeed)
            .SetEase(Ease.EaseOutSine)
            .SetOnTick(() =>
            {
                pages[currentPage].UpdateView();
            })
            .SetOnCompleted(() =>
            {
                if (direction == Direction.Backward)
                {
                    if (currentPage == pages.Length - 1)
                    {
                        return;
                    }
                    currentPage++;
                }
                else
                {
                    if (currentPage == 0)
                    {
                        return;
                    }
                    currentPage--;
                }
                pages[currentPage].transform.SetAsLastSibling();
            })
            .Start();
    }
}
