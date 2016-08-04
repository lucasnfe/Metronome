using UnityEngine;
using System.Collections;

public class MisLevelSelectButton : MisButton {

	protected override void ActionBeween () {

		// Call instance to load game world
		MisGameWorld.Instance.LoadPlayableLevel();
	}

	protected override void ActionAfter() {

		// Call instance to load game world
		MisGameWorld.Instance.SetupLevel();
	}
}
