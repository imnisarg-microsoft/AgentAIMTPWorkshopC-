// See https://aka.ms/new-console-template for more information

using AutoGen.Core;
using AutoGen.OpenAI;
using Azure.AI.OpenAI;
using AutoGen.OpenAI.Extension;

namespace AutogenDotNet.Agents
{
    public class OpenAIChatAgentDemo
    {
        private AzureOpenAIClient _azureOpenAIClient;
        private string _modelId;

        public OpenAIChatAgentDemo(AzureOpenAIClient azureOpenAIClient, string modelId) 
        {
            _azureOpenAIClient = azureOpenAIClient;
            _modelId = modelId;
        }

        public async Task RunAsync()
        {
            // create an open ai chat agent
            var openAIChatAgent = new OpenAIChatAgent(
                chatClient: _azureOpenAIClient.GetChatClient(_modelId),
                name: "assistant",
                systemMessage: "You are an assistant that help user to do some tasks.")
                .RegisterMessageConnector();

            var reply = await openAIChatAgent.SendAsync("Hello, tell me a joke!");
            Console.WriteLine(reply.GetContent());
        }
    }
}
