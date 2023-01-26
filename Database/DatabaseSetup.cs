using System;
using System.IO;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using static BlazorTruc.Pages.Index;

public class DatabaseSetup
{
    private string connectionString = "Data Source=localhost;Initial Catalog=ProjetDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

    public void SetupDatabase()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            //creation database
            using (SqlCommand command = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'ProjetDB')", connection))
            {
                //Creation des tables
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
                //insert les données json dans les tables
                var jsonFiles = Directory.GetFiles("R:/test/test", ".POrgConf.json", SearchOption.AllDirectories);
                foreach (var jsonFile in jsonFiles)
                {
                    var json = File.ReadAllText(jsonFile);
                    var project = JsonConvert.DeserializeObject<Project>(json);
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