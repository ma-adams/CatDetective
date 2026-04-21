//modified version of DA Lab's original code: https://github.com/DA-LAB-Tutorials/YouTube-Unity-Tutorials/blob/main/OutlineSelection.cs 

//MIT License
//Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OutlineSelection : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    public Transform player;
    public float detectionRadius = 5f;
    private bool isPlayerNearby = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (Mouse.current == null || player == null)
            return;

        // Highlight
        if (highlight != null)
        {
            Outline oldOutline = highlight.GetComponent<Outline>();
            if (oldOutline != null && highlight != selection)
            {
                oldOutline.enabled = false;
            }
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;

            if (highlight.CompareTag("selectable") && highlight != selection)
            {
                Outline outline = highlight.GetComponent<Outline>();

                if (outline == null)
                {
                    outline = highlight.gameObject.AddComponent<Outline>();
                    outline.OutlineColor = Color.magenta;
                    outline.OutlineWidth = 7.0f;
                }

                outline.enabled = true;
            }
            else
            {
                highlight = null;
            }
        }
    }
        /*
        // Selection
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (highlight != null)
            {
                if (selection != null)
                {
                    Outline selectionOutline = selection.GetComponent<Outline>();
                    if (selectionOutline != null)
                        selectionOutline.enabled = false;
                }

                selection = highlight;
                Outline outline = selection.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = true;

                highlight = null;
            }
            else
            {
                if (selection != null)
                {
                    Outline selectionOutline = selection.GetComponent<Outline>();
                    if (selectionOutline != null)
                        selectionOutline.enabled = false;

                    selection = null;
                }
            }
        } */
}