using UnityEngine;

// Attach to a wall-group parent in Lobby. WallSectionFader (on the Main Camera)
// raycasts to the player and calls SetHidden(true/false) on whichever sections the
// line of sight currently passes through. Each section then fades its entire
// renderer subtree as a unit, instead of just the specific child mesh the ray hit.
//
// Outer perimeter walls (NorthWall, eastWall, westWall) intentionally don't get
// this component — without it the fader's GetComponentInParent lookup returns null
// and they stay fully opaque.
[DisallowMultipleComponent]
public class WallSection : MonoBehaviour
{
    [Range(0f, 1f)] public float hiddenAlpha = 0.15f;
    [Tooltip("Units of alpha per second. 6 ≈ 0.15s fade.")]
    public float fadeSpeed = 6f;

    private Renderer[] renderers;
    private Material[][] opaqueMaterials;       // cached sharedMaterials per renderer (originals)
    private Material[][] transparentMaterials;  // pre-built URP-transparent clones, owned by this component
    private bool[] usingTransparent;

    private float currentAlpha = 1f;
    private float targetAlpha = 1f;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int SurfaceId = Shader.PropertyToID("_Surface");
    private static readonly int BlendId = Shader.PropertyToID("_Blend");
    private static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");
    private static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(false);
        opaqueMaterials = new Material[renderers.Length][];
        transparentMaterials = new Material[renderers.Length][];
        usingTransparent = new bool[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] src = renderers[i].sharedMaterials;
            opaqueMaterials[i] = src;

            Material[] clones = new Material[src.Length];
            for (int j = 0; j < src.Length; j++)
            {
                clones[j] = src[j] != null ? BuildTransparentClone(src[j]) : null;
            }
            transparentMaterials[i] = clones;
        }
    }

    private static Material BuildTransparentClone(Material source)
    {
        Material m = new Material(source);
        // Match WallTransparency.cs's URP transparent setup, but once per material — not per frame.
        if (m.HasProperty(SurfaceId)) m.SetFloat(SurfaceId, 1f);
        if (m.HasProperty(BlendId)) m.SetFloat(BlendId, 0f);
        if (m.HasProperty(ZWriteId)) m.SetFloat(ZWriteId, 0f);
        if (m.HasProperty(SrcBlendId)) m.SetFloat(SrcBlendId, (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (m.HasProperty(DstBlendId)) m.SetFloat(DstBlendId, (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetOverrideTag("RenderType", "Transparent");
        m.renderQueue = 3000;
        m.DisableKeyword("_SURFACE_TYPE_OPAQUE");
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.EnableKeyword("_ALPHABLEND_ON"); // legacy Standard fallback
        return m;
    }

    public void SetHidden(bool hidden)
    {
        targetAlpha = hidden ? hiddenAlpha : 1f;
        enabled = true; // ensure LateUpdate ticks while we're animating
    }

    void LateUpdate()
    {
        if (Mathf.Approximately(currentAlpha, targetAlpha)) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        bool fullyVisible = currentAlpha >= 0.999f;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null) continue;

            if (fullyVisible)
            {
                if (usingTransparent[i])
                {
                    // Restore the original opaque materials — preserves lighting/batching when idle.
                    r.sharedMaterials = opaqueMaterials[i];
                    usingTransparent[i] = false;
                }
                continue;
            }

            if (!usingTransparent[i])
            {
                r.sharedMaterials = transparentMaterials[i];
                usingTransparent[i] = true;
            }

            Material[] mats = transparentMaterials[i];
            for (int j = 0; j < mats.Length; j++)
            {
                Material m = mats[j];
                if (m == null) continue;
                if (m.HasProperty(BaseColorId))
                {
                    Color c = m.GetColor(BaseColorId);
                    c.a = currentAlpha;
                    m.SetColor(BaseColorId, c);
                }
                else if (m.HasProperty(ColorId))
                {
                    Color c = m.GetColor(ColorId);
                    c.a = currentAlpha;
                    m.SetColor(ColorId, c);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (transparentMaterials == null) return;
        for (int i = 0; i < transparentMaterials.Length; i++)
        {
            Material[] mats = transparentMaterials[i];
            if (mats == null) continue;
            for (int j = 0; j < mats.Length; j++)
                if (mats[j] != null) Destroy(mats[j]);
        }
    }
}
