using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GPUList<S> : System.IDisposable where S : struct {
    public const int MINIMUM_CAPACITY = 16;
    int count;
    S[] list;
    bool bufferIsDirty;
    ComputeBuffer buf;

    public GPUList(int initialCapasity = MINIMUM_CAPACITY) {
        count = 0;
        Resize (OptimalCapacity (initialCapasity));
    }

    #region List
    public int Count { get { return count; } }
    public S[] CPUBuffer { get { return list; } }
    public ComputeBuffer GPUBuffer { 
        get {
            if (bufferIsDirty) {
                bufferIsDirty = false;
                buf.SetData (list);
            }
            return buf; 
        }
    }
	public void Replace(S[] srcList) {
		bufferIsDirty = true;
		Resize (srcList.Length);
		System.Array.Copy (srcList, list, srcList.Length);
	}
    #endregion

    #region Elements 
    public S this[int i] {
        get { return list [i]; }
        set { 
            bufferIsDirty = true;
            list [i] = value; 
        }
    }
    public void Push(S s) {
        bufferIsDirty = true;
        Resize (OptimalCapacity (count + 1));
        list [count++] = s;
    }
	public S Pop() {
		bufferIsDirty = true;
		var s = list [--count];
		Resize (OptimalCapacity (count));
		return s;
	}
    public S RemoveAt(int indexOf) {
        bufferIsDirty = true;

        var s = list [indexOf];
        System.Array.Copy (list, indexOf + 1, list, indexOf, count - 1 - indexOf);
        Resize (OptimalCapacity (--count));
        return s;
    }
    #endregion

    int OptimalCapacity(int count) {
        return 1 << SmallestPowerOfTwoGreaterThan(Mathf.Max(MINIMUM_CAPACITY, count));
    }
    void Resize (int targetCapacity) {
        if (list == null || list.Length != targetCapacity) {
            System.Array.Resize (ref list, targetCapacity);
            Release (ref buf);
            buf = new ComputeBuffer (targetCapacity, Marshal.SizeOf (typeof(S)));
        }
    }

    static void Release<T>(ref T buf) where T : class, System.IDisposable  {
        if (buf != null) {
            buf.Dispose ();
            buf = null;
        }
    }
    static int SmallestPowerOfTwoGreaterThan (int n) {
        --n;
        n |= n >> 1;
        n |= n >> 2;
        n |= n >> 4;
        n |= n >> 8;
        n |= n >> 16;
        return ++n;
    }

    #region IDisposable implementation
    public void Dispose () {
        Release (ref buf);
    }
    #endregion
}
