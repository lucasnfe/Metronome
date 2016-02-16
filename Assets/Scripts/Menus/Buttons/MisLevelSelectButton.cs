using UnityEngine;
using System.Collections;

public class MisLevelSelectButton : MisButton {

	protected override void LoadAction () {

		// Call instance to load game world
		MisGameWorld.Instance.LoadPlayableLevel();
	}
}
