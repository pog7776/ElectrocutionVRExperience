﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour{

    #region Anim Names
    [Header("Anim Names")]
    // The names of the animations to play for each function
    [SerializeField]
    string triggerAnim = "Pin3";
    [SerializeField]
    string modeAnim = "Pin5";
    [SerializeField]
    string channelAnim = "Pin6";
    #endregion

    #region Shock
    [Header("Shock")]
    [SerializeField]
    private bool testShock = false;
    #endregion

    #region Channels
    [Header("Channels")]
    [SerializeField]
    private bool switchChannel = false;
    private int channel = 1;
    #endregion

    #region Other Stuff
    [Header("Other Stuff")]
    [SerializeField]
    private GameObject AurduinoControllerObj;
    [SerializeField]
    private bool runTest = false;
    #endregion

    #region Private Variables
    private Animator animator;
    // private Kreation.Firmata.OnOffController ArduinoController;
    #endregion

    // Start is called before the first frame update
    void Start(){
        animator = AurduinoControllerObj.GetComponent<Animator>();
        //ArduinoController = AurduinoControllerObj.GetComponent<Kreation.Firmata.OnOffController>();
    }

    // Update is called once per frame
    void Update(){

        // Run a test coroutine
        if(runTest){
            StartCoroutine(Test());
            runTest = false;
        }

        // Trigger the shocker
        if(testShock){
            Trigger();
            testShock = false;
        }

        // Switch channel to shock
        if(switchChannel){
            SwitchChannel();
            switchChannel = false;
        }
    }

    /// <summary>
    /// Triggers the collar
    /// </summary>
    public void Trigger(){
        animator.SetTrigger(triggerAnim);
    }

    /// <summary>
    /// Changes the channel to given channel
    /// </summary>
    /// <param name="to">Channel to change to.</param>
    public void ChangeChannel(int to){
        if(channel != to){
            channel = to;
            animator.SetTrigger(channelAnim);
        }
    }

    // Switches channel regardless of the current channel
    private void SwitchChannel(){
        if(channel == 1){
            ChangeChannel(2);
        }
        else{
            ChangeChannel(1);
        }
    }

    private IEnumerator Test(){
        ChangeChannel(1);
        Debug.Log("Setting channel to 1");
        yield return new WaitForSeconds(1);
        Debug.Log("Setting channel to 2");
        Debug.Log("3");
        yield return new WaitForSeconds(1);
        Debug.Log("2");
        yield return new WaitForSeconds(1);
        Debug.Log("1");
        yield return new WaitForSeconds(1);
        ChangeChannel(2);
        Debug.Log("Changing back to 1 in 3 seconds");
        yield return new WaitForSeconds(3);
        ChangeChannel(1);
        Debug.Log("Zapping!");
        yield return new WaitForSeconds(1);
        Trigger();
    }
}