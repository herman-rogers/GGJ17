﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public static int iCountKilled = 0;
    public float attackCooldown = 0;

    private PlayerController controllerHandle;
    private GameObject killPromptGameObject;
    private GameObject currentKillTargetHandle;

	void Start ()
    {
		controllerHandle = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        killPromptGameObject = GameObject.FindGameObjectWithTag(Constants.PlayerKillPromptTag);

        currentKillTargetHandle = null;

        if(!controllerHandle)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        if(!killPromptGameObject)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        killPromptGameObject.SetActive(false);
	}
	
	void Update ()
    {
		if( attackCooldown>0 )
        {
            attackCooldown -= Time.deltaTime;
        }
	}

    void OnTriggerStay(Collider other)
    {
        if(Constants.bGameOver)
        {
            return;
        }

        if(other.tag == Constants.CustomerTag)
        {
            CustomerAI tempAiHandle = other.GetComponent<CustomerAI>();

            if(tempAiHandle.isDead)
            {
                return;
            }

            if(currentKillTargetHandle == null)
            {
                currentKillTargetHandle = other.gameObject;
            }

            //if closer - update target


            if(other.gameObject == currentKillTargetHandle)
            {
                //set the prompt active
                if(!killPromptGameObject.activeSelf)
                {
                    killPromptGameObject.SetActive(true);
                }

                //update prompt pos
                if(killPromptGameObject.activeSelf)
                {
                    killPromptGameObject.transform.position = currentKillTargetHandle.transform.position + new Vector3(0, 1, 0);
                }

                //in attacked, reset and kill the target
                if(controllerHandle.QueryPlayerInput(Constants.InputType.PIT_ATTACK) && attackCooldown <= 0)
                {
                    attackCooldown = Constants.AttackCooldown;
                    controllerHandle.OnHit();
                    tempAiHandle.Die(true);
                    iCountKilled++;

                    controllerHandle.Rage_KilledCustomer();

                    killPromptGameObject.SetActive(false);
                    currentKillTargetHandle = null;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == Constants.CustomerTag)
        {
            if(other.gameObject == currentKillTargetHandle)
            {
                currentKillTargetHandle = null;
                killPromptGameObject.SetActive(false);
            }
        }
    }
}
