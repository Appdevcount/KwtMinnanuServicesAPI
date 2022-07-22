using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETradeAPI.Models
{
  public class BrokerSubOrdinateReq
    {
        public string OrganizationIds { get; set; }
        public int UserId { get; set; }
        public string MCUserId { get; set; }
    }
}