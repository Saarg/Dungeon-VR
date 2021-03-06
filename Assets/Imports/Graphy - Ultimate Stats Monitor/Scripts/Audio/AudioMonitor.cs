﻿/* ---------------------------------------
 * Author: Martin Pane (martintayx@gmail.com) (@tayx94)
 * Project: Graphy - Ultimate Stats Monitor
 * Date: 15-Dec-17
 * Studio: Tayx
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * -------------------------------------*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


namespace Tayx.Graphy.Audio
{
    public class AudioMonitor : MonoBehaviour
    {
        #region Private Variables

        private GraphyManager m_graphyManager;

        private AudioListener m_audioListener;

        private GraphyManager.LookForAudioListener m_findAudioListenerInCameraIfNull = GraphyManager.LookForAudioListener.ON_SCENE_LOAD;

        private FFTWindow m_FFTWindow = FFTWindow.Blackman;

        private int m_spectrumSize = 512;

        private float[] m_spectrum;

        private float m_maxDB;

        #endregion

        #region Properties

        /// <summary>
        /// Current audio spectrum from the specified AudioListener.
        /// </summary>
        public float[] Spectrum { get { return m_spectrum; } }

        /// <summary>
        /// Maximum DB registered in the current spectrum.
        /// </summary>
        public float MaxDB { get { return m_maxDB; } }

        /// <summary>
        /// Returns true if there is a reference to the audio listener.
        /// </summary>
        public bool SpectrumDataAvailable {  get { return m_audioListener != null;} }

        #endregion

        #region Unity Methods

        void Awake()
        {
            Init();
        }

        void Update()
        {
            if (m_audioListener != null)
            {
                AudioListener.GetSpectrumData(m_spectrum, 0, m_FFTWindow);

                m_maxDB = 0;

                // Save the highest value from the spectrum

                for (int i = 0; i < m_spectrum.Length; i++)
                {
                    if (m_maxDB < m_spectrum[i])
                    {
                        m_maxDB = m_spectrum[i];
                    }
                }

                m_maxDB = lin2dB(m_maxDB);
            }
            else if(     m_audioListener == null 
                     &&  m_findAudioListenerInCameraIfNull == GraphyManager.LookForAudioListener.ALWAYS)
            {
                FindAudioListener();
            }
        }   
        
        #endregion

        #region Public Methods
        
        public void UpdateParameters()
        {
            m_findAudioListenerInCameraIfNull = m_graphyManager.FindAudioListenerInCameraIfNull;

            m_audioListener = m_graphyManager.AudioListener;
            m_FFTWindow     = m_graphyManager.FftWindow;
            m_spectrumSize  = m_graphyManager.SpectrumSize;

            if (    m_audioListener == null 
                    &&  m_findAudioListenerInCameraIfNull != GraphyManager.LookForAudioListener.NEVER)
            {
                FindAudioListener();
            }

            m_spectrum = new float[m_spectrumSize];
        }

        /// <summary>
        /// Converts spectrum values to decibels using logarithms.
        /// </summary>
        /// <param name="linear"></param>
        /// <returns></returns>
        public float lin2dB(float linear)
        {
            return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -160.0f, 0.0f);
        }

        /// <summary>
        /// Normalizes a value in decibels between 0-1.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public float dBNormalized(float db)
        {
            return (db + 160) / 160f;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tries to find an audio listener in the main camera.
        /// </summary>
        private void FindAudioListener()
        {
            m_audioListener = Camera.main.GetComponent<AudioListener>();
        }

        private void Init()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();
            
            UpdateParameters();

            SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                if (m_findAudioListenerInCameraIfNull == GraphyManager.LookForAudioListener.ON_SCENE_LOAD)
                {
                    FindAudioListener();
                }
            };
        }

        #endregion


    }
}