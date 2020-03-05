// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio SkillRootWithEchoBot v$templateversion$

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace $safeprojectname$.Bots
{
    public class RootBot : SkillParentBase
{
    public RootBot(ConversationState conversationState, SkillsConfiguration skillsConfig, SkillHttpClient skillClient, IConfiguration configuration)
        : base(conversationState, skillsConfig, skillClient, configuration)
    {
    }

    public override string BotName => "Root Bot";

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Hello and welcome! I'm the Root with Echo bot's Root."), cancellationToken);
            }
        }
    }
}
}
