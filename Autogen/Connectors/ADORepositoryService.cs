// <copyright file="ADORepositoryService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutogenDotNet.Connectors
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.TeamFoundation.SourceControl.WebApi;

    /// <summary>
    /// Refer this - https://learn.microsoft.com/en-us/rest/api/azure/devops/?view=azure-devops-rest-7.2 for REST API
    /// And Refer this for .net API - https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/client-libraries/samples?view=azure-devops
    /// More Samples here - https://github.com/microsoft/azure-devops-dotnet-samples/blob/main/ClientLibrary/Samples/Git/ItemsSample.cs
    /// </summary>
    public class ADORepositoryService
    {
        private readonly ProjectHttpClient _projectHttpClient;
        private readonly GitHttpClient _gitHttpClient;
        private readonly HttpClient _httpClient;

        public ADORepositoryService(ProjectHttpClient projectHttpClient, GitHttpClient gitHttpClient, HttpClient httpClient)
        {

            _projectHttpClient = projectHttpClient ?? throw new Exception("Project HTTP client is null");
            _gitHttpClient = gitHttpClient ?? throw new Exception("Project HTTP client is null"); ;
            _httpClient = httpClient ?? throw new Exception("Project HTTP client is null");
        }

        /// <summary>
        /// This method will fetch the project details and iterate over all repositories.
        /// </summary>
        public async Task<List<GitRepository>> IterateOverRepositoriesAsync(string projectName)
        {
            var project = await _projectHttpClient.GetProject(projectName).ConfigureAwait(false);
            Console.WriteLine("Fetching repositories for project name : {0} and id {1}", projectName, project.Id);
            var repos = await _gitHttpClient.GetRepositoriesAsync(projectName).ConfigureAwait(false);
            return repos;
        }


        /// <summary>
        /// Download a file as string from repository.
        /// </summary>
        public async Task<string> GetFileAsString(string fileName, string projectName, string repoName)
        {
            var project = await _projectHttpClient.GetProject(projectName).ConfigureAwait(false);
            var repo = await _gitHttpClient.GetRepositoryAsync(project.Id, repoName).ConfigureAwait(false);

            var items = await _gitHttpClient.GetItemsAsync(repo.Id, scopePath: "/", recursionLevel: VersionControlRecursionType.Full);
            var filePath = items?.Where(o => o.GitObjectType == GitObjectType.Blob && o.Path.Contains(fileName))?.FirstOrDefault()?.Path;

            // retrieve the contents of the file
            GitItem item = await _gitHttpClient.GetItemAsync(repo.Id, filePath, includeContent: true);

            Console.WriteLine("File {0} at commit {1} is of length {2}", filePath, item.CommitId, item.Content.Length);
            return item.Content;
        }
    }
}
