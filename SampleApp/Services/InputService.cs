using EZRadiusClient.Models;

namespace SampleApp.Services;

public class InputService
{
    public static ArgumentsModel ValidateArguments(ArgumentsModel passedArguments)
    {
        if (string.IsNullOrWhiteSpace(passedArguments.ADUrl))
        {
            passedArguments.ADUrl = "https://login.microsoftonline.com/";
        }
        if (string.IsNullOrWhiteSpace(passedArguments.InstanceUrl))
        {
            passedArguments.InstanceUrl = "https://usa.ezradius.io/";
        }
        if (string.IsNullOrWhiteSpace(passedArguments.Scope))
        {
            passedArguments.Scope = "https://management.core.windows.net/.default";
        }
        return passedArguments;
    }

    public static int ChooseRadiusPolicy(
        List<RadiusPolicyModel> currentRadiusPolicies,
        string action
    )
    {
        Console.WriteLine("Choose a policy to " + action + ": ");
        for (int policyIndex = 0; policyIndex < currentRadiusPolicies.Count; policyIndex++)
        {
            Console.WriteLine(
                $"Enter {policyIndex} to select {currentRadiusPolicies[policyIndex].PolicyName}"
            );
        }
        int chosenPolicyIndex = -1;
        while (chosenPolicyIndex >= currentRadiusPolicies.Count || chosenPolicyIndex < 0)
        {
            string? userInput = Console.ReadLine();
            if (!int.TryParse(userInput, out chosenPolicyIndex))
            {
                Console.WriteLine(
                    $"Invalid selection: Please enter a value between 0 and {currentRadiusPolicies.Count - 1}"
                );
            }
        }
        return chosenPolicyIndex;
    }
}
