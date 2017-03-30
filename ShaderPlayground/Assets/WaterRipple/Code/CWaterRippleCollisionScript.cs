using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWaterRippleCollisionScript : MonoBehaviour {
    private readonly string S_OFFSET = "_Offset";
    private readonly string S_WAVE_AMPLITUDE = "_WaveAmplitude";
    private readonly string S_IMPACT = "_Impact";
    private readonly string S_DRAW_DISTANCE = "_DrawDistance";
    private readonly string S_TIMER = "_TIMER";

    private Renderer r;

    private float[] faWaveAmplitude = new float[8];
    private float[] faDrawDistance = new float[8];
    private Vector4[] vec4aOffset = new Vector4[8];
    private Vector4[] vec4aImpact = new Vector4[8];

    private float fTimer;
    private int iWaveNumber = 0;

    public float fMagnitudeDivider = 0.1f;

    void Start() {
        r = GetComponent<Renderer>();
    }

    void Update() {
        for (int index = 0; index < faWaveAmplitude.Length; index++) {
            if (faWaveAmplitude[index] > 0.01f) {
                faWaveAmplitude[index] *= .98f;
                faDrawDistance[index] += .05f;
            } else {
                faWaveAmplitude[index] = 0f;
                faDrawDistance[index] = 1f;
            }
        }

        fTimer += Time.deltaTime;
        UpdateShaderFloats();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody) {
            if (collision.contacts.Length > 0) {
                Vector3 collisionPoint = collision.contacts[0].point;
                vec4aOffset[iWaveNumber] = transform.InverseTransformPoint(collisionPoint) * -2f;
                vec4aImpact[iWaveNumber] = transform.InverseTransformPoint(collisionPoint);
                faWaveAmplitude[iWaveNumber] = collision.rigidbody.velocity.magnitude * fMagnitudeDivider;
                faDrawDistance[iWaveNumber] = 1f;

                iWaveNumber++;
                if (iWaveNumber > 7) {
                    iWaveNumber = 0;
                }
            }
        }
    }

    private void UpdateShaderFloats() {
        r.material.SetFloat(S_TIMER, Mathf.Repeat(fTimer, 1.0f));
        r.material.SetFloatArray(S_WAVE_AMPLITUDE, faWaveAmplitude);
        r.material.SetFloatArray(S_DRAW_DISTANCE, faDrawDistance);
        r.material.SetVectorArray(S_OFFSET, vec4aOffset);
        r.material.SetVectorArray(S_IMPACT, vec4aImpact);
    }
}