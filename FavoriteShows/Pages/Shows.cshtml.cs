using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;


namespace FavoriteShows.Pages
{
    // This is a PageModel for a Razor Page that handles displaying favorite tv shows
    public class FavoriteShowsModel : PageModel
    {
        // Property that will store the selected shows ID from form submissions
        [BindProperty]
        public int SelectedShowsId { get; set; }


        // List that will hold all ducks for the dropdown selection
        public List<SelectListItem> ShowsList { get; set; }

        // Property that will store the currently selected duck object
        public Shows SelectedShows { get; set; }


        // Handles HTTP GET requests to the page - loads the list of shows
        public void OnGet()
        {
            LoadShowsList();
        }


        // Handles HTTP POST requests (when user selects a show) - loads the show list
        // and retrieves the selected show's details
        public IActionResult OnPost()
        {
            LoadShowsList();
            if (SelectedShowsId != 0)
            {
                SelectedShows = GetShowsById(SelectedShowsId);
            }
            return Page();
        }


        // Helper method that loads the list of ducks from the SQLite database
        // for displaying in a dropdown menu
        private void LoadShowsList()
        {
            ShowsList = new List<SelectListItem>();
            using (var connection = new SqliteConnection("Data Source=FavoriteShows.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Shows";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowsList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(), // show ID as the value
                            Text = reader.GetString(1)             // show title as the display text
                        });
                    }
                }
            }
        }


        // Helper method that retrieves a specific show by its ID from the database
        // Returns all details of the shows
        private Shows? GetShowsById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=FavoriteShows.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Shows WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); // Using parameterized query for security

                // Get a reader to read the resultset from the database
                using (var reader = command.ExecuteReader())
                {
                    // If reader returns a record, then we'll return that new show from the database
                    if (reader.Read())
                    {
                        return new Shows
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            ReleaseYear = reader.GetInt32(2),
                            Genre = reader.GetString(3),
                            ImageFileName = reader.GetString(4)
                        };
                    } // end if
                } // end using
            } // end using

            // if there was no show matching the ID in the database return null
            return null;
        } // end get shows method
    }


    // Simple model class representing a tv show
    // model class maps to a table in the database so that you can create objects
    // in your application that represent the intance of an entity (rows in the table)
    // in the database
    public class Shows
    {
        // The show's attributes match the columns in the shows table
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear{ get; set; }
        public string Genre { get; set; }
        public string ImageFileName { get; set; }
    } // end shows class
}    