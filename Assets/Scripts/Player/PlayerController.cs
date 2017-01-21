﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Constants.PlayerState ePlayerState;
    private GameObject goCurrentInteractionGO;
    private CustomerQueue goCurrentQueueHandle;
    private InteractionPoint currentInteractionHandle;

    //till processing
    private float fTimeAtTill;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePlayerMovement();

        switch (ePlayerState)
        {
            case Constants.PlayerState.PS_USING_TILL:
            {
                TaskProcessCustomerQueue();
                break;
            }
        }
	}

    void HandleInteraction(GameObject InteractGO)
    {
        currentInteractionHandle = InteractGO.GetComponent<InteractionPoint>();

        if(currentInteractionHandle)
        {
            goCurrentInteractionGO = InteractGO;

            switch (currentInteractionHandle.eInteractionType)
            {
                case Constants.InteractionPointType.IPT_FOOD_PRODUCT:
                {
                    TaskPickupFoodItem(InteractGO);
                    break;
                }
                case Constants.InteractionPointType.IPT_CASHIER_TILL:
                {
                    ePlayerState = Constants.PlayerState.PS_USING_TILL;
                    goCurrentQueueHandle = goCurrentInteractionGO.GetComponent<CustomerQueue>();
                    currentInteractionHandle.SetInUse(true);
                    fTimeAtTill = 0;
                    break;
                }
            }
        }
    }

    void TaskPickupFoodItem(GameObject go)
    {

    }

    void TaskProcessCustomerQueue()
    {
        if (goCurrentInteractionGO)
        {
            //If we've moved away from the interaction till, we stop processing it
            if (Vector3.Distance(goCurrentInteractionGO.transform.position, this.transform.position) > currentInteractionHandle.fInteractionRadius)
            {
                CleanupInteraction();
            }
            else
            {
                fTimeAtTill += Time.deltaTime;
                currentInteractionHandle.AddProgress(Constants.PlayerTillIncreasePerTick * Time.deltaTime);

                //done with the current customer!
                if (currentInteractionHandle.HasProgressCompleted())
                {
                    currentInteractionHandle.ResetProgress();
                    this.GetComponent<PlayerRage>().Rage_ProcessedCustomerItems();
                    goCurrentQueueHandle.ReleaseCurrentCustomer();

                    //this.GetComponent<PlayerRage>().Score_ProcessedCustomerItems();
                }
            }
        }
        else
        {
            CleanupInteraction();
        }
    }


    void CleanupInteraction()
    {
        currentInteractionHandle.SetInUse(false);
        ePlayerState = Constants.PlayerState.PS_IDLE;
        goCurrentQueueHandle = null;
        currentInteractionHandle = null;
    }




    void UpdatePlayerMovement()
    {
        if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_UP))
        {
            this.transform.position += new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
        else if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_DOWN))
        {
            this.transform.position -= new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up*180), Constants.PlayerRotationSpeed * Time.deltaTime);
        }

        if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_LEFT))
        {
            this.transform.position -= new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.down*90), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
        else if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_RIGHT))
        {
            this.transform.position += new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up*90), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
    }
}
