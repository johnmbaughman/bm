﻿namespace SuggestLib.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion interface to generate suggestions
    /// based on sub entries of specified data.
    /// </summary>
    public interface ISuggestSource
    {
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="data"/> object.
        /// 
        /// The list of suggestion is empty if helper object is null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="input"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        Task<IList<object>> SuggestAsync(object data,
                                         string input,
                                         IHierarchyHelper helper);
    }
}