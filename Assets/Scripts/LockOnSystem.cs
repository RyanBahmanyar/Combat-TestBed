using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Reference: https://amirazmi.net/targeting-system/

public class LockOnSystem : MonoBehaviour
{
    #region Member Variables
    private int _numberOfTargetsInRange = 0;
    private List<GameObject> _targetCandidates;

    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Targetable")
        {
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
}
