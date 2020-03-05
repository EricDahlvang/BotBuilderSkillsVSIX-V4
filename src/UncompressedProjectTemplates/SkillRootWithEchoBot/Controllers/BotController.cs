// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio SkillRootBot v$templateversion$

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using $safeprojectname$.Bots;

namespace $safeprojectname$.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotFrameworkHttpAdapter _adapter;
        private readonly IBot _echoBot;
        private readonly IBot _rootBot;

        public BotController(BotFrameworkHttpAdapter adapter, Bots.RootBot rootBot, Bots.EchoBot echoBot)
        {
            _adapter = adapter;
            _rootBot = rootBot;
            _echoBot = echoBot;
        }

        [HttpPost, HttpGet]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            var bot = SkillValidation.IsSkillToken(Request.Headers["Authorization"]) ? _echoBot : _rootBot;
            await _adapter.ProcessAsync(Request, Response, bot);
        }
    }
}
