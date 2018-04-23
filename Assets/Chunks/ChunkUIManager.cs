using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChunkUIManager : MonoBehaviour {

    public CubeModel cubeModel;
    public RawImage blockTypeImage;
    public Texture blockAtlas;



    private void Start()
    {
        UIBridge.Instance.OnBlockSelectionChange += OnBlockTypeChange;
        blockTypeImage.texture = blockAtlas;
        Rect uv = blockTypeImage.uvRect;
        uv.width = 1.0f / cubeModel.atlasSize.x;
        uv.height = 1.0f / cubeModel.atlasSize.y;
        blockTypeImage.uvRect = uv;
        OnBlockTypeChange(1);
    }

    public void OnBlockTypeChange(int type)
    {
        Rect uv = blockTypeImage.uvRect;
        uv.x = ((type - 1) % cubeModel.atlasSize.x) * (1.0f / cubeModel.atlasSize.x);
        uv.y = (cubeModel.atlasSize.x - 1 - (int)((type - 1) / cubeModel.atlasSize.x)) * (1.0f / cubeModel.atlasSize.y);
        blockTypeImage.uvRect = uv;
    }

}
