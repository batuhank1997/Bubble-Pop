using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Game.Core;
using _01_Scripts.Game.Managers;
using DG.Tweening;
using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    [SerializeField] private Item itemPrefab;
    [SerializeField] private float shootSpeed;

    private Item _itemToShoot;
    private Item _nextItemToShoot;
    private Vector3 _shootPos = Vector3.zero;

    private Camera cam;
    private bool canShoot = true;
    
    private void Start()
    {
        cam = Camera.main;
        
        LoadItemToShoot();
        LoadNextItemToShoot();
        SetSubscriptions();
    }

    #region Subscriptions

    void SetSubscriptions()
    {
        GameManager.I.OnGameFail += OnGameFail;
    }

    private void OnDestroy()
    {
        GameManager.I.OnGameFail -= OnGameFail;
    }

    void OnGameFail()
    {
        canShoot = false;
    }

    #endregion

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            _shootPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && canShoot)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        _itemToShoot.SetReadyToShoot();
        Transform bubble = _itemToShoot.transform;
        var dir = (new Vector3(_shootPos.x, _shootPos.y, 0) - bubble.position).normalized;
        
        bubble.DOMove(dir, shootSpeed).SetSpeedBased(true).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

        ReloadShooter();
    }

    void LoadItemToShoot()
    {
        _itemToShoot = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        _itemToShoot.PrepareItem();
    }
    
    void LoadNextItemToShoot()
    {
        _nextItemToShoot = Instantiate(itemPrefab, transform.position + Vector3.left, Quaternion.identity);
        _nextItemToShoot.PrepareItem();
        _nextItemToShoot.transform.localScale *= 0.75f;
    }
    
    void ReloadShooter()
    {
        _itemToShoot = _nextItemToShoot;
        _itemToShoot.transform.DOScale(1, 0.25f);
        _itemToShoot.transform.DOMove(transform.position, 0.25f);
        
        LoadNextItemToShoot();
    }
}
