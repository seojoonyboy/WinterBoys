﻿using UnityEngine;
using System.Collections;

namespace I2.Loc
{
	// This class is used to spawn coroutines from outside of MonoBehaviors
	public class CoroutineManager : MonoBehaviour 
	{
		static CoroutineManager pInstance
		{
			get{
				if (mInstance==null)
				{
					GameObject GO = new GameObject( "_Coroutiner" );
                    GO.hideFlags = HideFlags.HideAndDontSave;
                    mInstance = GO.AddComponent<CoroutineManager>();
                    DontDestroyOnLoad(GO);
                }
                return mInstance;
			}
		}
        static CoroutineManager mInstance;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static Coroutine Start(IEnumerator coroutine)
		{
			#if UNITY_EDITOR
				// Special case to allow coroutines to run in the Editor
				if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
				{
					UnityEditor.EditorApplication.CallbackFunction delg=null;
					delg = delegate () 
					{
						if (!coroutine.MoveNext())
							UnityEditor.EditorApplication.update -= delg;
					};
					UnityEditor.EditorApplication.update += delg;
					return null;
				}
			#endif

			return pInstance.StartCoroutine(coroutine);
		}
	}
}