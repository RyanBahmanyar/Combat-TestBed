using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

//Reference: https://amirazmi.net/targeting-system/

public class LockOnSystem : MonoBehaviour
{
    #region Reference Variables
    private CinemachineTargetGroup _targetGroup;
    private int _numberOfTargetsInRange;
    private List<GameObject> _targetCandidates;
    private PlayerInput _playerInput;
    private bool _isLockOnPressed;
    private bool _lockedOn;
    private Transform _currentLockOnTransform;

    #endregion

    void Awake()
    {
        _playerInput = new PlayerInput();
        _targetGroup = GameObject.Find("Target Group").GetComponent<CinemachineTargetGroup>();
        _targetCandidates = new List<GameObject>();
        _numberOfTargetsInRange = 0;
        _lockedOn = false;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(_isLockOnPressed)
    //     {
    //         Debug.Log("Lock on pressed");
    //         ToggleLockOn();
    //     }
    // }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Targetable")
        {
            Debug.Log("Target Candidate Added");
            _targetCandidates.Add(other.gameObject);
            _numberOfTargetsInRange++;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.tag == "Targetable")
        {
            _targetCandidates.Remove(other.gameObject);
            _numberOfTargetsInRange--;
        }
    }

    public void ToggleLockOn()
    {
        //Add lock on target to camera target group
            if(!_lockedOn && _targetCandidates.Count > 0)
            {
                Debug.Log("Locking On");
                _currentLockOnTransform = _targetCandidates[0].transform;
                _targetGroup.AddMember(_currentLockOnTransform, 1f, 1f);
                _lockedOn = true;
            }
            //Remove current lock on target from camera target group
            else if (_currentLockOnTransform != null)
            {
                Debug.Log("Locking Off");
                _targetGroup.RemoveMember(_currentLockOnTransform);
                _lockedOn = false;
            }
    }
}
