using UnityEngine;
using System.Collections.Generic;

public class WallTransparency : MonoBehaviour
{
    public Transform target;
    public float transparentAlpha = 0.15f;

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    private List<Renderer> currentlyTransparent = new();

    void LateUpdate()
    {
        // Restore all previously transparent walls
        foreach (Renderer r in currentlyTransparent)
        {
            if (r != null)
                r.materials = originalMaterials[r];
        }
        currentlyTransparent.Clear();

        // Raycast from camera to cat
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance);

        foreach (RaycastHit hit in hits)
        {
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (r == null) continue;

            // Save original materials if first time seeing this renderer
            if (!originalMaterials.ContainsKey(r))
                originalMaterials[r] = r.materials;

            // Make transparent copies
            Material[] transparentMats = new Material[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                transparentMats[i] = new Material(r.materials[i]);
                transparentMats[i].SetFloat("_Surface", 1); // URP transparent
                transparentMats[i].SetFloat("_Blend", 0);
                transparentMats[i].SetOverrideTag("RenderType", "Transparent");
                transparentMats[i].renderQueue = 3000;
                Color c = transparentMats[i].color;
                c.a = transparentAlpha;
                transparentMats[i].color = c;
            }
            r.materials = transparentMats;
            currentlyTransparent.Add(r);
        }
    }
}