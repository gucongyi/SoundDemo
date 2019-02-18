using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMusic : MonoBehaviour {
    public Button buttonPlayerSFX2D;
    public Button buttonPlayerSFX3D;
    public Transform trans3dPos;
	// Use this for initialization

    void Start () {
        buttonPlayerSFX2D.onClick.AddListener(OnButtonPlayerSFX2DClick);
        buttonPlayerSFX3D.onClick.AddListener(OnButtonPlayerSFX3DClick);
    }

    public void OnButtonPlayerSFX2DClick()
    {
        GameSoundPlayer.Instance.PlaySoundEffect("laugh2D");
    }
    public void OnButtonPlayerSFX3DClick()
    {
        var pos3D = trans3dPos.localPosition;
        GameSoundPlayer.Instance.PlaySoundEffect("water", pos3D);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
