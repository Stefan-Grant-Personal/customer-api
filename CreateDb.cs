/*
This file creates a basic SQLite database and reads it

The code is adapted from https://www.technical-recipes.com/2016/using-sqlite-in-c-net-environments/
*/

using System.Data.SQLite;


namespace CustomerApi.SQLiteUtils;

public class SQLiteDbBuilder
{
    public static void CreateDb(string fileName, string createTableQuery)
    {
        // Create the file which will be hosting the database
        SQLiteConnection.CreateFile(fileName);

        // Connect to the database
        using (var connection = new SQLiteConnection($"Data Source={fileName}"))
        {
            // Create a database command
            using (var command = new SQLiteCommand(connection))
            {
                connection.Open();

                // Create the table
                command.CommandText = createTableQuery;
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
