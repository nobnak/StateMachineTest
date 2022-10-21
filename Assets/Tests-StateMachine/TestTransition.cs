using System.Collections;
using System.Collections.Generic;
using MinimalStateMashine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTransition {

	public enum States { Init = 0, State1, State2, State3, Finish }

	[Test]
	public void TestTransitionSimplePasses() {

		var sm = new StateMachine<States>();
		sm.State(States.Init)
			.Update(() => sm.Transit(States.State1));
		sm.State(States.State1)
			.Enter(() => sm.Transit(States.State2));
		sm.State(States.State2)
			.Enter(() => sm.Transit(States.State3))
			.Exit(() => {
				sm.Transit(States.State3);
				Assert.Fail("Overrun");
			});
		sm.State(States.State3)
			.Enter(() => sm.Transit(States.Finish));
		sm.State(States.Finish);

		sm.Wire(States.Init, States.State1)
			.Wire(States.State2)
			.Wire(States.State3)
			.Wire(States.Finish);

		sm.Unhandled += e => Debug.Log($"Unhandled exception:\n{e}");

		sm.Transit(States.Init);
		Assert.AreEqual(States.Init, sm.CurrState);

		for (var i = 0; i < 10; i++)
			sm.Update();
		Assert.AreEqual(States.Finish, sm.CurrState);
	}
}

