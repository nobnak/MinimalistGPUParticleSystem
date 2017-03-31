using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[ExecuteInEditMode]
public class GPUParticleRenderer : MonoBehaviour {
    [SerializeField]
    string bufferName = "particles";
    [SerializeField]
    Material mat;

	public GPUList<Particle> Particles { get; private set; }

    #region Unity
    void OnEnable() {
        Particles = new GPUList<Particle> ();
    }
    void OnRenderObject() {
        if (Particles != null && Particles.Count > 0) {
            mat.SetPass (0);
            mat.SetBuffer (bufferName, Particles.GPUBuffer);
            Graphics.DrawProcedural (MeshTopology.Points, Particles.Count, 1);
        }
    }
    void OnDisable() {
		if (Particles != null)
        	Particles.Dispose ();
    }
    #endregion


    [StructLayout(LayoutKind.Sequential)]
    public struct Particle {
		public readonly Vector3 pos;
		public readonly float size;

		public Particle(Vector3 pos, float size) {
			this.pos = pos;
			this.size = size;
		}
    }
}
