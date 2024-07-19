# EZRadius Client

This repository contains the Nuget Package for [EZRadius](https://www.keytos.io/docs/cloud-radius/). The EZRadiusClient contains several functions to get, create, edit, and delete EZRadius policies in addition to getting authorization logs. The SampleApp is a simple console application that shows how to use the EZRadiusClient. Shown below are example commands that demonstrate how to invoke the different EZRadiusClient methods. 

To run the SampleApp, it requires a token scope ID, a url for EZRadius instance (defaults to usa.ezradius.io), and an Azure Active Directory instance (Entra ID). It also takes a connection string for logging to Azure Application Insights. These parameters can be passed as command line arguments based on the flags below.

## Getting Started
1) Add the Nuget Package to your project with ```dotnet add package EZRadiusClient ```
2) Use the SampleApp to see how the client can be used and configured (must have EZRadius instance with a Radius policy)

## Displaying Radius Policies

Starting with a basic feature, the ```show``` verb will display all the Radius policies. This command calls the ```GetRadiusPoliciesAsync()``` method and prints to the console the Radius policies and their attributes currently in the passed EZRadius instance.   

```
    -s, --scope          The token scope for the EZRadius instance.
    
    -u, --url            The URL for the EZRadius instance. (Default: https://usa.ezradius.io)
    
    -a, --adUrl          The Azure Active Directory instance (Entra ID) url.
    
    -l, --log            The Azure Application Insights connection string. (Optional)
```

Sample Call: ```dotnet run show -s <scope_id> -u <instance_url> -a <ad_url> -l <insight_connection_string>```

## Downloading Allowed IP Addresses from Policy

Building off of the previous command, this command also calls the ```GetRadiusPoliciesAsync()``` method to get the policies, but it saves all the allowed IP addresses of a policy into a .csv file. The program asks the user which policy's IP addresses should be saved. To run this, use the ```download``` verb when running the SampleApp with the following flags. An example of the output .csv file is in the SampleApp directory and titled ```IPs.csv```.

```
    -s, --scope          The token scope for the EZRadius instance.
    
    -u, --url            The URL for the EZRadius instance. (Default: https://usa.ezradius.io)
    
    -a, --adUrl          The Azure Active Directory instance (Entra ID) url.
    
    -l, --log            The Azure Application Insights connection string. (Optional)
    
    -o, --output         The output file path to save or read the IP addresses and their secrets from Radius policy.
```

Sample Call: ```dotnet run download -s <scope_id> -u <instance_url> -a <ad_url> -l <insight_connection_string> -o <path_to_output_file>```


## Updating Allowed IP Addresses for Policy

This command utilizes the ```CreateOrEditRadiusPolicyAsync()``` method to make changes to an existing Radius policy from the EZRadius instance. The method takes in the ```RadiusPolicyModel``` which is one of the fields returned by the ```GetRadiusPoliciesAsync()```. It takes a .csv file containing IP addresses and their secrets; the format should match that of the sample .csv file in the SampleApp directory titled ```IPs.csv```. The IP addresses will overwrite the current IP addresses for that policy, which is then saved in the EZRadius instance. This command is run using the ```upload``` verb.  

```
    -s, --scope          The token scope for the EZRadius instance.
    
    -u, --url            The URL for the EZRadius instance. (Default: https://usa.ezradius.io)
    
    -a, --adUrl          The Azure Active Directory instance (Entra ID) url.
    
    -l, --log            The Azure Application Insights connection string. (Optional)
    
    -i, --input         The input file path containing IP addresses and their secrets for updating Radius policy.
```

Sample Call: ```dotnet run upload -s <scope_id> -u <instance_url> -a <ad_url> -l <insight_connection_string> -i <path_to_input_file>```

## Deleting Radius Policy

Deleting a Radius policy can be done using the ```delete``` verb. This command calls the ```DeleteRadiusPolicyAsync()``` which takes in a ```RadiusPolicyModel``` and deletes that Radius policy from the EZRadius instance. This command cannot be undone, so once the policy is removed, it will cannot be recovered.

```
    -s, --scope          The token scope for the EZRadius instance.
    
    -u, --url            The URL for the EZRadius instance. (Default: https://usa.ezradius.io)
    
    -a, --adUrl          The Azure Active Directory instance (Entra ID) url.
    
    -l, --log            The Azure Application Insights connection string. (Optional)   
```

Sample Call: ```dotnet run delete -s <scope_id> -u <instance_url> -a <ad_url> -l <insight_connection_string>```

## Displaying Authorization Logs

Similar to the displaying Radius policies, this command gets authentication logs and displays them to the console. It uses the ```GetAuthAuditLogsAsync()``` method to get the logs from the EZRadius instance, and prints information about each log to the terminal. To run this command, use the verb ```getlogs```. The SampleApp is configured to return logs for the previous 2 days; to change this see ```TimeFrameModel``` in the EZRadiusClient directory.

```
    -s, --scope          The token scope for the EZRadius instance.
    
    -u, --url            The URL for the EZRadius instance. (Default: https://usa.ezradius.io)
    
    -a, --adUrl          The Azure Active Directory instance (Entra ID) url.
    
    -l, --log            The Azure Application Insights connection string. (Optional)
```

Sample Call: ```dotnet run getlogs -s <scope_id> -u <instance_url> -a <ad_url> -l <insight_connection_string>```