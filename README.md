# Root Motion Animation Track

A custom Timeline track for playing animations on characters via Timeline that does proper scene offsets.

## Overview

This track provides a simple way to play animation clips via Timeline. It binds to an Animator component and plays animations using Unity's playable graph system.

## Usage

1. Add a "FM/Root Motion Animation Track" to your Timeline
2. Bind an Animator component to the track
3. Add animation clips to the track

## Handling Root Motion

This track does **NOT** apply root motion directly. If your animations have root motion curves and you need them applied to the character, you must handle this yourself via the `OnAnimatorMove` callback.

### Example: Root Motion Handler Script

Create a script and attach it to your character. Add the following code:

```csharp
using UnityEngine;
using UnityEngine.AI;

public class RootMotionHandler : MonoBehaviour
{
    private Animator _cachedAnimator;
    private Animator CachedAnimator
    {
        get
        {
            if (_cachedAnimator == null) _cachedAnimator = GetComponent<Animator>();
            return _cachedAnimator;
        }
    }
    
    private NavMeshAgent _cachedNavMesh;
    private NavMeshAgent CachedNavMesh
    {
        get
        {
            if (_cachedNavMesh == null) _cachedNavMesh = GetComponent<NavMeshAgent>();
            return _cachedNavMesh;
        }
    }
    
    /*************************************************************************************************************/
    
    void OnAnimatorMove()
    {
        if (Application.isPlaying)
        {
            // Runtime: bound to NavMesh or physics
            // Example using NavMeshAgent:
            float y = Mathf.Lerp(transform.position.y, CachedNavMesh.nextPosition.y, Time.deltaTime * 6f);
            transform.position = new Vector3(CachedNavMesh.nextPosition.x, y, CachedNavMesh.nextPosition.z);
            transform.rotation = CachedAnimator.rootRotation;
        }
        else
        {
            // Edit time: unbound to NavMesh/physics
            // Directly apply animator delta for Timeline preview
            transform.position += CachedAnimator.deltaPosition;
            transform.rotation = CachedAnimator.rootRotation;
        }
    }
}
```

### Notes

- The `OnAnimatorMove` callback intercepts root motion before Unity applies it
- At runtime, you can bind root motion to NavMesh, physics, or custom movement systems
- In edit mode (Timeline scrubbing), you apply the animator's delta directly for preview
- You can customize the runtime behavior based on your game's needs (physics, CharacterController, etc.)


