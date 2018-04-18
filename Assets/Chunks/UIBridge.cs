using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBridge {

    private static UIBridge instance;

    public static UIBridge Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new UIBridge();
            }
            return instance;
        }
    }

    public delegate void onBlockSelectionChange(int type);
    public onBlockSelectionChange OnBlockSelectionChange;

}
