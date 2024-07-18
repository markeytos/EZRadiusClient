# EZRadius Client
This Repo contains the Nuget Package for [EZRadius](https://www.keytos.io/docs/cloud-radius/). The EZRadiusClient contains several functions to get, create, edit, and delete EZRadius policies in addition to getting authorization logs.

## Getting Started
1) Add the Nuget Package to your project with ```dotnet add package EZRadiusClient ```
2) Use the SampleApp to see how the client can be used and configured

## Sample Application

The SampleApp is a simple console application that demonstrates how to use the EZRadiusClient. It gets the current Radius policies, saves the allowed IP addresses and their corresponding secrets to a file, and it also reads IP addresses and their secrets from the file to update the Radius policy. 

To run, it requires a scope ID, a url for EZRadius instance (defaults to usa.ezradius.io), and an Active Directory instance (Entra ID). It also takes Application Insights connection string for logging. These parameters can be passed as command line arguments based on the flags below.
```
    -s, --scope          The scope ID for the EZRadius instance.
    
    -u, --url            The URL for the EZRadius instance. (Default: https://usa.ezradius.io)
    
    -a, --adUrl          The Active Directory instance (Entra ID) url.
    
    -i, --insight        The Application Insights connection string. (Optional)
    
    -o, --file           The output file path to save or read the IP addresses and their secrets.
```

While inside the SampleApp directory, run the following command to start the application while passing the necessary parameters:
```dotnet run -s <scope_id> -u <instance_url> -a <ad_url> -i <insight_connection_string> -o <output_file_path>```