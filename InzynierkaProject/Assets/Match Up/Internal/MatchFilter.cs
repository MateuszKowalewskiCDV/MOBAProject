using System.Collections.Generic;
using System.Linq;

namespace MatchUp
{
    /// <summary>
    /// Defines a filter to use when retrieving matches via GetMatchList()
    /// </summary>
    /// <remarks>
    /// Filter the results of GetMatchList() by passing in a Dictionary of filters like so:
    /// <code>
    /// var filters = new List<MatchFilter>() {
    ///     new MatchFilter("eloScore", 100, MatchFilter.OperationType.GREATER),
    ///     new MatchFilter("eloScore", 300, MatchFilter.OperationType.LESS)
    /// };
    /// matchUp.GetMatchList(OnMatchList, filters);
    /// </code>
    /// The above example would retrieve only matches with an eloScore between 100 and 300.
    /// </remarks>
    public class MatchFilter : MatchData
    {
        /// <summary>
        /// Supported filter operations are EQUAL, NOT_EQUAL, LIKE, NOT_LIKE, LESS, and GREATER
        /// </summary>
        /// <remarks>
        /// LIKE performs an sql search using a wildcard on both sides of the value.
        ///     ex: WHERE someKey LIKE "%someValue%"
        /// Attempting to use the LIKE operation on a non-string value will result in
        /// undefined behaviour (but most likely your filter just won't do anything).
        /// </remarks>
        public enum OperationType
        {
            EQUALS, NOT_EQUALS, LIKE, NOT_LIKE, LESS, GREATER
        }

        /// <summary>
        /// The OperationType to use when applying this filter.
        /// </summary>
        public OperationType operation;

        /// <summary>
        /// The key whose value will be filtered on. 
        /// </summary>
        public string key;

        /// <summary>
        /// Create a float or double filter.
        /// </summary>
        /// <param name="key">The key of the MatchData to filter</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="operation">The OperationType to use when applying the filter</param>
        public MatchFilter(string key, double value, OperationType operation = OperationType.EQUALS) : base(value)
        {
            this.key = key;
            this.operation = operation;
        }

        /// <summary>
        /// Create a int or long filter.
        /// </summary>
        /// <param name="key">The key of the MatchData to filter</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="operation">The OperationType to use when applying the filter</param>
        public MatchFilter(string key, long value, OperationType operation = OperationType.EQUALS) : base(value)
        {
            this.key = key;
            this.operation = operation;
        }

        /// <summary>
        /// Create a string filter.
        /// </summary>
        /// <param name="key">The key of the MatchData to filter</param>
        /// <param name="value">The value to compare against</param>
        /// <param name="operation">The OperationType to use when applying the filter</param>
        public MatchFilter(string key, string value, OperationType operation = OperationType.EQUALS) : base(value)
        {
            this.key = key;
            this.operation = operation;
        }

        /// <summary>
        /// Convert this Filter to a string for sending to the matchmaking server.
        /// </summary>
        /// <returns>A string containing all of the info that the matchmaking server needs to apply the filter.</returns>
        public string Serialize()
        {
            return base.Serialize(key) + "," + (int)operation;
        }

        /// <summary>
        /// Convert a List of MatchFilters to a string for sending to the matchmaking server.
        /// </summary>
        /// <param name="filters">The List of MatchFilters to convert</param>
        /// <returns>A string containing all of the info that the matchmaking server needs to apply the filters.</returns>
        public static string Serialize(List<MatchFilter> filters)
        {
            if (filters == null || filters.Count() == 0) return "";

            string[] dataAsStrings = new string[filters.Count];
            int i = 0;
            foreach (MatchFilter filter in filters)
            {
                dataAsStrings[i] = filter.Serialize();
                i++;
            }
            return string.Join("|", dataAsStrings);
        }
    }
}
