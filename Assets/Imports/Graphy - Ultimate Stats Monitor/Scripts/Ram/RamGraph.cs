/* ---------------------------------------
 * Author: Martin Pane (martintayx@gmail.com) (@tayx94)
 * Project: Graphy - Ultimate Stats Monitor
 * Date: 15-Dec-17
 * Studio: Tayx
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * -------------------------------------*/

using UnityEngine;
using UnityEngine.UI;

#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

using System.Collections;
using Tayx;

namespace Tayx.Graphy.Ram
{
    public class RamGraph : Graph.Graph
    {
        #region Private Variables

        private GraphyManager m_graphyManager;

        private RamMonitor m_ramMonitor;

        [SerializeField] private Image m_imageAllocated;
        [SerializeField] private Image m_imageReserved;
        [SerializeField] private Image m_imageMono;

        private int m_resolution = 150;

        private ShaderGraph m_shaderGraphAllocated;
        private ShaderGraph m_shaderGraphReserved;
        private ShaderGraph m_shaderGraphMono;

        private float[] m_allocatedArray;
        private float[] m_reservedArray;
        private float[] m_monoArray;

        private float m_highestMemory;

        #endregion

        #region Unity Methods

        void Awake()
        {
           Init();
        }

        void Update()
        {
            UpdateGraph();
        }

        #endregion
        
        #region Public Methods

        public void UpdateParameters()
        {
            m_resolution = m_graphyManager.RamGraphResolution;
            
            CreatePoints();
        }

        #endregion

        #region Private Methods

        protected override void UpdateGraph()
        {
            float allocatedMemory = m_ramMonitor.AllocatedRam;
            float reservedMemory = m_ramMonitor.ReservedRam;
            float monoMemory = m_ramMonitor.MonoRam;

            m_highestMemory = 0;

            for (int i = 0; i <= m_resolution - 1; i++)
            {
                if (i >= m_resolution - 1)
                {
                    m_allocatedArray[i] = allocatedMemory;
                    m_reservedArray[i]  = reservedMemory;
                    m_monoArray[i]      = monoMemory;
                }
                else
                {
                    m_allocatedArray[i] = m_allocatedArray[i + 1];
                    m_reservedArray[i]  = m_reservedArray[i + 1];
                    m_monoArray[i]      = m_monoArray[i + 1];
                }

                if (m_highestMemory < m_reservedArray[i])
                {
                    m_highestMemory = m_reservedArray[i];
                }
            }

            for (int i = 0; i <= m_resolution - 1; i++)
            {
                m_shaderGraphAllocated.Array[i] = m_allocatedArray[i] / m_highestMemory;

                m_shaderGraphReserved.Array[i]  = m_reservedArray[i] / m_highestMemory;

                m_shaderGraphMono.Array[i]      = m_monoArray[i] / m_highestMemory;
            }

            m_shaderGraphAllocated.UpdatePoints();
            m_shaderGraphReserved.UpdatePoints();
            m_shaderGraphMono.UpdatePoints();
        }

        protected override void CreatePoints()
        {

            m_shaderGraphAllocated.Array    = new float[m_resolution];
            m_shaderGraphReserved.Array     = new float[m_resolution];
            m_shaderGraphMono.Array         = new float[m_resolution];

            m_allocatedArray    = new float[m_resolution];
            m_reservedArray     = new float[m_resolution];
            m_monoArray         = new float[m_resolution];

            for (int i = 0; i < m_resolution; i++)
            {
                m_shaderGraphAllocated.Array[i] = 0;
                m_shaderGraphReserved.Array[i]  = 0;
                m_shaderGraphMono.Array[i]      = 0;
            }

            // Initialize the material values
            
            // Colors
            
            m_shaderGraphAllocated.GoodColor     = m_graphyManager.AllocatedRamColor;
            m_shaderGraphAllocated.CautionColor  = m_graphyManager.AllocatedRamColor;
            m_shaderGraphAllocated.CriticalColor = m_graphyManager.AllocatedRamColor;
            
            m_shaderGraphAllocated.UpdateColors();

            m_shaderGraphReserved.GoodColor     = m_graphyManager.ReservedRamColor;
            m_shaderGraphReserved.CautionColor  = m_graphyManager.ReservedRamColor;
            m_shaderGraphReserved.CriticalColor = m_graphyManager.ReservedRamColor;
            
            m_shaderGraphReserved.UpdateColors();
            
            m_shaderGraphMono.GoodColor     = m_graphyManager.MonoRamColor;
            m_shaderGraphMono.CautionColor  = m_graphyManager.MonoRamColor;
            m_shaderGraphMono.CriticalColor = m_graphyManager.MonoRamColor;
            
            m_shaderGraphMono.UpdateColors();

            // Thresholds
            
            m_shaderGraphAllocated.GoodThreshold = -1;
            m_shaderGraphAllocated.CautionThreshold = -1;
            
            m_shaderGraphAllocated.UpdateThresholds();
            
            m_shaderGraphReserved.GoodThreshold = -1;
            m_shaderGraphReserved.CautionThreshold = -1;
            
            m_shaderGraphReserved.UpdateThresholds();
            
            m_shaderGraphMono.GoodThreshold = -1;
            m_shaderGraphMono.CautionThreshold = -1;
            
            m_shaderGraphMono.UpdateThresholds();

            m_shaderGraphAllocated.UpdateArray();
            m_shaderGraphReserved.UpdateArray();
            m_shaderGraphMono.UpdateArray();
            
            // Average
            
            m_shaderGraphAllocated.Average = -1;
            m_shaderGraphReserved.Average = -1;
            m_shaderGraphMono.Average = -1;

            m_shaderGraphAllocated.UpdateAverage();
            m_shaderGraphReserved.UpdateAverage();
            m_shaderGraphMono.UpdateAverage();
        }

        private void Init()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();

            m_ramMonitor = GetComponent<RamMonitor>();
            
            m_shaderGraphAllocated = new ShaderGraph();

            m_shaderGraphAllocated.Material = m_imageAllocated.material;

            m_shaderGraphAllocated.InitializeShader();

            m_shaderGraphReserved = new ShaderGraph();

            m_shaderGraphReserved.Material = m_imageReserved.material;

            m_shaderGraphReserved.InitializeShader();

            m_shaderGraphMono = new ShaderGraph();

            m_shaderGraphMono.Material = m_imageMono.material;

            m_shaderGraphMono.InitializeShader();

            UpdateParameters();
        }

        #endregion
    }
}