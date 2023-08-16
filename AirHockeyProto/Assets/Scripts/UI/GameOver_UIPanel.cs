using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOver_UIPanel : UIPanel
{
    public Button RetryButton;
    public override void OpenPanel()
    {
        base.OpenPanel();
        if(_isOpen)
            RetryButton?.onClick.AddListener(() => OnRetryClicked() );
    }
    public override void ClosePanel()
    {
        if (_isOpen)
            RetryButton?.onClick.RemoveAllListeners();

        base.ClosePanel();
    }
    void OnRetryClicked() 
    {
        AirHockeyUIManager auim = FindObjectOfType<AirHockeyUIManager>();
        if (auim != null)
            auim.ReportResetClicked();
    }
}
