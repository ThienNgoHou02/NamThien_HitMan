using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache
{
    public static Dictionary<Collider, Character> _ColliderToCharacters = new Dictionary<Collider, Character>();
    public static Character GetCharacterByCollider(Collider collider)
    {
        if (!_ColliderToCharacters.TryGetValue(collider, out Character c))
        {
            c = collider.GetComponent<Character>();
            if (c)
                _ColliderToCharacters.Add(collider, c);
            else
            {
                return null;
            }
        }
        return _ColliderToCharacters[collider];
    }

    public static Dictionary<Transform, Character> _TransformToCharacters = new Dictionary<Transform, Character>();
    public static Character GetCharacterByTransform(Transform tf)
    {
        if (!_TransformToCharacters.TryGetValue(tf, out Character c))
        {
            c = tf.GetComponent<Character>();
            if (c)
                _TransformToCharacters.Add(tf, c);
            else
            {
                return null;
            }
        }
        return _TransformToCharacters[tf];
    }

    public static Dictionary<Character, Transform> _CharacterToTransforms = new Dictionary<Character, Transform>();
    public static Transform GetTransformByCharacter(Character character)
    {
        if (!_CharacterToTransforms.TryGetValue(character, out Transform tf))
        {
            tf = character.transform;
            if (tf)
            {
                _CharacterToTransforms.Add(character, tf);
            }
            else
            {
                return null;
            }
        }
        return _CharacterToTransforms[character];
    }

    public static Dictionary<Transform, MeshRenderer> _TransformToMeshRens = new Dictionary<Transform, MeshRenderer>();
    public static MeshRenderer GetMeshRenByTransform(Transform tf)
    {
        if (!_TransformToMeshRens.TryGetValue(tf, out MeshRenderer mesh))
        {
            mesh = tf.GetComponent<MeshRenderer>();
            if (mesh)
            {
                _TransformToMeshRens.Add(tf, mesh);
            }
            else
            {
                return null;
            }    
        }
        return _TransformToMeshRens[tf];
    }

    public static Dictionary<float, WaitForSeconds> _TimeToWFSs = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWFSByTime(float time)
    {
        if (!_TimeToWFSs.TryGetValue(time, out WaitForSeconds wfs))
        {
            wfs = new WaitForSeconds(time);
            if (wfs != null)
            {
                _TimeToWFSs.Add(time, wfs);
            }
            else
            {
                return null;    
            }
        }
        return _TimeToWFSs[time];
    }

    public static Dictionary<float, WaitForSecondsRealtime> _TimeToWFSRealTimes = new Dictionary<float, WaitForSecondsRealtime>();
    public static WaitForSecondsRealtime GetWFSRealTimeByTime(float time)
    {
        if (!_TimeToWFSRealTimes.TryGetValue(time, out WaitForSecondsRealtime wfsRT))
        {
            wfsRT = new WaitForSecondsRealtime(time);
            if (wfsRT != null)
            {
                _TimeToWFSRealTimes.Add(time, wfsRT);
            }
            else
            {
                return null;
            }
        }
        return _TimeToWFSRealTimes[time];
    }
}
