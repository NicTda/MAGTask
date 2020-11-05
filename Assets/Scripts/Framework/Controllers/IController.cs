//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

namespace CoreFramework
{
    /// Interface that defines the base behaviour of a controller
    /// 
	public interface Icontroller
	{
        /// Updates the state of the FSM
        /// 
		void Update();

        /// Disposes the state of the controller
        /// 
		void Dispose();
	}
}
