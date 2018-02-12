using nobnak.Gist.GPUBuffer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseGPUParticleSystemModule {

    public class BaseGPUParticleSystem : MonoBehaviour {
        public const string KERNEL_EMIT = "Emit";

        public const string PROP_COUNTER = "_Counter";
        public const string PROP_COUNTER_OFFSET = "_CounterOffset";

        public const string PROP_DEAD_CONSUME_LIST = "_DeadConsumeList";
        public const string PROP_ALIVE_APPEND_LIST = "_AliveAppendList";
        public const string PROP_UPLOAD_POSITION_LIST = "_UploadPositionList";

        public const string PROP_POSITION_LIST = "_PositionList";
        public const string PROP_ROTATION_LIST = "_RotationList";
        public const string PROP_OBJECT_TO_WORLD_LIST = "_ObjectToWorldList";
        public const string PROP_WORLD_TO_OBJECT_LIST = "_WorldToObjectList";

        [SerializeField] protected int capasity;
        [SerializeField] protected ComputeShader compute;
        [SerializeField] protected Mesh mesh;
        [SerializeField] protected Material mat;

        protected int kernelIDEmit;

        protected GPUList<Vector3> positionList;
        protected GPUList<Quaternion> rotationList;
        protected GPUList<Matrix4x4> objectToWorldList;
        protected GPUList<Matrix4x4> worldToObjectList;

        protected GPUList<uint> args;
        protected GPUList<uint> deadList;
        protected GPUList<uint> aliveList;
        protected GPUList<Vector3> uploadPositionList;

        #region Unity
        private void OnEnable() {
            this.kernelIDEmit = compute.FindKernel(KERNEL_EMIT);
            
            this.positionList = new GPUList<Vector3>(capasity);
            this.rotationList = new GPUList<Quaternion>(capasity);
            this.objectToWorldList = new GPUList<Matrix4x4>(capasity);
            this.worldToObjectList = new GPUList<Matrix4x4>(capasity);

            this.args = new GPUList<uint>(5, ComputeBufferType.IndirectArguments);
            this.deadList = new GPUList<uint>(capasity, ComputeBufferType.Append);
            this.aliveList = new GPUList<uint>(capasity, ComputeBufferType.Append);

            this.uploadPositionList = new GPUList<Vector3>();

            for (var i = 0; i < capasity; i++)
                deadList[i] = (uint)i;
            deadList.Buffer.SetCounterValue((uint)capasity);
            aliveList.Buffer.SetCounterValue(0);
        }
        private void OnDisable() {
            positionList.Dispose();
            rotationList.Dispose();
            objectToWorldList.Dispose();
            worldToObjectList.Dispose();

            args.Dispose();
            deadList.Dispose();
            aliveList.Dispose();
            uploadPositionList.Dispose();
        }
        private void Update() {
            var bounds = new Bounds(Vector3.zero, 1000f * Vector3.one);
            args[0] = mesh.GetIndexCount(0);
            ComputeBuffer.CopyCount(aliveList.Buffer, args.Buffer, 4);
            Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, bounds, args.Buffer, 0);
        }
        #endregion

        public void Emit(Vector3 p) {
            uploadPositionList.Clear();
            uploadPositionList.Add(p);

            var counterOffset = 0;
            ComputeBuffer.CopyCount(deadList.Buffer, args.Buffer, counterOffset);

            compute.SetBuffer(kernelIDEmit, PROP_POSITION_LIST, positionList.Buffer);

            compute.SetInt(PROP_COUNTER_OFFSET, counterOffset);
            compute.SetBuffer(kernelIDEmit, PROP_COUNTER, args.Buffer);
            compute.SetBuffer(kernelIDEmit, PROP_DEAD_CONSUME_LIST, deadList.Buffer);
            compute.SetBuffer(kernelIDEmit, PROP_ALIVE_APPEND_LIST, aliveList.Buffer);

            compute.SetBuffer(kernelIDEmit, PROP_UPLOAD_POSITION_LIST, uploadPositionList.Buffer);

            compute.Dispatch(kernelIDEmit, uploadPositionList.Count, 1, 1);

#if false
            ComputeBuffer.CopyCount(deadList.Buffer, args.Buffer, 0);
            Debug.LogFormat("Dead list count={0}, ({1})",
                args.Data[0], Join(",", deadList.Data));
            ComputeBuffer.CopyCount(aliveList.Buffer, args.Buffer, 0);
            Debug.LogFormat("Alive list count={0}, ({1})", 
                args.Data[0], Join(",", aliveList.Data));
#endif

        }

        public static string Join<T>(string seperater, T[] tt) {
            return string.Join(",", tt.Select(v => v.ToString()).ToArray());
        }
    }
}
