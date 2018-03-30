/* ---------------------------------------
 * Author: Martin Pane (martintayx@gmail.com) (@tayx94)
 * Project: Graphy - Ultimate Stats Monitor
 * Date: 05-Dec-17
 * Studio: Tayx
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * -------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Text;
using UnityEngine.Events;

namespace Tayx.Graphy.Ram
{
    public class RamText : MonoBehaviour
    { 
        #region Private Variables

        private GraphyManager m_graphyManager;

        private RamMonitor m_ramMonitor;

        [SerializeField] private Text m_allocatedSystemMemorySizeText;
        [SerializeField] private Text m_reservedSystemMemorySizeText;
        [SerializeField] private Text m_monoSystemMemorySizeText;

        private float m_updateRate = 4f;  // 4 updates per sec.

        private float m_deltaTime = 0.0f;

        private StringBuilder m_sb;

        #endregion

        #region Unity Methods

        void Awake()
        {
            Init();
        }

        void Update()
        {
            m_deltaTime += Time.unscaledDeltaTime;

            if (m_deltaTime > 1.0 / m_updateRate)
            {
                // Update allocated and reserved memory

                m_sb.Length = 0;
                m_sb.AppendFormat("{0:0.0}", m_ramMonitor.AllocatedRam).Append(" MB");
                m_allocatedSystemMemorySizeText.text = m_sb.ToString();

                m_sb.Length = 0;
                m_sb.AppendFormat("{0:0.0}", m_ramMonitor.ReservedRam).Append(" MB");
                m_reservedSystemMemorySizeText.text = m_sb.ToString();

                m_sb.Length = 0;
                m_sb.AppendFormat("{0:0.0}", m_ramMonitor.MonoRam).Append(" MB");
                m_monoSystemMemorySizeText.text = m_sb.ToString();

                m_deltaTime = 0;
            }
        }

        #endregion
        
        #region Public Methods
        
        public void UpdateParameters()
        {
            m_allocatedSystemMemorySizeText.color = m_graphyManager.AllocatedRamColor;
            m_reservedSystemMemorySizeText.color = m_graphyManager.ReservedRamColor;
            m_monoSystemMemorySizeText.color = m_graphyManager.MonoRamColor;

            m_updateRate = m_graphyManager.RamTextUpdateRate;
        }
        
        #endregion

        #region Private Methods

        private void Init()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();

            m_ramMonitor = GetComponent<RamMonitor>();
            
            m_sb = new StringBuilder();

            UpdateParameters();
        }

        #endregion

    }
}