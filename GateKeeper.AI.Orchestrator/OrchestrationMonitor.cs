using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GateKeeper.AI.Orchestrator;

internal sealed class OrchestrationMonitor
{
    public List<StreamingChatMessageContent> StreamedResponses = [];

    public ChatHistory History { get; } = [];

    public ValueTask ResponseCallback(ChatMessageContent response)
    {
        this.History.Add(response);
        WriteResponse(response);
        return ValueTask.CompletedTask;
    }

    public ValueTask StreamingResultCallback(StreamingChatMessageContent streamedResponse, bool isFinal)
    {
        this.StreamedResponses.Add(streamedResponse);

        if (isFinal)
        {
            WriteStreamedResponse(this.StreamedResponses);
            this.StreamedResponses.Clear();
        }

        return ValueTask.CompletedTask;
    }
    protected static void WriteResponse(ChatMessageContent response)
    {
        if (!string.IsNullOrEmpty(response.Content))
        {
            System.Console.WriteLine($"\n# RESPONSE {response.Role}{(response.AuthorName is not null ? $" - {response.AuthorName}" : string.Empty)}: {response}");
            
        }
    }

    protected static void WriteStreamedResponse(IEnumerable<StreamingChatMessageContent> streamedResponses)
    {
        string? authorName = null;
        AuthorRole? authorRole = null;
        StringBuilder builder = new();
        foreach (StreamingChatMessageContent response in streamedResponses)
        {
            authorName ??= response.AuthorName;
            authorRole ??= response.Role;

            if (!string.IsNullOrEmpty(response.Content))
            {
                builder.Append($"({JsonSerializer.Serialize(response.Content)})");
            }
        }

        if (builder.Length > 0)
        {
            System.Console.WriteLine($"\n# STREAMED {authorRole ?? AuthorRole.Assistant}{(authorName is not null ? $" - {authorName}" : string.Empty)}: {builder}\n");
        }
    }
}