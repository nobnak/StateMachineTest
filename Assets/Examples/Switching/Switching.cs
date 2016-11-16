using UnityEngine;
using System.Collections;
using StateMachine;

public class Switching : MonoBehaviour {
    enum SignalEnum { Green = 0, Red, Blue }

    public GameObject green;
    public GameObject red;
    public GameObject blue;

    FSM<SignalEnum> _fsmSignal;
    float _timer;

	void Awake() {
        _fsmSignal = new FSM<SignalEnum> (this);

        _fsmSignal.State (SignalEnum.Green).Enter ((f) => { 
            ToggleColor(f.Current);
            _timer = 1f;
        }).Update ((f) => {
            if ((_timer -= Time.deltaTime) <= 0f)
                f.Goto(SignalEnum.Red);            
        });

        _fsmSignal.State (SignalEnum.Red).Enter ((f) => {
            ToggleColor (f.Current);
            _timer = 1f;
        }).Update ((f) => {
            if ((_timer -= Time.deltaTime) <= 0f)
                f.Goto (SignalEnum.Blue);
        });

        _fsmSignal.State (SignalEnum.Blue).Enter ((f) => {
            ToggleColor (f.Current);
            _timer = 1f;
        }).Update ((f) => {
            if ((_timer -= Time.deltaTime) <= 0f)
                f.Goto(SignalEnum.Green);
        });

        _fsmSignal.Goto (SignalEnum.Green);
            
	}

    void ToggleColor (SignalEnum color) {
        green.SetActive (color == SignalEnum.Green);
        red.SetActive (color == SignalEnum.Red);
        blue.SetActive (color == SignalEnum.Blue);
    }

    void OnDestroy() {
        _fsmSignal.Dispose ();
    }
}
