// -----------------------------------------------------------------------
// <copyright file="MultipleResultAction.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Runtime.Serialization;
    using System.ComponentModel;

    /// <summary>
    /// Determines the action to take when multiple results are found
    /// </summary>
    [DataContract(Name = "multiple-result-action", Namespace = "http://lithnet.local/Lithnet.Idm.Core/v1/")]
    public enum MultipleResultAction
    {
        /// <summary>
        /// Use all the results
        /// </summary>
        [Description("Use all results")]
        [EnumMember(Value = "use-all")]
        UseAll,

        /// <summary>
        /// Use the first result that was found
        /// </summary>
        [Description("Use the first matching result")]
        [EnumMember(Value = "use-first")]
        UseFirst,

        /// <summary>
        /// Use none of the results
        /// </summary>
        [Description("Return null")]
        [EnumMember(Value = "use-none")]
        UseNone,

        /// <summary>
        /// Throw an exception
        /// </summary>
        [Description("Throw an error")]
        [EnumMember(Value = "throw-error")]
        Error
    }
}
