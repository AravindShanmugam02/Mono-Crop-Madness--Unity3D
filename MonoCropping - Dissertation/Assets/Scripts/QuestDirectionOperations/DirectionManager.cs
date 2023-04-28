using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class DirectionManager : MonoBehaviour
{
    private Transform _thirdPersonCam;
    private RawImage _hCompass;
    [SerializeField] private Sprite _hCompassImg;

    // Start is called before the first frame update
    void Start()
    {
        _thirdPersonCam = GameObject.Find("Main Camera").GetComponent<Transform>();
        _hCompass = GameObject.Find("HCompassImage").GetComponent<RawImage>();
        //_hCompassImg = AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/Direction/HCompass.png", typeof(Sprite)) as Sprite;
        
        if( _hCompassImg == null || _hCompass == null || _thirdPersonCam == null)
        {
            Debug.LogError("Error - Direction Manager!");
        }

        _hCompass.texture = _hCompassImg.texture;
    }

    // Update is called once per frame
    void Update()
    {
        _hCompass.uvRect = new Rect((_thirdPersonCam.eulerAngles.y / 360.0f), 0.0f, _hCompass.uvRect.width, _hCompass.uvRect.height);
    }
}
