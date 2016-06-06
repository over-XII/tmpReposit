﻿using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    
    public Camera cameraPlayer;

    public float ShootPower = 500;

    [SerializeField]
    private Transform mainHand;

    private PhotonView view;
    
    [HideInInspector]
    public bool hasBall = false;
    
	void Start ()
    {
        view = GetComponentInParent<PhotonView>();
    }

	void Update () {
        //Debug.LogError(InputManager.Instance + "------" + GameObject.Find("InputManager"));
        if (hasBall && view.isMine && GameManager.Instance.ballOfGame.GetComponent<PhotonView>().ownerId == GetComponent<PhotonView>().viewID)
        {
            view.RPC("CarryBall", PhotonTargets.AllBuffered);

            if (InputManager.Instance.IsPassing)
            {
                if (PhotonNetwork.offlineMode) //ne va surment jamais étre utiliser car jeu est toujours Online .
                {
                    Debug.LogError("Be carefull you are offLine");
                }
                else
                {
                    view.RPC("ShootBall", PhotonTargets.AllBuffered);
                }
            }
        }
	}

    [PunRPC]
    private void ShootBall()
    {
        hasBall = false;
        GameManager.Instance.ballOfGame.GetComponent<Rigidbody>().isKinematic = false;
        Debug.Log(cameraPlayer.transform.forward);
        GameManager.Instance.ballOfGame.GetComponent<Rigidbody>().AddForce(cameraPlayer.transform.forward * ShootPower);
        //GameManager.Instance.ballOfGame.GetComponent<Collider>().enabled = true;
        Invoke("ResetOwnerBall", 0.5f);
    }

    private void ResetOwnerBall()
    {
        if (GameManager.Instance.ballOfGame != null)
        {
            GameManager.Instance.ballOfGame.GetComponent<BallBehaviour>().IDPreviousOwner = -1;
            Physics.IgnoreCollision(GameManager.Instance.ballOfGame.GetComponent<Collider>(), GetComponent<Collider>(),false);
        }
    }

    [PunRPC]
    private void CarryBall()
    {
        if (GameManager.Instance.ballOfGame != null)
            GameManager.Instance.ballOfGame.transform.position = mainHand.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(cameraPlayer.transform.position, cameraPlayer.transform.forward * ShootPower);
    }

}
