using System.Collections.Generic;
using UnityEngine;

// Drop on the Lobby Main Camera (replaces the old WallTransparency).
// Each frame, sweeps a capsule from the camera position to `target` (the player)
// and tells every WallSection it overlaps to hide; tells previously-hidden ones
// that left the capsule to show.
//
// Capsule (vs. a thin raycast) catches:
//   - walls the camera is currently clipping into / passing through
//   - walls whose meshes the line of sight would have threaded between
//   - the full volume between camera and player, not just a 1D line
//
// Non-WallSection colliders (floors, props, NPCs, outer walls without the
// component) are dropped from the result and stay opaque.
public class WallSectionFader : MonoBehaviour
{
    public Transform target;

    [Tooltip("Radius of the swept capsule between camera and player. Bigger = fades more walls near the line of sight.")]
    public float radius = 0.5f;

    [Tooltip("Optional layer filter. Default Everything; outer walls opt out by simply not having a WallSection.")]
    public LayerMask wallMask = ~0;

    [Tooltip("Reused buffer size for the overlap query. Raise if you can have more than this many colliders in one capsule at once.")]
    public int maxOverlaps = 32;

    private Collider[] overlapBuffer;
    private readonly HashSet<WallSection> currentlyHidden = new();
    private readonly HashSet<WallSection> nextHidden = new();

    void Awake()
    {
        overlapBuffer = new Collider[Mathf.Max(8, maxOverlaps)];
    }

    void LateUpdate()
    {
        if (target == null) return;

        nextHidden.Clear();

        int count = Physics.OverlapCapsuleNonAlloc(
            transform.position, target.position, radius,
            overlapBuffer, wallMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            Collider c = overlapBuffer[i];
            if (c == null) continue;
            WallSection section = c.GetComponentInParent<WallSection>();
            if (section != null) nextHidden.Add(section);
        }

        // Newly entered → start hiding.
        foreach (WallSection s in nextHidden)
            if (!currentlyHidden.Contains(s))
                s.SetHidden(true);

        // Just left → start showing.
        foreach (WallSection s in currentlyHidden)
            if (!nextHidden.Contains(s))
                s.SetHidden(false);

        currentlyHidden.Clear();
        foreach (WallSection s in nextHidden)
            currentlyHidden.Add(s);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawLine(transform.position, target.position);
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(target.position, radius);
    }
}
