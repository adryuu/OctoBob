using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPanelButton : MonoBehaviour
{
    //Id del panel que se muestra
    public string PanelId;

    public PanelShowBehaviour Behaviour;

    private PanelManager _panelManager;

    private void Start()
    {
        _panelManager = PanelManager.Instance;
    }

    public void DoShowPanel()
    {
        _panelManager.ShowPanel(PanelId, Behaviour);
    }
}
