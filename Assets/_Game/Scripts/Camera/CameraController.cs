using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Transform _cameraTF;
    [SerializeField] private float _followingSpeed;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private Material _normalMAT;
    [SerializeField] private Material _fadedMAT;

    Transform _followTF;
    Transform _obstacleTF;
    Vector3 _Camera;

    private void Update()
    {
        RaycastHit hit;
        Vector3 direction = (_followTF.position - _cameraTF.position).normalized;
        float distance = Vector3.Distance(_cameraTF.position, _followTF.position);
        if (Physics.Raycast(_cameraTF.position, direction, out hit, distance, _obstacleLayer))
        {
            if (_obstacleTF != hit.transform)
            {
                if (_obstacleTF != null)
                {
                    MeshRenderer oldMesh = Cache.GetMeshRenByTransform(_obstacleTF);
                    oldMesh.material = _normalMAT;
                }

                _obstacleTF = hit.transform;
                MeshRenderer newMesh = Cache.GetMeshRenByTransform(_obstacleTF);
                newMesh.material = _fadedMAT;
            }
        }
        else
        {
            if (_obstacleTF != null)
            {
                MeshRenderer mesh = Cache.GetMeshRenByTransform(_obstacleTF);
                mesh.material = _normalMAT;
                _obstacleTF = null;
            }
        }
    }
    private void LateUpdate()
    {
        Vector3 follow = Vector3.SmoothDamp(transform.position, _followTF.position, ref _Camera, _followingSpeed * Time.deltaTime);
        transform.position = follow;
    }
    public void CameraShake(float shakePower)
    {
        float xShake = Random.Range(-shakePower, shakePower);
        float yShake = Random.Range(-shakePower, shakePower);
        float zShake = Random.Range(-shakePower, shakePower);
        transform.position += new Vector3(xShake, yShake, zShake);
    }
    public Transform Follow
    {
        get { return _followTF; }
        set { _followTF = value; }
    }
}
