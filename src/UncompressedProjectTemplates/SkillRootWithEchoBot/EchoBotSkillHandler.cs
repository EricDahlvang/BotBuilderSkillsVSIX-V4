// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio SkillRootWithEchoBot v$templateversion$

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$
{
    public class EchoBotSkillHandler : SkillHandler
    {
        public EchoBotSkillHandler(BotAdapter adapter, Bots.EchoBot bot, SkillConversationIdFactoryBase conversationIdFactory, ICredentialProvider credentialProvider, AuthenticationConfiguration authConfig, IChannelProvider channelProvider = null, ILogger logger = null)
        : base(adapter, bot, conversationIdFactory, credentialProvider, authConfig, channelProvider, logger)
        {
        }
    }
}
