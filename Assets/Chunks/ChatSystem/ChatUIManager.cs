using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChatUIManager : NetworkBehaviour {

    public float textGridShowTime = 3.0f;

    public InputField chatField;
    public RectTransform chatTextGrid;
    public GameObject chatTextGridPanel;
    public GameObject chatTextPrefab;

    private bool submitDebounce = false;
    private float textGridHideTimer = 0;


    


    private void Start()
    {
        if (isServer)
        {
            UIBridge.Instance.DisperseChatMessage += ReceiveChatMessage;
        }

        if (!isLocalPlayer)
        {
            return;
        }
        chatField.gameObject.SetActive(false);
        chatTextGridPanel.SetActive(false);
    }

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(!UIBridge.Instance.uiIntercept && textGridHideTimer > 0)
        {
            textGridHideTimer -= Time.deltaTime;
            if(textGridHideTimer <= 0)
            {
                chatTextGridPanel.SetActive(false);
            }
        }

        if(Input.GetAxisRaw("Submit") == 0 && submitDebounce)
        {
            submitDebounce = false;
        }

        if(Input.GetAxisRaw("Submit") > 0 && !submitDebounce)
        {
            if (UIBridge.Instance.uiIntercept)
            {
                chatField.gameObject.SetActive(false);
                if(chatField.text != "")
                {
                    CmdSendChatMessage(Network.player.ipAddress + ": " + chatField.text);
                    chatField.text = "";
                    textGridHideTimer = textGridShowTime;
                }
                else if(textGridHideTimer <= 0)
                {
                    chatTextGridPanel.SetActive(false);
                }
            }
            else
            {
                chatField.gameObject.SetActive(true);
                chatTextGridPanel.SetActive(true);
                chatField.ActivateInputField();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            submitDebounce = true;
            UIBridge.Instance.uiIntercept = !UIBridge.Instance.uiIntercept;
        }
    }

    [Command]
    private void CmdSendChatMessage(string txt)
    {
        //RpcReceiveChatMessage(txt);
        UIBridge.Instance.DisperseChatMessage(txt);
    }

    public void ReceiveChatMessage(string txt)
    {
        RpcReceiveChatMessage(txt);
    }

    [ClientRpc]
    private void RpcReceiveChatMessage(string txt)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        chatTextGridPanel.SetActive(true);
        textGridHideTimer = textGridShowTime;

        GameObject chattxt = Instantiate(chatTextPrefab);
        chattxt.transform.parent = chatTextGrid;
        chattxt.GetComponent<Text>().text = txt;
    }

}
