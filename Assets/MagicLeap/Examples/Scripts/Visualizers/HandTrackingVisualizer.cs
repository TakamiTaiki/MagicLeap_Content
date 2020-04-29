// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Component used to hook into the Hand Tracking script and attach
    /// primitive game objects to it's detected keypoint positions for
    /// each hand.
    /// </summary>
    public class HandTrackingVisualizer : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("The hand to visualize.")]
        private MLHandType _handType = MLHandType.Left;

        [SerializeField] Transform _center = null;
        [SerializeField] Transform _thumb = null;
        [SerializeField] Transform _index = null;
        #endregion

        #region Private Properties
        /// <summary>
        /// Returns the hand based on the hand type.
        /// </summary>
        private MLHand Hand
        {
            get
            {
                if (_handType == MLHandType.Left)
                {
                    return MLHands.Left;
                }
                else
                {
                    return MLHands.Right;
                }
            }
        }
        #endregion

        #region Unity Methods
        /// <summary>
        /// Initializes MLHands API.
        /// </summary>
        void Start()
        {
            MLResult result = MLHands.Start();
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: HandTrackingVisualizer failed starting MLHands, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Stops the communication to the MLHands API.
        /// </summary>
        void OnDestroy()
        {
            if (MLHands.IsStarted)
            {
                MLHands.Stop();
            }
        }

        /// <summary>
        /// Update the keypoint positions.
        /// </summary>
        void Update()
        {
            if (MLHands.IsStarted)
            {
                _center.position = Hand.Center;
                _center.gameObject.SetActive(Hand.IsVisible);

                _index.position = Hand.Index.KeyPoints[2].Position;
                _index.gameObject.SetActive(Hand.IsVisible);

                _thumb.position = Hand.Thumb.KeyPoints[2].Position;
                _thumb.gameObject.SetActive(Hand.IsVisible);
            }
        }
        #endregion
    }
}
