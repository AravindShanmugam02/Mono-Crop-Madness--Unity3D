using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField]
    private float _rayCastDistance = 2.0f;

    private PlayerController _playerController;
    private LandManagerHUD _landManagerHUD;
    private LandManager _land = null, _selectedLand = null;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
        _landManagerHUD = GameObject.Find("LandManagerHUD").GetComponent<LandManagerHUD>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit _hit;

        if (Physics.Raycast(transform.position, Vector3.down, out _hit, _rayCastDistance))
        {
            //Debug.Log("::" + _hit.collider.tag);
            OnRayHit(_hit);
        }
    }

    void OnRayHit(RaycastHit hit)
    {
        Collider other = hit.collider;
        _land = null;

        if (other.tag == "Land")
        {
            _land = other.GetComponent<LandManager>();
            SelectLand(_land);
        }
        else
        {
            if (_selectedLand != null)
            { 
              _selectedLand.SelectedActivate(false);
              _selectedLand = null;
            }
        }
    }

    void SelectLand(LandManager land)
    {
        if(_selectedLand != null)
        {
            _selectedLand.SelectedActivate(false);
            _selectedLand = null;
        }

        _selectedLand = land;
        land.SelectedActivate(true);
    }

    void InvokeInteraction()
    {

    }
}