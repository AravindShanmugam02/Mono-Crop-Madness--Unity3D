using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void SetThisSlotWithShopItem(GameItemsData gameItemsData) { _thisSlotItem = gameItemsData; }

    private bool _hovering;
    private GameItemsData _thisSlotItem;

    private bool _ranOnce;

    // Start is called before the first frame update
    void Start()
    {
        _hovering = false;
        _ranOnce = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.fullyExited)
        {
            _hovering = true;
            ShopManager.Instance.RenderItemInfoInShopItemDescriptionTextBox(_thisSlotItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.fullyExited)
        {
            _hovering = false;
            ShopManager.Instance.RenderItemInfoInShopItemDescriptionTextBox(null);
        }
    }
}
