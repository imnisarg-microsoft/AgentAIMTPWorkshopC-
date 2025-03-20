using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AutogenDotNet.Agents
{
    public class OpenAIAgentWithUserProxy
    {
        private AzureOpenAIClient _azureOpenAIClient;
        private string _modelId;
        private readonly ChatCompletionOptions ChatCompletionOptions = new()
        {
            Temperature = 0,
            Seed = 50,
        };

        public OpenAIAgentWithUserProxy(AzureOpenAIClient azureOpenAIClient, string modelId)
        {
            _azureOpenAIClient = azureOpenAIClient;
            _modelId = modelId;
        }

        public async Task RunAsync()
        {
            var chatClient = _azureOpenAIClient.GetChatClient(_modelId);
            var assistantAgent = new OpenAIChatAgent(
                chatClient: chatClient,
                name: "assistant",
                options: ChatCompletionOptions,
                systemMessage: "You are an assistant that help user to do some tasks.")
                .RegisterMessageConnector();

            var userProxyAgent = new OpenAIChatAgent(
                name: $"UserProxy",
                systemMessage: "You are a proxy to an user.",
                chatClient: chatClient,
                options: ChatCompletionOptions)
                .RegisterMessageConnector();
            
            var messages = await userProxyAgent.InitiateChatAsync(receiver: assistantAgent, message: "Ask the assistant agent to tell about MITRE techniques", maxRound: 1);
            foreach (var item in messages)
            {
                Console.WriteLine(item?.GetContent());
            }
        }
    }
}
