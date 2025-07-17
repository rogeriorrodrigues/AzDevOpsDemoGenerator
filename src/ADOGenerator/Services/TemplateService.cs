﻿using ADOGenerator.IServices;
using ADOGenerator.Models;
using ADOGenerator.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestAPI.Extractor;
using RestAPI.ProjectsAndTeams;
using RestAPI;
using ADOGenerator;
using System.IO;
using System.Linq;

public class TemplateService : ITemplateService
{
    private readonly IConfiguration _config;
    private readonly IExtractorService extractorService;

    public TemplateService(IConfiguration config)
    {
        _config = config;
        extractorService = new ExtractorService(_config);
    }

    public bool AnalyzeProject(Project model)
    {
        try
        {
            string defaultHost = _config["AppSettings:DefaultHost"];
            string projectPropertyVersion = _config["AppSettings:ProjectPropertyVersion"];

            ADOConfiguration config = new ADOConfiguration
            {
                AccountName = model.accountName,
                PersonalAccessToken = model.accessToken,
                UriString = defaultHost + model.accountName,
                VersionNumber = projectPropertyVersion,
                ProjectId = model.ProjectId,
                _adoAuthScheme = model.adoAuthScheme
            };

            Projects projects = new Projects(config);
            ProjectProperties.Properties load = projects.GetProjectProperties();
            model.ProcessTemplate = load.value[4].value;
            ExtractorService es = new ExtractorService(_config);
            ExtractorAnalysis analysis = new ExtractorAnalysis();
            ProjectConfigurations appConfig = extractorService.ProjectConfiguration(model);
            analysis.teamCount = extractorService.GetTeamsCount(appConfig);
            analysis.IterationCount = extractorService.GetIterationsCount(appConfig);
            analysis.WorkItemCounts = extractorService.GetWorkItemsCount(appConfig);
            analysis.BuildDefCount = extractorService.GetBuildDefinitionCount(appConfig);
            analysis.ReleaseDefCount = extractorService.GetReleaseDefinitionCount(appConfig);
            analysis.ErrorMessages = es.errorMessages;

            LogAnalysisResults(model, analysis);

            return true;
        }
        catch (Exception ex)
        {
            model.id.ErrorId().AddMessage("Error during project analysis: " + ex.Message);
            return false;
        }
    }
    public bool CheckTemplateExists(Project model)
    {
        try
        {
            if (extractorService.IsTemplateExists(model.ProjectName))
            {
                model.id.AddMessage("Template already exists.");
                return true; // Template exists
            }
            else
            {
                model.id.AddMessage("Template does not exist.");
                return false; // Template does not exist
            }
        }
        catch (Exception ex)
        {
            model.id.ErrorId().AddMessage("Error checking template existence: " + ex.Message);
            return false; // Error occurred while checking template existence
        }
    }

    public (bool,string,string) GenerateTemplateArtifacts(Project model)
    {
        try
        {
            string[] createdTemplate = extractorService.GenerateTemplateArifacts(model);
            if (createdTemplate == null || createdTemplate.Length == 0)
            {
                model.id.AddMessage("No artifacts were generated.");
                return (false, string.Empty,string.Empty); // No artifacts generated
            }
            string template = createdTemplate[1];
            string templateLocation = createdTemplate[2];
            return (true,template, templateLocation); // Artifact generation completed successfully
        }
        catch (Exception ex)
        {
            model.id.ErrorId().AddMessage("Error during artifact generation: " + ex.Message);
            return (false, string.Empty, string.Empty); // Artifact generation failed
        }
    }

    public IEnumerable<TemplateSelection.Template> GetAvailableTemplates()
    {
        var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates", "TemplateSetting.json");
        if (!File.Exists(templatesPath))
        {
            return Enumerable.Empty<TemplateSelection.Template>();
        }

        var json = File.ReadAllText(templatesPath);
        var parsed = JsonConvert.DeserializeObject<TemplateSelection.Templates>(json);
        if (parsed?.GroupwiseTemplates == null)
        {
            return Enumerable.Empty<TemplateSelection.Template>();
        }

        return parsed.GroupwiseTemplates.SelectMany(g => g.Template);
    }

    private void LogAnalysisResults(Project model, ExtractorAnalysis analysis)
    {
        model.id.AddMessage(Environment.NewLine + "-------------------------------------------------------------------");
        model.id.AddMessage("| Analysis of the project                                         |");
        model.id.AddMessage("|-----------------------------------------------------------------|");
        model.id.AddMessage($"| {"Property",-30} | {"Value",-30} |");
        model.id.AddMessage("|-----------------------------------------------------------------|");
        model.id.AddMessage($"| {"Project Name",-30} | {model.ProjectName.PadRight(30)} |");
        model.id.AddMessage($"| {"Process Template Type",-30} | {model.ProcessTemplate.PadRight(30)} |");
        model.id.AddMessage($"| {"Teams Count",-30} | {analysis.teamCount.ToString().PadRight(30)} |");
        model.id.AddMessage($"| {"Iterations Count",-30} | {analysis.IterationCount.ToString().PadRight(30)} |");
        model.id.AddMessage($"| {"Build Definitions Count",-30} | {analysis.BuildDefCount.ToString().PadRight(30)} |");
        model.id.AddMessage($"| {"Release Definitions Count",-30} | {analysis.ReleaseDefCount.ToString().PadRight(30)} |");

        if (analysis.WorkItemCounts.Count > 0)
        {
            model.id.AddMessage("|-----------------------------------------------------------------|");
            model.id.AddMessage("| Work Items Count:                                               |");
            model.id.AddMessage("|-----------------------------------------------------------------|");
            foreach (var item in analysis.WorkItemCounts)
            {
                model.id.AddMessage($"| {item.Key.PadRight(30)} | {item.Value.ToString().PadRight(30)} |");
            }
        }
        if (analysis.ErrorMessages.Count > 0)
        {
            model.id.AddMessage("|-----------------------------------------------------------------|");
            model.id.AddMessage("| Errors:                                                         |");
            model.id.AddMessage("|-----------------------------------------------------------------|");
            foreach (var item in analysis.ErrorMessages)
            {
                model.id.AddMessage($"| {item.PadRight(60)} |");
            }
        }
        model.id.AddMessage("-------------------------------------------------------------------");
    }
}
