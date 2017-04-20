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

/// <summary>
/// Audio handler - sets up audio input device(s), signal DSP w/ buffers, etc.
/// How to set up in Unity IDE:
/// 1) Create an Audio Source GameObject and link it to this class's "micSource" object
/// 2) Create a Mixer asset, add a group for "Microphone", expose the volume parameter of that
///    subgroup and call it "micPlaythruVol". 
/// 3) In the Audio Source settings, set "Output" to the Microphone mixer group.
/// 3) Link the Mixer to this class's "micMixer" object.
/// </summary>
/// 
/// 

public class MicHardware : MonoBehaviour {
	// -- Events/Delegates
	public delegate void MicInitializedAction ();
	public static event MicInitializedAction OnMicInitialized;

	// -- Scene objects in the Unity editor, must be linked
	public AudioSource micSource;
	public UnityEngine.Audio.AudioMixer micMixer;


	// -- Audio Interface settings
	public int micSampleRate = 44100;
	public int recSeconds = 2;
	public string micDeviceName;

	// -- used for events
	private bool initialized = false;

	// -- actual Microphone signal, updated every Update()
	public float[] micRawData;
	private int micBufferSize;



	// Use this for initialization

	void Start () {
		if (micSource == null)
			micSource = GetComponent<AudioSource> ();

		micDeviceName = null; //for default microphone

		micSource.clip = Microphone.Start (micDeviceName, true, recSeconds, micSampleRate);
		micSource.loop = true;

		while (!(Microphone.GetPosition (null) > 0)) {}

		micMixer.SetFloat("micPlaythruVol", -80.0f); //prevents microphone playing through speakers
		micSource.Play ();

		micBufferSize = 8192; // must be power of 2, max appears to be 16384
		micRawData = new float[micBufferSize];
	}


	public void UpdateMicSignal() {
		if (initialized) {
			// Do what you need in here with incoming Microphone signal
			// remember your buffer's length must be a power of two!

			micSource.GetOutputData(micRawData, 0); // left or mono channel
		}
	}

	// Update is called once per frame
	void Update () {
		if (micSource.isPlaying)
			initialized = true;

		if (initialized) {
			UpdateMicSignal ();
			UpdateEvents ();
		}
	}

	private void UpdateEvents() {
		if (initialized) {
			if (OnMicInitialized != null) {
				OnMicInitialized ();
				OnMicInitialized = null; // only clear the events chain when 
			} 
		}
	}

	// If you really want the raw audio data, MicHardware.FillData will give it to you.
	public void FillBufferWithMicrophone(float[] buffer) {
		micSource.GetOutputData (buffer, 0);
	}


	public string DebugText() {
		return System.String.Format(
			"MicDebug: Initialized={0:D}, Buffer Duration={1:F2}s, deviceName={2}",
			initialized, BufferDuration(), micDeviceName
			);
	}

	public float BufferDuration() {
		return (float)micBufferSize / (float)micSampleRate;
	}
}

// -- What devices are available
// public int totalDeviceCount;

