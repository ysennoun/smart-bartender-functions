# smart-bartender-functions

Project regrouping all serverless functions to be used in the project smart-bartender.

## Structure 

Here is the structure of this module : 

    FunctionApp
     | - host.json
     | - ReadCommand
     | | - function.json
     | | - run.csx
     | - GetCommand
     | | - function.json
     | | - run.csx 
     | | - run.csx
     | - ServeCommand
     | | - function.json
     | | - run.csx 
     | - library
     | | - dao
     | | | cosmosClient.csx
     | | - model
     | | | command.csx    
     | - bin

## Composition

It is composed of three modules : 

- ReadCommand : receive command and store in data base with status "not served"
- GetCommand : return all unserved command
- ServeCommand : for a command, change status to "served"