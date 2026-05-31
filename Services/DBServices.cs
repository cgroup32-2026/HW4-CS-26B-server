using System.Data;
using System.Data.SqlClient;

namespace tar1.Services
{
    public class DBServices
    {
        private SqlConnection con;
        private SqlCommand cmd;

        private void Connect()
        {
            string connStr = "Data Source=Media.ruppin.ac.il;Initial Catalog=igroup132_test2;User ID=igroup132;Password=igroup132_59195";
            con = new SqlConnection(connStr);
            con.Open();
        }

        private void Disconnect()
        {
            if (con != null && con.State == ConnectionState.Open)
                con.Close();
        }

        private Models.Game ReadGameFromReader(SqlDataReader reader)
        {
            return new Models.Game
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] == DBNull.Value ? "" : reader["Name"].ToString(),
                SteamUrl = reader["SteamUrl"] == DBNull.Value ? "" : reader["SteamUrl"].ToString(),
                Image = reader["Image"] == DBNull.Value ? "" : reader["Image"].ToString(),
                ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? "" : reader["ReleaseDate"].ToString(),
                ReviewSummary = reader["ReviewSummary"] == DBNull.Value ? "" : reader["ReviewSummary"].ToString(),
                Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Price"]),
                Windows = reader["Windows"] != DBNull.Value && Convert.ToBoolean(reader["Windows"]),
                Mac = reader["Mac"] != DBNull.Value && Convert.ToBoolean(reader["Mac"]),
                Linux = reader["Linux"] != DBNull.Value && Convert.ToBoolean(reader["Linux"]),
                Tags = new List<string>()
            };
        }

        //****************USERS **********

        public List<Dictionary<string, object>> GetAllUsers()
        {
            List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetAllUsers_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var user = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        user[reader.GetName(i)] = reader[i];
                    users.Add(user);
                }
            }
            finally { Disconnect(); }
            return users;
        }

        public int InsertUser(string name, string email, string password, bool active)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_InsertUser_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@Active", active);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            finally { Disconnect(); }
        }

        public Dictionary<string, object> LoginUser(string email, string password)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_LoginUser_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var user = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        user[reader.GetName(i)] = reader[i];
                    return user;
                }
                return null;
            }
            finally { Disconnect(); }
        }

        public bool UpdateUser(int id, string name, string password)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_UpdateUser_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Password", password);
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            finally { Disconnect(); }
        }

        //***************************** GAMES **************

        public List<Models.Game> GetAllGames()
        {
            List<Models.Game> games = new List<Models.Game>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetAllGames_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    var existing = games.FirstOrDefault(g => g.Id == id);
                    if (existing == null)
                    {
                        existing = ReadGameFromReader(reader);
                        games.Add(existing);
                    }
                    if (reader["TagName"] != DBNull.Value)
                        existing.Tags.Add(reader["TagName"].ToString());
                }
            }
            finally { Disconnect(); }
            return games;
        }

        public List<Models.Game> GetGamesByName(string name)
        {
            List<Models.Game> games = new List<Models.Game>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetGamesByName_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    var existing = games.FirstOrDefault(g => g.Id == id);
                    if (existing == null)
                    {
                        existing = ReadGameFromReader(reader);
                        games.Add(existing);
                    }
                    if (reader["TagName"] != DBNull.Value)
                        existing.Tags.Add(reader["TagName"].ToString());
                }
            }
            finally { Disconnect(); }
            return games;
        }

        public int InsertGame(Models.Game game)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_InsertGame_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", game.Name);
                cmd.Parameters.AddWithValue("@SteamUrl", game.SteamUrl ?? "");
                cmd.Parameters.AddWithValue("@Image", game.Image ?? "");
                cmd.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate ?? "");
                cmd.Parameters.AddWithValue("@ReviewSummary", game.ReviewSummary ?? "");
                cmd.Parameters.AddWithValue("@Price", game.Price);
                cmd.Parameters.AddWithValue("@Windows", game.Windows);
                cmd.Parameters.AddWithValue("@Mac", game.Mac);
                cmd.Parameters.AddWithValue("@Linux", game.Linux);
                object result = cmd.ExecuteScalar();
                int newId = Convert.ToInt32(result);

                // insert tags
                if (game.Tags != null)
                {
                    foreach (var tag in game.Tags)
                        InsertTag(newId, tag);
                }
                return newId;
            }
            finally { Disconnect(); }
        }

        public bool UpdateGame(int id, Models.Game game)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_UpdateGame_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", game.Name);
                cmd.Parameters.AddWithValue("@SteamUrl", game.SteamUrl ?? "");
                cmd.Parameters.AddWithValue("@Image", game.Image ?? "");
                cmd.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate ?? "");
                cmd.Parameters.AddWithValue("@ReviewSummary", game.ReviewSummary ?? "");
                cmd.Parameters.AddWithValue("@Price", game.Price);
                cmd.Parameters.AddWithValue("@Windows", game.Windows);
                cmd.Parameters.AddWithValue("@Mac", game.Mac);
                cmd.Parameters.AddWithValue("@Linux", game.Linux);
                int rows = cmd.ExecuteNonQuery();
                Disconnect();

                // update tags
                if (game.Tags != null)
                {
                    DeleteGameTags(id);
                    foreach (var tag in game.Tags)
                        InsertTag(id, tag);
                }
                return rows > 0;
            }
            finally { Disconnect(); }
        }

        public bool DeleteGame(int id)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_DeleteGame_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            finally { Disconnect(); }
        }

        public void InsertTag(int gameId, string tagName)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_InsertTag_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GameId", gameId);
                cmd.Parameters.AddWithValue("@TagName", tagName);
                cmd.ExecuteNonQuery();
            }
            finally { Disconnect(); }
        }

        public List<string> GetAllTags()
        {
            List<string> tags = new List<string>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetAllTags_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    tags.Add(reader["TagName"].ToString());
            }
            finally { Disconnect(); }
            return tags;
        }

        public List<Models.Game> GetGamesByTag(string tagName)
        {
            List<Models.Game> games = new List<Models.Game>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetGamesByTags_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TagName", tagName);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    var existing = games.FirstOrDefault(g => g.Id == id);
                    if (existing == null)
                    {
                        existing = ReadGameFromReader(reader);
                        games.Add(existing);
                    }
                    if (reader["TagName"] != DBNull.Value)
                        existing.Tags.Add(reader["TagName"].ToString());
                }
            }
            finally { Disconnect(); }
            return games;
        }

        public bool AddUserGame(int userId, int gameId)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_AddUserGame_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@GameId", gameId);
                cmd.ExecuteNonQuery();
                return true;
            }
            finally { Disconnect(); }
        }


        public void DeleteGameTags(int gameId)
        {
            try
            {
                Connect();
                cmd = new SqlCommand("SP_DeleteGameTags_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GameId", gameId);
                cmd.ExecuteNonQuery();
            }
            finally { Disconnect(); }
        }


        public List<Models.Game> GetUserGames(int userId)
        {
            List<Models.Game> games = new List<Models.Game>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetUserGames_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    var existing = games.FirstOrDefault(g => g.Id == id);
                    if (existing == null)
                    {
                        existing = ReadGameFromReader(reader);
                        games.Add(existing);
                    }
                    if (reader["TagName"] != DBNull.Value)
                        existing.Tags.Add(reader["TagName"].ToString());
                }
            }
            finally { Disconnect(); }
            return games;
        }

        public List<Models.Game> GetRecommendedGames(int userId)
        {
            List<Models.Game> games = new List<Models.Game>();
            try
            {
                Connect();
                cmd = new SqlCommand("SP_GetRecommendedGames_HW", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    var existing = games.FirstOrDefault(g => g.Id == id);
                    if (existing == null)
                    {
                        existing = ReadGameFromReader(reader);
                        games.Add(existing);
                    }
                    if (reader["TagName"] != DBNull.Value)
                        existing.Tags.Add(reader["TagName"].ToString());
                }
            }
            finally { Disconnect(); }
            return games;
        }
    }
}