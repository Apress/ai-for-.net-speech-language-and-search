using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using DrCheckup.Models;

namespace DrCheckup.Dialogs
{
    [LuisModel("", "")]
    [Serializable]
    public class HealthCheckupDialog : LuisDialog<object>
    {
        /// <summary>
        /// Patient details
        /// (instance variables form the state of a dialog;
        ///  they remain persistent during the lifetime of a session)
        /// </summary>
        string patientFirstName, patientLastName, patientAge;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [LuisIntent("Greet")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            string message = "Hello, there. Tell me your symptoms and I will tell you possible conditions. " +
                "Or ask me to schedule an appointment with a doctor. " +
                "But first I need to know your name and age.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("PatientDetails")]
        public async Task PatientDetails(IDialogContext context, LuisResult result)
        {
            EntityRecommendation patientFirstNameEntity, patientLastNameEntity, patientAgeEntity;
            
            if (!result.TryFindEntity("PatientName::FirstName", out patientFirstNameEntity))
            {
                patientFirstNameEntity = new EntityRecommendation() { Entity = "Unknown" };
            }
            if (!result.TryFindEntity("PatientName::LastName", out patientLastNameEntity))
            {
                patientLastNameEntity = new EntityRecommendation() { Entity = "Unknown" };
            }
            if (!result.TryFindEntity("builtin.age", out patientAgeEntity))
            {
                patientAgeEntity = new EntityRecommendation() { Resolution = new Dictionary<string, object>() { { "value", "29" } } };
            }

            patientFirstName = patientFirstNameEntity.Entity;
            patientLastName = patientLastNameEntity.Entity;
            patientAge = patientAgeEntity.Resolution["value"].ToString();

            string message = $"Hi, {patientFirstName}. How can I help you today?";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetCondition")]
        public async Task GetCondition(IDialogContext context, LuisResult result)
        {
            IList<SymptomGroup> symptomGroups = new List<SymptomGroup>();
            foreach (CompositeEntity sg in result.CompositeEntities)
            {
                if (sg.ParentType == "SymptomGroup")
                {
                    symptomGroups.Add(new SymptomGroup() {
                        BodyPart = sg.Children.Where(x => x.Type == "BodyPart").FirstOrDefault().Value,
                        Symptoms = sg.Children.Where(x => x.Type == "Symptom").Select(x => x.Value).ToArray()
                    });
                }
            }

            string[] conditions = { };
            
            // TODO: Call ApiMedic API using extracted symptom group(s)

            string message;
            if (conditions.Length < 1)
            {
                message = "I'm sorry, I could not determine your condition.";
            }
            else
            {
                message = $"You might be suffering from {string.Join(" or ", conditions)}.";
            }

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}