// Copyright (c) Microsoft Corporation. All rights reserved.
// LLMConfiguration.cs

using AutoGen;

namespace AutogenDotNet.Config;

public static class LLMConfiguration
{
    public static OpenAIConfig GetAzureOpenAIGPT4(string modelId, string? apiKey = null)
    {
        var azureOpenAIKey = apiKey ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new Exception("Please pass Open AI key or set AZURE_OPENAI_API_KEY environment variable.");
        return new OpenAIConfig(modelId, azureOpenAIKey);
    }
}
