# Demo Server

# Nuget
dotnet add package Newtonsoft.Json
dotnet add package Ardalis.GuardClauses
dotnet add package sqlite
dotnet add package CsvHelper

An Api server for connecting to the injesting NASA APOD json data and loading into Kafka. 

https://api.nasa.gov/planetary/apod
 
# Stack 
* c# 6
* Kafka
* Sqlite 

# ToDo
1. Unit Tests
2. Backend timer to get new APOD from NASA
3. Web Socket output for clients to connect and trigger when new APOD is available
