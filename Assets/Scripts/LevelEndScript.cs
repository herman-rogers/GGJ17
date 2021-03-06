﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndScript : MonoBehaviour
{
    public GameObject pauseButton;

    private bool bSetEndScriptUp = false;

    void Update()
    {
        if( Constants.bGameOver && pauseButton.activeInHierarchy )
        {
            pauseButton.SetActive(false);
        }
        if( Constants.bGameOver && !bSetEndScriptUp )
        {
            bSetEndScriptUp = true;
            foreach( Transform child in transform )
            {
                //Constants.sFailReason
                if( child.tag == Constants.EndScoreUITag )
                {
                    GameObject.FindGameObjectWithTag(Constants.PlayerTag).GetComponent<PlayerController>().fPlayerScore *= Mathf.Max(1, PlayerAttack.iCountKilled / 2);

                    child.GetComponent<Text>().text = "sCORE: " + GameObject.FindGameObjectWithTag(Constants.PlayerTag).GetComponent<PlayerController>().fPlayerScore;
                }

                if(child.tag == "EndReason")
                {
                    child.GetComponent<Text>().text = Constants.sFailReason.ToUpper();
                }

                child.gameObject.SetActive(true);
            }
        }
    }
}
