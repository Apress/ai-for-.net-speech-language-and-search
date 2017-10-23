using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace HealthCheckupBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        string[] greetingPhrases = new string[] { "hi", "hello", "howdy", "how are you", "good morning", "good afternoon" };
        string[] appointmentPhrases = new string[] { "appointment", "schedule", "meeting" };
        string[] slotPhrases = new string[] { "1", "2", "3", "4", "5", "first", "second", "third", "one", "two", "three" };

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string userText = activity.Text.ToLower();
                string replyText = "";
                if(this.MessageHasPhrase(userText, greetingPhrases))
                {
                    replyText = @"Hi! I am Health Checkup Bot.
                    I will be glad to help you schedule a meeting with a doctor.
                    Say, 'schedule a meeting.'";
                }
                else if (this.MessageHasPhrase(userText, appointmentPhrases))
                {
                    replyText = @"I have found the following available slots.
                    Please specify the **slot number** to confirm your appointment.
                    1. Feb 26, 9:00 am
                    2. Mar 02, 12:30pm"; // basic markdown is supported
                }
                else if (this.MessageHasPhrase(userText, slotPhrases))
                {
                    replyText = "Your appointment is confirmed with Dr. John Doe. See you soon.";
                }
                else
                {
                    replyText = "Sorry, I did not understand that.";
                }

                Activity reply = activity.CreateReply(replyText);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private bool MessageHasPhrase(string message, string[] phraseList)
        {
            foreach (string phrase in phraseList)
            {
                if (message.Contains(phrase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}