# SimpleBotToBot Echo Skill

Bot Framework v4 skills echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple skill consumer (SimpleRootBot) that sends message activities to a separate skill.  This project also contains an EchoBot, demonstrating this bot consumed as a skill as well as Root Bot.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts in this sample

The solution includes a parent bot (`RootBot`) and a skill bot (`EchoBot`) and shows how the parent bot can post activities to a skill bot and returns the skill responses to the user.  These bots demonstrate how a single project can be bot Skill and Skill Root.

- This project shows how to consume an echo skill as well as expose an echo skill and includes:
  - A [RootBot](Bots/RootBot.cs) that calls the echo skill and keeps the conversation active until the user says "end" or "stop". [RootBot](Bots/RootBot.cs) also keeps track of the conversation with the skill and handles the `EndOfConversation` activity received from the skill to terminate the conversation
  - A [EchoBot](Bots/EchoBot.cs) that respond with an echo of the user's text until "end" or "stop". The EchoBot will send `EndOfConversation` activity received back to the RootBot to terminate the conversation and yield control back to the parent bot
  - A simple [SkillConversationIdFactory](SkillConversationIdFactory.cs) based on an in memory `ConcurrentDictionary` that creates and maintains conversation IDs used to interact with a skill
  - A [SkillsConfiguration](SkillsConfiguration.cs) class that can load skill definitions from `appsettings`
  - A [SkillController](Controllers/SkillController.cs) that handles skill responses
  - An [AllowedCallersClaimsValidator](Authentication/AllowedCallersClaimsValidator.cs) class that is used to authenticate that responses sent to the bot are coming from the configured skills, or configured parent bots
  - A [Startup](SimpleRootBot/Startup.cs) class that shows how to register the different skill components for dependency injection
  - A [sample skill manifest](EchoSkillBot/wwwroot/manifest/echoskillbot-manifest-1.0.json) that describes what the skill can do

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the `RootBot` and update [appsettings.json](appsettings.json) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Separately, create a different bot and add the `MicrosoftAppId` to the `AllowedCallers` list in [appsettings.json](appsettings.json) These callers will trigger the EchoBot.
- Update the `BotFrameworkSkills` section in [appsettings.json](SimpleRootBot/appsettings.json) with the app ID for the skill you created in the previous step



## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `RootBot`

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.
