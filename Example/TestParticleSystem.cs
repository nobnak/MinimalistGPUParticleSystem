using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinimalistGPUParticleSystem {

    public class TestParticleSystem : MonoBehaviour {

        [SerializeField] protected MinimalistGPUParticle gpups;

        #region Unity
        private void OnEnable() {
            StartCoroutine(Emitter());
        }

        IEnumerator Emitter() {
            yield return null;

            for (var i = 0; i < 4; i++) {
                yield return new WaitForSeconds(1f);
                gpups.Emit(10f * Random.insideUnitSphere);
            }
        }
        #endregion
    }
}
