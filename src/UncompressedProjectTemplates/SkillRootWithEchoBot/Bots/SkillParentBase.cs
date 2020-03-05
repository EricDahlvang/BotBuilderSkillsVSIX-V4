// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio SkillRootWithEchoBot v$templateversion$

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace $safeprojectname$.Bots
{
    public abstract class SkillParentBase : ActivityHandler
    {
        public const string ActiveSkillPropertyName = "activeSkillProperty";

        private readonly IStatePropertyAccessor<BotFrameworkSkill> _activeSkillProperty;
        private readonly string _botId;
        private readonly ConversationState _conversationState;
        private readonly SkillHttpClient _skillClient;
        private readonly SkillsConfiguration _skillsConfig;

        public SkillParentBase(ConversationState conversationState, SkillsConfiguration skillsConfig, SkillHttpClient skillClient, IConfiguration configuration)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _skillsConfig = skillsConfig ?? throw new ArgumentNullException(nameof(skillsConfig));
            _skillClient = skillClient ?? throw new ArgumentNullException(nameof(skillsConfig));
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
            if (string.IsNullOrWhiteSpace(_botId))
            {
                throw new ArgumentException($"{MicrosoftAppCredentials.MicrosoftAppIdKey} is not set in configuration");
            }

            // Create state property to track the active skill
            _activeSkillProperty = conversationState.CreateProperty<BotFrameworkSkill>(ActiveSkillPropertyName);
        }

        public abstract string BotName { get; }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            // Forward all activities except EndOfConversation to the skill.
            if (turnContext.Activity.Type != ActivityTypes.EndOfConversation)
            {
                // Try to get the active skill
                var activeSkill = await _activeSkillProperty.GetAsync(turnContext, () => null, cancellationToken);

                if (activeSkill != null)
                {
                    // Send the activity to the skill
                    await SendToSkill(turnContext, activeSkill, cancellationToken);
                    return;
                }
            }

            await base.OnTurnAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text;

            if (!string.IsNullOrEmpty(text))
            {
                BotFrameworkSkill targetSkill = null;
                if (_skillsConfig.Skills.TryGetValue(text, out targetSkill))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Got it, connecting you to the {text} skill..."), cancellationToken);

                    // Save active skill in state
                    await _activeSkillProperty.SetAsync(turnContext, targetSkill, cancellationToken);

                    // Send the activity to the skill
                    await SendToSkill(turnContext, targetSkill, cancellationToken);
                    return;
                }
            }

            // just respond with choices
            await turnContext.SendActivityAsync(MessageFactory.Attachment(GetOptionsAttachment()), cancellationToken);

            // Save conversation state
            await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);
        }

        protected override async Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            // forget skill invocation
            await _activeSkillProperty.DeleteAsync(turnContext, cancellationToken);

            // Show status message, text and value returned by the skill
            var eocActivityMessage = $"Received {ActivityTypes.EndOfConversation}.\n\nCode: {turnContext.Activity.Code}";
            if (!string.IsNullOrWhiteSpace(turnContext.Activity.Text))
            {
                eocActivityMessage += $"\n\nText: {turnContext.Activity.Text}";
            }

            if ((turnContext.Activity as Activity)?.Value != null)
            {
                eocActivityMessage += $"\n\nValue: {JsonConvert.SerializeObject((turnContext.Activity as Activity)?.Value)}";
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(eocActivityMessage), cancellationToken);

            // We are back at the root
            await turnContext.SendActivityAsync(MessageFactory.Text("Back in the root bot. Say \"skill\" and I'll patch you through"), cancellationToken);

            // Save conversation state
            await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }

        private Attachment GetOptionsAttachment()
        {
            HeroCard heroCard = null;

            if (_skillsConfig.Skills.Count > 0)
            {
                var buttons = new List<CardAction>();
                foreach (var skill in _skillsConfig.Skills)
                {
                    buttons.Add(new CardAction(ActionTypes.ImBack, title: $"{skill.Value.Id}", text: skill.Value.Id, value: skill.Value.Id));
                }

                heroCard = new HeroCard
                {
                    Title = $"MainParentWithEcho Options for {BotName}",
                    Text = "Click one of the buttons below to initiate that skill.",
                    Buttons = buttons
                };
            }
            else
            {
                heroCard = new HeroCard
                {
                    Title = $"No Skills configured for {BotName}",
                    Subtitle = "Configure some skills in appsettings.json",
                };
            }

            return heroCard.ToAttachment();
        }

        private async Task SendToSkill(ITurnContext turnContext, BotFrameworkSkill targetSkill, CancellationToken cancellationToken)
        {
            // NOTE: Always SaveChanges() before calling a skill so that any activity generated by the skill
            // will have access to current accurate state.
            await _conversationState.SaveChangesAsync(turnContext, force: true, cancellationToken: cancellationToken);

            // route the activity to the skill
            var oAuthScope = turnContext.TurnState.Get<string>(BotAdapter.OAuthScopeKey);
            var response = await _skillClient.PostActivityAsync(oAuthScope, _botId, targetSkill, _skillsConfig.SkillHostEndpoint, (Activity)turnContext.Activity, cancellationToken);

            // Check response status
            if (!(response.Status >= 200 && response.Status <= 299))
            {
                throw new HttpRequestException($"Error invoking the skill id: \"{targetSkill.Id}\" at \"{targetSkill.SkillEndpoint}\" (status is {response.Status}). \r\n {response.Body}");
            }
        }
    }
}
