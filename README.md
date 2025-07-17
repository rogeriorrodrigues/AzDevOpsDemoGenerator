# Azure DevOps Demo Generator


## About

The Azure DevOps Demo Generator can create projects in your Azure DevOps organization, prepopulated with template-based content including source code, work items, iterations, service endpoints, build and release definitions, and more!

The original purpose of this system is to simplify working with the [Azure DevOps hands-on-labs](https://www.azuredevopslabs.com), demos and other education material. But it can also be used to drive your ownAzure DevOps automation utilities, provision your own custom templates, or as a reference for using the [Azure DevOps REST APIs](https://learn.microsoft.com/en-us/rest/api/azure/devops/).

## Contributions

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

For more information on contributing, please visit the [contributor guide](./CONTRIBUTING.md).

## Contributors:

- [Akshay Hosur](https://github.com/akshay-online)

## Web Application

A new ASP.NET Core project (`ADOGenerator.Web`) exposes minimal API endpoints that leverage the existing services to create projects or generate template artifacts. This application can be deployed to Azure App Service.

### Running locally

```bash
cd src/ADOGenerator.Web
dotnet run
```

### Deploying to Azure

Publish the project and deploy the generated output to an Azure App Service instance using the Azure portal or the `az webapp` CLI commands.
