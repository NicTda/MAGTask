//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    /// FSM controller using an animator
    ///
    public abstract class FSMController : Controller
    {
        protected FSMAnimator m_fsm = null;
        protected FSMView m_fsmView = null;

        #region Public functions
        /// @param localDirector
        ///     The local director owner of the Controller
        /// @param sceneView
        ///     The view component of the fsm
        /// 
        public FSMController(LocalDirector localDirector, FSMView fsmView)
            : base(localDirector)
        {
            m_fsmView = fsmView;
            m_fsmView.Initialise();

            m_fsm = new FSMAnimator();
            m_fsm.Initialise(m_fsmView.FSMAnimator);
        }
        #endregion

        #region Controller functions
        /// Update
        /// 
        public override void Update()
        {
            m_fsm.Update();
        }
        #endregion
    }
}
