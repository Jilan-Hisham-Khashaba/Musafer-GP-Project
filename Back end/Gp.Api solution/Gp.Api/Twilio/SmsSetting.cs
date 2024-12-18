using GP.Core;
using GP.Repository;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Types;

using Twilio.Rest.Api.V2010.Account;

namespace Gp.Api.Twilio
{
    public class SmsSetting : ISmsMessage
    {
        private TwilioSetting _options;

        public SmsSetting(IOptions<TwilioSetting> options)
        {
            _options = options.Value;
        }
        public MessageResource Send(SMS sms)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);


            var result = MessageResource.Create
                (
                body: sms.Body,
                from: new PhoneNumber(_options.TwilioPhoneNumber),
                to: sms.PhoneNumber

                );
            return result;

        }
    }
}
