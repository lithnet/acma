// -----------------------------------------------------------------------
// <copyright file="TimeSpanType.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies what type of time span component is represented by an associated integer value
    /// </summary>
    [DataContract(Name = "time-span-type", Namespace = "http://lithnet.local/Lithnet.Idm.Transforms/v1/")]
    public enum TimeSpanType
    {
        /// <summary>
        /// The number of months
        /// </summary>
        [EnumMember(Value = "months")]
        Months,
        
        /// <summary>
        /// The number of weeks
        /// </summary>
        [EnumMember(Value = "weeks")]
        Weeks,

        /// <summary>
        /// The number of days
        /// </summary>
        [EnumMember(Value = "days")]
        Days,

        /// <summary>
        /// The number of hours
        /// </summary>
        [EnumMember(Value = "hours")]
        Hours,

        /// <summary>
        /// The number of minutes
        /// </summary>
        [EnumMember(Value = "minutes")]
        Minutes
    }
}
