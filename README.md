# Cross Tenant Daemon App Using Microsoft Identity Platform

## About this sample

### Overview

This sample shows how an ISV can create an application that enable customers to sign-up to a SaaS application that can access customer data in a long-running and non-interactive way, providing that the customer provide their consent to access this data. 

The sign-up flow looks like this:

1. The ISV creates a multi-tenant app registration.
2. The customer signs up to the app by visiting the ISV's app **landing page**. Do to the non-interactive nature of the data access needed, it will typically need to be an administrator from the customers own tenant, who is doing the app sign-up.
3. As part of the sign-up process the customer will be asked to consent to a set of delegate permissions needed to complete the sign-up . For more see: [Permission Types](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#permission-types).
4. As the customer provide their consent a [service principal](https://docs.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals) for the ISV app will be created in the customers own tenant.
5. To complete the sign-up process, application permission(s) now needed to be granted for the service principal representing the app in the customers tenant. This process is also know as: [App Role Assignment](https://docs.microsoft.com/en-us/graph/api/serviceprincipal-post-approleassignments?view=graph-rest-1.0&tabs=http). This can be done manually by a customer Azure admin using [az cli](https://docs.microsoft.com/en-us/cli/azure/), or the ISV's landing page can use Microsoft Graph on behalf of the user, providing that the user have the needed delegate permissions, to do app role assignments to a service principal - hence the need for administrative privileges.

After the sign-up process completes successfully, a daemon app can now be run by the ISV in the ISV's own Azure subscription. To run the daemon app need only to know the tenant id of the customer. To gain access to the customers data, the daemon app use MSAL with the customers tenant id together with the app id and the app secret (or certificate) for the multi-tenant app registration in the ISV's own home tenant. No app secrets (or certificates) need to be exchanged with the customers using the app, which makes this approach to creating a cross  tenant daemon app secure and easy to manage.

Using [Razor](https://docs.microsoft.com/en-us/dotnet/architecture/porting-existing-aspnet-apps/comparing-razor-pages-aspnet-mvc).