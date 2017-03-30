using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[ExecuteInEditMode]
public class GPUParticle : MonoBehaviour {
    [SerializeField]
    string bufferName = "particles";
    [SerializeField]
    Material mat;

    GPUList<Particle> particles;

    #region Unity
    void Awake() {
        particles = new GPUList<Particle> ();
    }
    void OnRenderObject() {
        if (particles == null && particles.Count > 0) {
            mat.SetPass (0);
            mat.SetBuffer (bufferName, particles.GPUBuffer);
            Graphics.DrawProcedural (MeshTopology.Points, particles.Count, 1);
        }
    }
    void OnDestroy() {
        particles.Dispose ();
    }
    #endregion


    [StructLayout(LayoutKind.Sequential)]
    public struct Particle {
        public Vector3 pos;
        public float size;
    }
}
