using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private TrailRenderer _trailRenderer;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
    }
    public void EnableTrail()
    {
        _trailRenderer.enabled = true;
    }
    public void DisableTrail()
    {
        _trailRenderer.enabled = false;
    }
}
