using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductCountManager : MonoBehaviour
{
    [SerializeField] private int _productCount;
    private TextMeshProUGUI _productCountText;

    // Start is called before the first frame update
    void Start()
    {
        _productCountText = GameObject.Find("ProductCountText").GetComponent<TextMeshProUGUI>();

        if(_productCountText == null)
        {
            Debug.LogError("_productCountText is null in Product Count Manager!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _productCount = BarnManager.Instance.GetStorageItemsCount();
        _productCountText.text = _productCount.ToString();
    }
}
