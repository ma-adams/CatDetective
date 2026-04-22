//modified version of DA Lab's original code: https://github.com/DA-LAB-Tutorials/YouTube-Unity-Tutorials/blob/main/OutlineSelection.cs 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OutlineSelection : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public Color outlineColor = Color.magenta;
    public float outlineWidth = 7f;
    private List<Transform> selectables = new List<Transform>();

    void Start()
    {
        // Cache all selectable objects at start
        GameObject[] objs = GameObject.FindGameObjectsWithTag("selectable");
        foreach (GameObject obj in objs)
        {
            selectables.Add(obj.transform);
        }
    }

    void Update()
    {
        foreach (Transform obj in selectables)
        {
            float distance = Vector3.Distance(player.position, obj.position);
            bool isInside = distance <= detectionRadius;

            Outline outline = obj.GetComponent<Outline>();

            if (isInside)
            {
                if (outline == null)
                {
                    outline = obj.gameObject.AddComponent<Outline>();
                    outline.OutlineColor = outlineColor;
                    outline.OutlineWidth = outlineWidth;
                    outline.OutlineMode = Outline.Mode.OutlineAll;
                }

                outline.enabled = true;
            }
            else
            {
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
        }
    }
}
