using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanelAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float popUpSpeed;
    bool visible = true;
    bool transitioning = false;
    float yTransform = 50;
    public void OnPointerEnter(PointerEventData eventData)
    {
        transitioning = true;
        visible = false;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transitioning = true;
        visible = true;
    }
    private void Update()
    {
        if (transitioning)
        {
            if (!visible)
            {
                yTransform -= popUpSpeed * Time.deltaTime;
                if (yTransform <= 0)
                {
                    transitioning = false;
                    yTransform = 0;
                }
                rectTransform.anchoredPosition = new Vector3(0, yTransform, 0);

            }
            else
            {
                yTransform += popUpSpeed * Time.deltaTime;
                if (yTransform >= 50)
                {
                    transitioning = false;
                    yTransform = 50;
                }
                rectTransform.anchoredPosition = new Vector3(0, yTransform, 0);
            }
        }
    }
}
