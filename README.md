# Cross Tenant Daemon App Using Microsoft Identity Platform

## About this sample

### Overview

This sample is typically relevant if you are an ISV (independent software vendor) creating a cloud app for customers already using Microsoft Azure and/or Microsoft 365. The cloud app should be able to access data available through Microsoft Graph, as a daemon.  

You want your customers (typically an Azure AD administrator) to be able sign up to your app, and as part of the sign-up process provide consent to specific permissions needed for the app you're providing.  

The app will then be able to run in a non-interactive process in your tenant and be able to access Microsoft Graph according to the permissions that the customer consented to.

### Scenario

The app is a multi-tenant app.