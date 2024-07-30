using Microsoft.Extensions.Options;
using Shaghaf.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Shaghaf.Service.SMSService
{
    public class SMSService : ISMSService
    {
        private readonly TwilioSettings _twilio;

        public SMSService(IOptions<TwilioSettings> twilio)
        {
            _twilio = twilio.Value;
        }

        public MessageResource SendMessage(string phoneNumber, string body)
        {
            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);

            var result = MessageResource.Create(
                body: body,
                from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
                to: phoneNumber
                );

            return result;
        }
    }
}
