using System;
using System.IO;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using static BlazorTruc.Pages.Index;

public class DatabaseSetup
{
    private string connectionString = "Server=localhost;Database=MyDatabase;User Id=myUsername;Password=myPassword;";

    public void SetupDatabase()
    {
        // Check the flag
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'MyDatabase')", connection))
            {
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE SetupFlag (
                                            Id INT PRIMARY KEY IDENTITY(1,1),
                                            Flag BIT NOT NULL
                                        )";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO SetupFlag (Flag) VALUES (1)";
                command.ExecuteNonQuery();
                command.CommandText = @"CREATE TABLE Projects (
                                            Id INT PRIMARY KEY IDENTITY(1,1),
                                            Title NVARCHAR(255),
                                            Description NVARCHAR(MAX),
                                            Author NVARCHAR(255),
                                            Responsible NVARCHAR(255),
                                            Date DATE,
                                            Language NVARCHAR(255),
                                            Keywords NVARCHAR(255),
                                            Color NVARCHAR(255),
                                            Client NVARCHAR(255),
                                            State NVARCHAR(255),
                                            Type NVARCHAR(255))";
                command.ExecuteNonQuery();
                var jsonFiles = Directory.GetFiles("R:/test/test", ".POrgConf.json", SearchOption.AllDirectories);
                foreach (var jsonFile in jsonFiles)
                {
                    // Read the contents of the JSON file
                    var json = File.ReadAllText(jsonFile);
                    // Deserialize the JSON into a Project object
                    var project = JsonConvert.DeserializeObject<Project>(json);
                    // Insert the data into the Projects table
                    using (SqlConnection connection1 = new SqlConnection(connectionString))
                    {
                        connection1.Open();
                        using (SqlCommand command1 = new SqlCommand(
                            "INSERT INTO Projects (Title, Description, Author, Responsible, Date, Language, Keywords, Color, Client, State, Type) " +
                            "VALUES (@title, @description, @author, @responsible, @date, @language, @keywords, @color, @client, @state, @type)", 
                            connection1)
                        )
                    {
                        command1.Parameters.AddWithValue("@title", project.Titre);
                        command1.Parameters.AddWithValue("@description", project.Description);
                        command1.Parameters.AddWithValue("@author", project.Auteur);
                        command1.Parameters.AddWithValue("@responsible", project.Responsable);
                        command1.Parameters.AddWithValue("@date", project.Date);
                        command1.Parameters.AddWithValue("@language", project.Language);
                        command1.Parameters.AddWithValue("@keywords", project.Keywords);
                        command1.Parameters.AddWithValue("@color", project.Color);
                        command1.Parameters.AddWithValue("@client", project.Client);
                        command1.Parameters.AddWithValue("@state", project.StateProj);
                        command1.Parameters.AddWithValue("@type", project.TypeProj);
                        command1.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}