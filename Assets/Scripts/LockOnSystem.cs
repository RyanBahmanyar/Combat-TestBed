using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

//Reference: https://amirazmi.net/targeting-system/

public class LockOnSystem : MonoBehaviour
{
    #region Reference Variables
    private CinemachineTargetGroup _targetGroup;
    //private int _numberOfTargetsInRange;
    private List<GameObject> _targetCandidates;
    private bool _lockedOn;
    private Transform _currentLockOnTransform;
    [SerializeField] float _maxLockOnAngle;
    private Image _lockOnReticle;

    #endregion

    void Awake()
    {
        _targetGroup = GameObject.Find("Target Group").GetComponent<CinemachineTargetGroup>();
        _lockOnReticle = GameObject.Find("Reticle").GetComponent<Image>();
        _lockOnReticle.enabled = false; 
        _targetCandidates = new List<GameObject>();
        //_numberOfTargetsInRange = 0;
        _lockedOn = false;
    }

    //Update is called once per frame
    void Update()
    {
        if(_lockedOn)
        {
            //Making lock on UI follow position of object currently locked on
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(_currentLockOnTransform.position);
            _lockOnReticle.transform.position = screenPosition;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Targetable")
        {
            Debug.Log("Target Candidate Added");
            _targetCandidates.Add(other.gameObject);
            // _numberOfTargetsInRange++;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.tag == "Targetable")
        {
            _targetCandidates.Remove(other.gameObject);
            // _numberOfTargetsInRange--;
        }
    }

    public void ToggleLockOn()
    {
        //Add lock on target to camera target group
        if(!_lockedOn && _targetCandidates.Count > 0)
        {
            //Sort the target candidates in order to lock on to the closest one
            List<GameObject> sortedTargetCandidates = _targetCandidates.OrderBy(targetCandidates =>
            {
                return AngleFromTarget(targetCandidates);
            }).ToList();

            // foreach( var x in sortedTargetCandidates) {
            //     Debug.Log( x.ToString());
            // }

            //Only use first target candidate from the sorted list that is within the max angle range
            int index = 0;
            while(_currentLockOnTransform == null && index < sortedTargetCandidates.Count)
            {
                if(AngleFromTarget(sortedTargetCandidates[index]) <= _maxLockOnAngle)
                {
                    Debug.Log("Locking On");
                    _currentLockOnTransform = sortedTargetCandidates[index].transform;
                    _targetGroup.AddMember(_currentLockOnTransform, 1f, 1f);
                    _lockedOn = true;
                    _lockOnReticle.enabled = true;
                }
                index++;
            }
        }
        //Remove current lock on target from camera target group
        else if (_currentLockOnTransform != null)
        {
            Debug.Log("Locking Off");
            _targetGroup.RemoveMember(_currentLockOnTransform);
            _currentLockOnTransform = null;
            _lockedOn = false;
            _lockOnReticle.enabled = false;
        }
    }

    private float AngleFromTarget(GameObject target)
    {
        //get vector from camera to target candidate
        Vector3 targetDirection = target.transform.position - Camera.main.transform.position;

        //convert camera forward to 2d vector
        Vector2 cameraForward = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);

        //convert target direction into 2d vector
        Vector2 targetDirection2D = new Vector2(targetDirection.x, targetDirection.z);

        //angle between camera forward2d and target direction 2d vectors
        return Vector2.Angle(cameraForward, targetDirection2D);
    }
}
