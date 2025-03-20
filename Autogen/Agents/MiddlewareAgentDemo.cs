using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AutogenDotNet.Agents
{
    public class MiddlewareAgentDemo
    {
        private AzureOpenAIClient _azureOpenAIClient;
        private string _modelId;
        private readonly ChatCompletionOptions ChatCompletionOptions = new()
        {
            Temperature = 0,
            Seed = 50,
        };

        public MiddlewareAgentDemo(AzureOpenAIClient azureOpenAIClient, string modelId)
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

            assistantAgent.RegisterMiddleware(async (messages, options, agent, ct) =>
            {
                if (messages.Last() is TextMessage lastMessage)
                {
                    lastMessage.Content = $"[middleware 0] {lastMessage.Content}";
                    return lastMessage;
                }
                return await agent.GenerateReplyAsync(messages, options, ct);
            });
            
            var reply = await assistantAgent.SendAsync("Hello, tell me a joke!");
            Console.WriteLine(reply.GetContent());
        }
    }
}
