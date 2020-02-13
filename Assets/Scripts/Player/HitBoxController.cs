using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour{

    #region Singleton
    private static HitBoxController _instance;
	public static HitBoxController Instance { get { return _instance; } }
    #endregion

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
    float timeBetweenShocks = 20;

    [SerializeField]
    private bool testShock = false;

    private bool canShock = true;
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
    private VRShooterKit.VR_Manager vR_Manager;

    [SerializeField]
    private GameObject AurduinoControllerObj;
    
    [SerializeField]
    private bool runTest = false;
    #endregion

    #region Private Variables
    private Animator animator;
    // private Kreation.Firmata.OnOffController ArduinoController;
    #endregion


    private VRShooterKit.VR_ControllerInfo rightControllerInfo;

    private VRShooterKit.VR_Controller rightController = null;
    private VRShooterKit.VR_Controller leftController = null;



    private void Awake() {
        SetSingleton();
    }

    // Start is called before the first frame update
    void Start(){
        animator = AurduinoControllerObj.GetComponent<Animator>();
        canShock = true;

        rightController = VRShooterKit.VR_Manager.instance.RightController;
        leftController = VRShooterKit.VR_Manager.instance.LeftController;

        // ! Super important input from controller
        // if (rightController.Input.GetButtonDown( VRShooterKit.VR_InputButton.Button_Secondary )){
        //     // Do the thing
        // }



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

    private void SetSingleton() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}

    /// <summary>
    /// Triggers the collar
    /// </summary>
    public void Trigger(){

        // Ensure you can't be shocked
        if (rightController.Input.GetButtonDown( VRShooterKit.VR_InputButton.Button_Secondary ) || leftController.Input.GetButtonDown( VRShooterKit.VR_InputButton.Button_Secondary )){
            canShock = false;
        }
        else{
            canShock = true;
        }

        if(canShock){
            canShock = false;
            animator.SetTrigger(triggerAnim);
            StartCoroutine(WaitForShock(timeBetweenShocks));
        }
    }

    private IEnumerator WaitForShock(float time){
        yield return new WaitForSeconds(time);
        canShock = true;
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
