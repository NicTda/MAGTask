//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;
using System.Collections.Generic;

namespace CoreFramework
{
	/// State objects container - containing objects that will be acted upon based on states
	///
	[System.SerializableAttribute]
	public class StateObjectsContainer
    {
        public bool m_show = true;
        public string m_state = string.Empty;
		public List<GameObject> m_objects = new List<GameObject>();
	}
}
