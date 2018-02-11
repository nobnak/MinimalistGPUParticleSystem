using nobnak.Gist.GPUBuffer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseGPUParticleSystemModule {

    public class BaseGPUParticleSystem : System.IDisposable {
        public const string KERNEL_EMIT = "Emit";

        public const string PROP_POSITION_LIST = "_PositionList";

        public const string PROP_COUNTER = "_Counter";
        public const string PROP_DEAD_CONSUME_LIST = "_DeadConsumeList";
        public const string PROP_ALIVE_APPEND_LIST = "_AliveAppendList";

        public const string PROP_UPLOAD_POSITION_LIST = "_UploadPositionList";

        protected readonly ComputeShader compute;
        protected readonly int kernelIDEmit;

        protected int capasity;
        protected GPUList<Vector3> positionList;

        protected GPUList<uint> counter;
        protected GPUList<uint> deadList;
        protected GPUList<uint> aliveList;

        protected GPUList<Vector3> uploadPositionList;

        public BaseGPUParticleSystem(ComputeShader compute, int capasity) {
            this.compute = compute;
            this.kernelIDEmit = compute.FindKernel(KERNEL_EMIT);

            this.capasity = capasity;
            this.positionList = new GPUList<Vector3>(capasity);

            this.counter = new GPUList<uint>(1, ComputeBufferType.IndirectArguments);
            this.deadList = new GPUList<uint>(capasity, ComputeBufferType.Append);
            this.aliveList = new GPUList<uint>(capasity, ComputeBufferType.Append);

            this.uploadPositionList = new GPUList<Vector3>();

            for (var i = 0; i < capasity; i++)
                deadList[i] = (uint)i;
            deadList.Buffer.SetCounterValue((uint)capasity);
            aliveList.Buffer.SetCounterValue(0);
        }

        #region IDisposable
        public void Dispose() {
            positionList.Dispose();
            counter.Dispose();
            deadList.Dispose();
            aliveList.Dispose();
            uploadPositionList.Dispose();
        }
        #endregion

        public void Emit(Vector3 p) {
            uploadPositionList.Clear();
            uploadPositionList.Add(p);

            ComputeBuffer.CopyCount(deadList.Buffer, counter.Buffer, 0);

            compute.SetBuffer(kernelIDEmit, PROP_POSITION_LIST, positionList.Buffer);

            compute.SetBuffer(kernelIDEmit, PROP_COUNTER, counter.Buffer);
            compute.SetBuffer(kernelIDEmit, PROP_DEAD_CONSUME_LIST, deadList.Buffer);
            compute.SetBuffer(kernelIDEmit, PROP_ALIVE_APPEND_LIST, aliveList.Buffer);

            compute.SetBuffer(kernelIDEmit, PROP_UPLOAD_POSITION_LIST, uploadPositionList.Buffer);

            compute.Dispatch(kernelIDEmit, uploadPositionList.Count, 1, 1);

            ComputeBuffer.CopyCount(deadList.Buffer, counter.Buffer, 0);
            Debug.LogFormat("Dead list count={0}, ({1})",
                counter.Data[0], Join(",", deadList.Data));
            ComputeBuffer.CopyCount(aliveList.Buffer, counter.Buffer, 0);
            Debug.LogFormat("Alive list count={0}, ({1})", 
                counter.Data[0], Join(",", aliveList.Data));

        }

        public static string Join<T>(string seperater, T[] tt) {
            return string.Join(",", tt.Select(v => v.ToString()).ToArray());
        }
    }
}
