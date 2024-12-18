using GP.Core;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;

namespace Gp.Api.Twilio
{
    public interface ISmsMessage
    {
        MessageResource Send(SMS sms);
    }
}
