using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    protected bool _isOpen;
    public virtual void OpenPanel() 
    {
        if (_isOpen)
            return;

        this.gameObject.SetActive(true);
        _isOpen = true;
    }
    public virtual void ClosePanel()
    {
        if (_isOpen == false)
            return;

        this.gameObject.SetActive(false);
        _isOpen = false;
    }
}
