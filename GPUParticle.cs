using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace BasicGPUParticle {

    [StructLayout(LayoutKind.Sequential)]
    public struct GPUParticle {
        public readonly Matrix4x4 model;

        public GPUParticle(Matrix4x4 model) {
            this.model = model;
        }

        public static explicit operator GPUParticle(Transform tr) {
            return new GPUParticle (tr.localToWorldMatrix);
        }
        public static GPUParticle Convert(Vector3 pos, Quaternion rot, Vector3 scale) {
            return new GPUParticle (Matrix4x4.TRS (pos, rot, scale));
        }
    }
}
