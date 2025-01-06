# sts-ai-support

This project is a proof of concept to explore the capabilities of Azure OpenAI to support internal proccesses associated with [Plan technology for your school](https://github.com/DFE-Digital/sts-plan-technology-for-your-school).  
We are working with models set up in the DfE Azure OpenAI sandbox environment. This app has been developed to enable interaction with the sandbox models programmatically to assist with monitoring the impact of different settings on our results.  
The next stage of development (as of 06/01/25) will involve incorporating our own data sources to set context, provide examples and inform responses.  

## Running locally

### Pre-requisites
- .NET 8.0 and IDE for local running
- [Azure OpenAI 2.1.0](https://www.nuget.org/packages/Azure.AI.OpenAI)
- Azure subscription with permissions to create Azure OpenAI resources (this may incur costs for you or your organisation)

## Setup
1. Create an Azure OpenAI resource - note Keys and Endpoints in the Overview section
2. In Azure AI Foundry under the new resource, [deploy a base model](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/deploy-models-openai) of your choice and give it a unique name
3. Update `appsettings.json` or use dotnet user-secrets to set the following:
  - AzureOpenAI:Endpoint = ENDPOINT OF YOUR AZURE OPENAI RESOURCE
  - AzureOpenAI:ApiKey = KEY FROM YOUR AZURE OPENAI RESOURCE
  - AzureOpenAI:DeploymentName = NAME OF YOUR DEPLOYED MODEL
4. Adjust the system message, temperature and max tokens in `Index.cshtml.cs` as desired


