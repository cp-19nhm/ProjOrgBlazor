using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using static BlazorTruc.Pages.Index;

public class DbManager
{
    private readonly string connectionString = "Server=127.0.0.1;port=3306;Database=json_files;Uid=root;Pwd=1234";

    public void InsertJsonFiles(string[] files)
    {
        //var files = Directory.GetFiles("R:/test/test", "*.POrgConf.json", SearchOption.AllDirectories);

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var jsonData = JsonConvert.DeserializeObject<Project>(json);
                if (!ProjectExists(jsonData.Titre))
                {

                    var query = "INSERT INTO files (Titre, Description, Auteur, Responsable, Date, Language, Keywords, Color, Client, StateProj, TypeProj) VALUES (@titre, @description, @auteur, @responsable, @date, @language, @keywords, @color, @client, @stateProj, @typeProj)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@titre", jsonData.Titre ?? "");
                        command.Parameters.AddWithValue("@description", jsonData.Description ?? "");
                        command.Parameters.AddWithValue("@auteur", jsonData.Auteur ?? "");
                        command.Parameters.AddWithValue("@responsable", jsonData.Responsable ?? "");
                        command.Parameters.AddWithValue("@date", jsonData.Date ?? "");
                        command.Parameters.AddWithValue("@language", jsonData.Language ?? "");
                        command.Parameters.AddWithValue("@keywords", jsonData.Keywords ?? "");
                        command.Parameters.AddWithValue("@color", jsonData.Color ?? "");
                        command.Parameters.AddWithValue("@client", jsonData.Client ?? "");
                        command.Parameters.AddWithValue("@stateProj", jsonData.StateProj ?? "");
                        command.Parameters.AddWithValue("@typeProj", jsonData.TypeProj ?? "");

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    private bool ProjectExists(string titre)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM files WHERE Titre = @titre";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@titre", titre);
            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    public List<Project> GetFilesFromDatabase()
    {
        // Create a list to store the Project objects
        List<Project> jsonFiles = new List<Project>();

        // Create a new MySqlConnection and connect to the database
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Create a new MySqlCommand to retrieve the files from the database
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM json_files.files", connection))
            {
                // Execute the command and retrieve the data
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new Project object and set its properties
                        Project jsonFile = new Project();
                        jsonFile.Id = reader.GetInt32("id");
                        jsonFile.Titre = reader["Titre"].ToString();
                        jsonFile.Description = reader["Description"].ToString();
                        jsonFile.Auteur = reader["Auteur"].ToString();
                        jsonFile.Responsable = reader["Responsable"].ToString();
                        jsonFile.Date = reader["Date"].ToString();
                        jsonFile.Language = reader["Language"].ToString();
                        jsonFile.Keywords = reader["Keywords"].ToString();
                        jsonFile.Color = reader["Color"].ToString();
                        jsonFile.Client = reader["Client"].ToString();
                        jsonFile.StateProj = reader["StateProj"].ToString();
                        jsonFile.TypeProj = reader["TypeProj"].ToString();

                        jsonFiles.Add(jsonFile);
                    }
                }
            }
        }

        return jsonFiles;
    }

    //public void RemoveDuplicateProjects(Project project)
    //{
    //    using (var connection = new SqlConnection(connectionString))
    //    {
    //        connection.Open();
    //        using (var command = new SqlCommand("DELETE FROM json_files.files WHERE Titre = @Titre AND id != @id", connection))
    //        {
    //            command.Parameters.AddWithValue("@Titre", project.Titre);
    //            command.Parameters.AddWithValue("@id", project.Id);
    //            int rowsAffected = command.ExecuteNonQuery();
    //            if (rowsAffected > 0)
    //            {
    //                Console.WriteLine($"Deleted {rowsAffected} duplicate project(s) with title {project.Titre}");
    //            }
    //        }
    //    }
    //}
}
