// Copyright (c) Microsoft Corporation. All rights reserved.
// AssistantAgentDemo.cs

using AutoGen.Core;
using AutoGen;
using AutogenDotNet.Config;

namespace AutogenDotNet.Agents;

/// <summary>
/// This example shows the basic usage of <see cref="AssistantAgent"/> class.
/// </summary>
public static class AssistantAgentDemo
{
    
    public static async Task RunAsync()
    {
        var gtp4oConfig = LLMConfiguration.GetAzureOpenAIGPT4("https://dex-jpn-openai-stg.openai.azure.com/", "gpt-4o");
        var config = new ConversableAgentConfig
        {
            Temperature = 0,
            ConfigList = [gtp4oConfig],
        };

        // create assistant agent
        var assistantAgent = new AssistantAgent(
            name: "assistant",
            systemMessage: "You convert what user said to all uppercase.",
            llmConfig: config)
            .RegisterPrintMessage();

        // talk to the assistant agent
        var reply = await assistantAgent.SendAsync("hello world");
        reply.GetContent()?.Equals("HELLO WORLD");

        // to carry on the conversation, pass the previous conversation history to the next call
        var conversationHistory = new List<IMessage>
        {
            new TextMessage(Role.User, "hello world"), // first message
            reply, // reply from assistant agent
        };

        reply = await assistantAgent.SendAsync("hello world again", conversationHistory);
        reply.GetContent()?.Equals("HELLO WORLD AGAIN");
    }
}
