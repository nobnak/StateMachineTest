using UnityEngine;
using System.Collections;
using StateMachine;

public class Switching : MonoBehaviour {
    enum SignalEnum { Green = 0, Red }

    public GameObject green;
    public GameObject red;

    FSM<SignalEnum> _fsmSignal;
    float _timer;

	void Awake() {
        _fsmSignal = new FSM<SignalEnum> (this);
        _fsmSignal.Ensure (SignalEnum.Green).SetEnter ((f) => { 
            green.SetActive (true);
            red.SetActive (false);
            _timer = 1f;
        }).SetUpdate ((f) => {
            if ((_timer -= Time.deltaTime) <= 0f)
                f.Goto(SignalEnum.Red);            
        });
        _fsmSignal.Ensure (SignalEnum.Red).SetEnter ((f) => {
            green.SetActive (false);
            red.SetActive (true);
            _timer = 1f;
        }).SetUpdate ((f) => {
            if ((_timer -= Time.deltaTime) <= 0f)
                f.Goto(SignalEnum.Green);
        });
        _fsmSignal.Goto (SignalEnum.Green);
            
	}
    void OnDestroy() {
        _fsmSignal.Dispose ();
    }
}
