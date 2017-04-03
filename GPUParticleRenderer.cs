using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicGPUParticle {

    [ExecuteInEditMode]
    public class GPUParticleRenderer : MonoBehaviour {
        [SerializeField]
        string bufferName = "particles";
        [SerializeField]
        Material mat;

    	public GPUList<GPUParticle> Particles { get; private set; }

        #region Unity
        void OnEnable() {
            Particles = new GPUList<GPUParticle> ();
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

    }
}
