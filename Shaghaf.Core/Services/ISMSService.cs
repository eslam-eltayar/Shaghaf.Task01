using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Shaghaf.Core.Services
{
    public interface ISMSService
    {
        MessageResource SendMessage(string phoneNumber, string body);
    }
}
