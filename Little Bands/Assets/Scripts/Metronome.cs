using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public double bpm = 120.0f;

    double nextTick = 0.0f;
    double sampleRate = 0.0f;
    bool ticked = false;

    // Start is called before the first frame update
    void Start()
    {


        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;

        nextTick = startTick + (60.0 / bpm);
    }

    void LateUpdate() {
        if(!ticked && nextTick >= AudioSettings.dspTime) {
            ticked = true;
            BroadcastMessage("OnTick");
        }
    }

    void OnTick() {
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void FixedUpdate() {
        double timePerTick = 60.0f / bpm;
        double dspTime = AudioSettings.dspTime;

        while (dspTime >= nextTick) {
            ticked = false;
            nextTick += timePerTick;
        }
    }
}
