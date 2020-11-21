namespace ZTR.Framework.DataAccess.DBHelpers
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// State changes helping functions
    /// </summary>
    class StateHelper
    {
        /// <summary>
        /// Set the state change values
        /// </summary>
        /// <param name="state">Current state value</param>
        /// <returns>Updated state values</returns>
        public static EntityState ConvertState(State state)
        {
#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (state)
#pragma warning restore IDE0066 // Convert switch statement to expression
            {
                case State.Added:
                    return EntityState.Added;
                case State.Modified:
                    return EntityState.Modified;
                case State.Deleted:
                    return EntityState.Deleted;
                case State.Unchanged:
                    return EntityState.Unchanged;
                default:
                    return EntityState.Detached;
            }
        }
    }
}
