//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using CoreFramework;
using System.Collections.Generic;

namespace MAGTask
{
    /// Service that handles the level objectives
    /// 
	public sealed class ObjectiveService : Service
    {
        private List<ObjectiveModel> m_objectives = new List<ObjectiveModel>(2);

        #region Service functions
        /// Used to complete the initialisation in case the service depends
        /// on other Services
        /// 
        public override void OnCompleteInitialisation()
        {
        }
        #endregion

        #region Public functions
        /// @param objectiveData
        ///     The data of the objective to add
        ///     
        /// @return The model fo the objective
        /// 
        public ObjectiveModel AddObjective(ObjectiveData objectiveData)
        {
            var objectiveModel = new ObjectiveModel(objectiveData);
            m_objectives.Add(objectiveModel);
            return objectiveModel;
        }

        /// @return Whether all the objectives are completed
        /// 
        public bool AreObjectivesComplete()
        {
            bool completed = true;
            foreach(var objective in m_objectives)
            {
                if (objective.IsComplete() == false)
                {
                    completed = false;
                    break;
                }
            }
            return completed;
        }

        /// @param type
        ///     The type of objective targetted
        /// @param amount
        ///     The progressed amount for that objective
        /// 
        public void LogEvent(ObjectiveType type, int amount = 1)
        {
            foreach(var objective in m_objectives)
            {
                if(objective.m_data.m_type == type)
                {
                    objective.AddProgress(amount);
                }
            }
        }

        /// @param type
        ///     The type of objective targetted
        /// @param target
        ///     The target of the event
        /// @param amount
        ///     The progressed amount for that objective
        /// 
        public void LogEvent(ObjectiveType type, TileColour target, int amount = 1)
        {
            foreach (var objective in m_objectives)
            {
                if (objective.m_data.m_type == type && objective.m_data.m_target == target)
                {
                    objective.AddProgress(amount);
                }
            }
        }

        /// @param type
        ///     The type of objective targetted
        /// @param value
        ///     The value of the event
        /// @param amount
        ///     The progressed amount for that objective
        /// 
        public void LogEventWithValue(ObjectiveType type, int value = 1, int amount = 1)
        {
            foreach (var objective in m_objectives)
            {
                if (objective.m_data.m_type == type && objective.m_data.m_value <= value)
                {
                    objective.AddProgress(amount);
                }
            }
        }
        #endregion
    }
}
