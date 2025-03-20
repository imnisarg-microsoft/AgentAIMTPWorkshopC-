using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using AutogenDotNet.Functions;
using Azure.AI.OpenAI;
using OpenAI.Chat;


namespace AutogenDotNet.Agents
{
    public class AgentWithFunctionCall
    {
        private AzureOpenAIClient _azureOpenAIClient;
        private string _modelId;
        private readonly ChatCompletionOptions ChatCompletionOptions = new()
        {
            Temperature = 0,
            Seed = 50,
        };

        public AgentWithFunctionCall(AzureOpenAIClient azureOpenAIClient, string modelId)
        {
            _azureOpenAIClient = azureOpenAIClient;
            _modelId = modelId;
        }

        public async Task RunAsync()
        {
            var function = new TypeSafeFunctionCall();
            var functionCallMiddleware = new FunctionCallMiddleware(
               functions: [function.WeatherReportFunctionContract],
               functionMap: new Dictionary<string, Func<string, Task<string>>>
               {
                    { function.WeatherReportFunctionContract.Name, function.WeatherReportWrapper },
               });

            var chatClient = _azureOpenAIClient.GetChatClient(_modelId);
            var assistantAgent = new OpenAIChatAgent(
                chatClient: chatClient,
                name: "assistant",
                options: ChatCompletionOptions,
                systemMessage: "You are an assistant that help user to do some tasks.")
                .RegisterMessageConnector()
                .RegisterMiddleware(functionCallMiddleware);

            var reply = await assistantAgent.SendAsync("What's the weather in Hyderabad today? today is 2025-03-18");
            Console.WriteLine(reply.GetContent());

            var userProxyAgent = new OpenAIChatAgent(
                name: $"UserProxy",
                systemMessage: "You are a proxy to an user.",
                chatClient: chatClient,
                options: ChatCompletionOptions)
                .RegisterMessageConnector();

            var messages = await userProxyAgent.InitiateChatAsync(receiver: assistantAgent, message: "What's the weather in Hyderabad today? today is 2025-03-18", maxRound: 1);
            foreach (var item in messages)
            {
                Console.WriteLine(item?.GetContent());
            }
        }
    }
}
