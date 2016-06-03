using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold VMRoleSizeProfile detail
    /// </summary>
    public class VMRoleSizeProfile
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public long? CPU { get; set; }
    }

    /// <summary>
    /// Model class to hold VMRoleSizeProfile List
    /// </summary>
    public class VMRoleSizeProfileList : List<VMRoleSizeProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VMRoleSizeProfileList"/> class.
        /// </summary>
        public VMRoleSizeProfileList()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the VMRoleSizeProfile class.
        /// </summary>
        /// <param name="records">The records.</param>
        public VMRoleSizeProfileList(IEnumerable<VMRoleSizeProfile> records)
            : base(records)
        {
        }
    }
}