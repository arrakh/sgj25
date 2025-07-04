/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ToggleDisplayObject : MonoBehaviour
{

    public GameObject objectToShow;

    public void ShowPanel()
    {
        objectToShow.SetActive(true); 
    }

    public void HidePanel()
    {
        objectToShow.SetActive(false);
    }

    
}
