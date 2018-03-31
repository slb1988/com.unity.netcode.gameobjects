﻿using MLAPI.MonoBehaviours.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MLAPI.NetworkingManagerComponents
{
    /// <summary>
    /// The main class for controlling lag compensation
    /// </summary>
    public static class LagCompensationManager
    {
        public static List<TrackedObject> SimulationObjects = new List<TrackedObject>();

        /// <summary>
        /// Turns time back a given amount of seconds, invokes an action and turns it back
        /// </summary>
        /// <param name="secondsAgo">The amount of seconds</param>
        /// <param name="action">The action to invoke when time is turned back</param>
        public static void Simulate(float secondsAgo, Action action)
        {
            if(!NetworkingManager.singleton.isServer)
            {
                Debug.LogWarning("MLAPI: Lag compensation simulations are only to be ran on the server.");
                return;
            }
            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                SimulationObjects[i].ReverseTransform(secondsAgo);
            }

            action.Invoke();

            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                SimulationObjects[i].ResetStateTransform();
            }
        }

        private static byte error = 0;
        /// <summary>
        /// Turns time back a given amount of seconds, invokes an action and turns it back. The time is based on the estimated RTT of a clientId
        /// </summary>
        /// <param name="clientId">The clientId's RTT to use</param>
        /// <param name="action">The action to invoke when time is turned back</param>
        public static void Simulate(int clientId, Action action)
        {
            if (!NetworkingManager.singleton.isServer)
            {
                Debug.LogWarning("MLAPI: Lag compensation simulations are only to be ran on the server.");
                return;
            }
            float milisecondsDelay = NetworkTransport.GetCurrentRTT(NetworkingManager.singleton.hostId, clientId, out error) / 2f;
            Simulate(milisecondsDelay * 1000f, action);
        }

        internal static void AddFrames()
        {
            for (int i = 0; i < SimulationObjects.Count; i++)
            {
                SimulationObjects[i].AddFrame();
            }
        }
    }
}
