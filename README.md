# Cross Tenant Daemon App Using Microsoft Identity Platform

## About this sample

This sample is written with [ASP.NET Core 6](https://devblogs.microsoft.com/dotnet/announcing-asp-net-core-in-net-6/) with [Razor](https://docs.microsoft.com/en-us/dotnet/architecture/porting-existing-aspnet-apps/comparing-razor-pages-aspnet-mvc) and [Microsof Identity Web authentication library (Identity.Web)](https://docs.microsoft.com/en-us/azure/active-directory/develop/microsoft-identity-web). The Daemon app uses [Microsoft Authentication Library (MSAL)](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-overview).

### Overview

This sample shows how an ISV can create a SaaS app where customers can sign-up to for the app and the app can access customer data in a long-running and non-interactive way, providing that the customer provides their consent to access this data. 

The sign-up flow looks like this, at a high level:

1. The ISV creates a multi-tenant app registration in the ISV's home tenant.
2. The customer signs up to the app by visiting the ISV's app **landing page**. 
2. Due to the long-running and non-interactive nature of the data access needed, it will in most cases - if not all cases - need to be an administrator from the customers own tenant that does the sign-up, for the app.
3. As part of the sign-up process, the customer administrator will be asked to consent to a set of delegate permissions. For more details about delegate and application permissions see: [Permission Types](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#permission-types).
4. As the customer provide their consent to these permissions, a [service principal](https://docs.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals) for the ISV app will automatically be created in the customers own tenant. This service principal is a representation of the app in the customers own tenant.
5. To ensure that the service principal have the needed application permissions to access data from a non-interative sessions, one or more application permissions now need to be granted for this service principal. This process is typically referede to as: [App Role Assignment](https://docs.microsoft.com/en-us/graph/api/serviceprincipal-post-approleassignments?view=graph-rest-1.0&tabs=http). App role assignments can be done manually by a customer Azure admin using [az cli](https://docs.microsoft.com/en-us/cli/azure/). Or it can be done via ISV's landing page on behalf of the user usin Microsoft Graph, providing that the user have consented to the needed delegate permissions for doing service principal app role assignments.

After the sign-up process completes successfully, a daemon app can now be run by the ISV in the ISV's own Azure subscription. To run, the daemon app need only to know the tenant id of the customer. To gain access to the customers data, the daemon app uses MSAL with the customers tenant id together with the appid and the app secret (or certificate) for the multi-tenant app registration, which were created in the ISV's own home tenant. No app secrets (or certificates) need to be exchanged between the ISV and the customer for running the daemon app, which makes this approach to creating a cross  tenant daemon app secure and easy to manage for ISV. Additionally, the customer remains in control, as the the custom can at any time delete the service principal in the custoomers own tenant, which will revoke the daemon app permissions.

### How to run this sample

To run this sample you'll need:

- Visual Studio 2022
- An internet connection
- Two Azure AD tenants. You can use your existing Azure AD tenant and create a second free tenant if you do not already have one, as explained below.

#### Step 1: Clone this tenant

From your shell or command line:

```
git clone https://github.com/Azure-Samples/ms-identity-cross-tenant-daemon.git
```

or download and exact the repository .zip file.

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

### Step 2: Create a second AD Tenant

To create a free Azure AD Tenant follow this [quickstart guide](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-access-create-new-tenant). 

### Step 3: Create multi-tenant app registration

The multi-tenant app registration will be created in the new Azure AD tenant that was created in step 2.

