/*
Copyright (c) 2017, Music Star Games/Marc Gelfo (dev@musicstargames.com) All rights reserved.

Seriously fun music apps: play like a Music Star with our games and practice software.
http://musicstargames.com/

Released under BSD-3-Clause license: https://opensource.org/licenses/BSD-3-Clause

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicDemo : MonoBehaviour {

	// -- Connect to objects in the Editor
	public AudioSource micSource;
	public GameObject volumeCube;
	public Text debugText;

	// -- Control the height of the volume cube
	public float yScale = 4;

	// -- Internals
	private MicHardware currMic;
	private float currVolume;
	private int bufferLength;


	// Start is called once for initialization
	void Start() {
		// Connect with the active MicHardware component
		currMic = micSource.GetComponent<MicHardware> ();
	}


	// Update is called once per frame
	void Update () {
		currVolume = CalcAverageVolume ();

		volumeCube.transform.position = 
			new Vector3 (0,  currVolume * yScale, 0);

		debugText.text = currMic.DebugText ();
	}


	// CalcAverageVolume() returns RMS volume for entire audio buffer
	float CalcAverageVolume() {
		float currSample;
		float sum = 0;
		float rootMeanSquared;

		for (int i = 0; i < currMic.micRawData.Length; i++) {
			currSample = currMic.micRawData [i];
			sum += currSample * currSample;
		}

		rootMeanSquared = Mathf.Sqrt (sum);

		return rootMeanSquared;
	}
}
