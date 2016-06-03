using System;
using System.ComponentModel.DataAnnotations;
using ContosoPortal.Models;
using System.Collections.Generic;

namespace GistackWAPService.models
{
    /// <summary>
    /// Model class to hold VMM statistics
    /// </summary>
   public class VMMStatistics
   {
      public string CloudName { get; set; }
      public int VMCount { get; set; }

   }

    /// <summary>
    /// Model class to hold VMM statistics list
    /// </summary>
   public class VMMStatisticsList : List<VMMStatistics>
   {
       /// <summary>
       /// Initializes a new instance of the <see cref="VMMStatisticsList"/> class.
       /// </summary>
       public VMMStatisticsList()
           : base()
       {
       }

       /// <summary>
       /// Initializes a new instance of the VMM Statistics class.
       /// </summary>
       /// <param name="records">The records.</param>
       public VMMStatisticsList(IEnumerable<VMMStatistics> records)
           : base(records)
       {
       }
   }

}