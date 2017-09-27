﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Matrix;
using PlayGroup;

/// <summary>
/// Toggles the active state of the object by gather all components and setting
/// their active state. It ignores network components so item can be synced
/// </summary>
public class VisibleBehaviour : NetworkBehaviour
{

	/// <summary>
	/// This will also set the enabled state of every component
	/// </summary>
	[SyncVar(hook = "UpdateState")]
	public bool visibleState = true;

	private bool isPlayer = false;
	private RegisterTile registerTile;

	//Ignore these types
	private const string networkId = "NetworkIdentity";
	private const string networkT = "NetworkTransform";
	private const string objectBehaviour = "ObjectBehaviour";
	private const string regTile = "RegisterTile";

	public override void OnStartClient()
	{
		StartCoroutine(WaitForLoad());
		base.OnStartClient();

		registerTile = GetComponent<RegisterTile>();
		PlayerScript pS = GetComponent<PlayerScript>();
		if (pS != null)
			isPlayer = true;
	}

	IEnumerator WaitForLoad()
	{
		yield return new WaitForSeconds(3f);
		UpdateState(visibleState);
	}

	void UpdateState(bool _aliveState)
	{
		MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>(true);
		Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
		Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < scripts.Length; i++) {
			if (scripts[i].GetType().Name != networkId && scripts[i].GetType().Name != networkT
				&& scripts[i].GetType().Name != objectBehaviour
			   && scripts[i].GetType().Name != regTile) {
				scripts[i].enabled = _aliveState;
			}
		}

		for (int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = _aliveState;
		}

		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].enabled = _aliveState;
		}

		if (registerTile != null) {
			if (_aliveState) {
				EditModeControl eC = gameObject.GetComponent<EditModeControl>();
				if (eC != null)
					eC.Snap();

				registerTile.UpdateTile(transform.position);
			}else{
				registerTile.RemoveTile();
			}
		}
	}
}