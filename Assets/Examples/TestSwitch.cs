using StateMashineSys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSwitch : MonoBehaviour {

    public Link link = new Link();
    public Tuner tuner = new Tuner();

    protected StateMachine<StateEnum> fsm;
    protected float start_time;

    protected Rect win_rect;
    protected bool clicked;

    #region unity
    private void OnEnable() {
        win_rect = new Rect(10, 10, 10, 10);
        fsm = new StateMachine<StateEnum>();

        fsm.State(StateEnum.Init).Update(() => {
            if (clicked)
                fsm.Change(StateEnum.Started);
            clicked = false;
        });

        fsm.State(StateEnum.Started).Enter(() => {
            start_time = Time.time;
            foreach (var r in link.cubes)
                r.enabled = false;
        }).Update(() => fsm.Change(StateEnum.Processing));

        fsm.State(StateEnum.Processing).Update(() => {
            var elapsed = Time.time - start_time;
            var active_index = Mathf.FloorToInt(elapsed * link.cubes.Count / tuner.duration);
            for (var i = 0; i < link.cubes.Count; i++)
                link.cubes[i].enabled = (i == active_index);
            if (active_index >= link.cubes.Count)
                fsm.Change(StateEnum.Finished);
        });

        fsm.State(StateEnum.Finished).Enter(() => {
            start_time = Time.time;
            foreach (var r in link.cubes) r.enabled = true;
        }).Update(() => {
            var elapsed = Time.time - start_time;
            if (elapsed > 1f) {
                foreach (var r in link.cubes) r.enabled = false;
                fsm.Change(StateEnum.Init);
            }
        });

		fsm.Wire(StateEnum.Init, StateEnum.Started)
			.Wire(StateEnum.Processing)
			.Wire(StateEnum.Finished)
			.Wire(StateEnum.Init);

		if (!fsm.Change(StateEnum.Init))
            Debug.LogError($"Somthing wrong");
    }
    private void Update() {
        fsm.Update();
    }
    private void OnGUI() {
        win_rect = GUILayout.Window(GetInstanceID(), win_rect, Window, name);
    }
    #endregion

    #region member
    void Window(int id) {
        using (new GUILayout.VerticalScope()) {
            GUI.enabled = fsm.CurrState == StateEnum.Init;
            clicked = GUILayout.Button("Start");
        }
        GUI.DragWindow();
    }
    #endregion

    #region classes
    public enum StateEnum { Init = 0, Started, Processing, Finished }

    [System.Serializable]
    public class Link {
        public List<Renderer> cubes = new List<Renderer>();
    }
    [System.Serializable]
    public class Tuner {
        public float duration = 10f;
    }
    #endregion
}