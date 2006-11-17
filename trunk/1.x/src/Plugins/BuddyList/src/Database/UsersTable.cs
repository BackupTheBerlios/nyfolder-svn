/* [ BuddyList/UsersTable.cs ] NyFolder Users Table
 * Author: Matteo Bertozzi
 * ============================================================================
 * This file is part of NyFolder.
 *
 * NyFolder is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * NyFolder is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with NyFolder; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.IO;
using System.Data;
using System.Collections;

using Niry;
using Niry.Utils;
using Niry.Database;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.BuddyList {
	public class UsersTable : DbTable {
		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Open Db Connection With Users Table
		public UsersTable() : base("users") {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Get All The Users of The Specified Account
		public string[] GetAllUsers (int id) {
			string sql = "SELECT username FROM users WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			return(db.ExecuteReadStrings(sql, sqlParams));
		}

		/// Get All The Accepted Users of The Specified Account
		public string[] GetAcceptedUsers (int id) {
			string sql = "SELECT username FROM users WHERE id=@Id AND accept=1;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			return(db.ExecuteReadStrings(sql, sqlParams));
		}

		/// Get All The Non Accepted Users of The Specified Account
		public string[] GetNotAcceptedUsers (int id) {
			string sql = "SELECT username FROM users WHERE id=@Id AND accept=0;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			return(db.ExecuteReadStrings(sql, sqlParams));
		}

		public void Remove (string username) {
			string sql = "DELETE from users WHERE username=@Name;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Name", username);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		public void RemoveAll (int id) {
			string sql = "DELETE from users WHERE id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		public int Insert (int id, string username, bool accept) {
			try {
				string sql = "INSERT INTO users VALUES (@Id, @Name, @Accept);";
				Hashtable sqlParams = new Hashtable();
				sqlParams.Add("@Id", id);
				sqlParams.Add("@Name", username);
				sqlParams.Add("@Accept", (accept == true) ? 1 : 0);
				return(db.ExecuteNonQueryGetID(sql, sqlParams));
			} catch {
				return(-1);
			}
		}

		public void SetAccept (int id, string username, bool accept) {
			string sql = "UPDATE users SET accept=@Accept WHERE username=@Name AND id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			sqlParams.Add("@Name", username);
			sqlParams.Add("@Accept", (accept == true) ? 1 : 0);
			db.ExecuteNonQuery(sql, sqlParams);
		}

		public bool GetAccept (int id, string username) {
			string sql = "SELECT accept FROM users WHERE username=@Name AND id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			sqlParams.Add("@Name", username);
			return(db.ExecuteReadInt(sql, sqlParams) == 1 ? true : false);
		}

		public bool IsPresent (int id, string username) {
			string sql = "SELECT accept FROM users WHERE username=@Name AND id=@Id;";
			Hashtable sqlParams = new Hashtable();
			sqlParams.Add("@Id", id);
			sqlParams.Add("@Name", username);
			return(db.ExecuteReadInt(sql, sqlParams) >= 0 ? true : false);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		// +----------+------------------+
		// | id       | INTEGER          |
		// +----------+------------------+
		// | username | VARCHAR(32 + 24) |
		// +----------+------------------+
		// | accept   | INTEGER          |
		// +----------+------------------+
		protected override void CreateAccountsTable() {
			string sql = "CREATE TABLE users (" +
							"id INTEGER NOT NULL," +
							"username VARCHAR(56) NOT NULL," +
							"accept INTEGER NOT NULL," +
							"PRIMARY KEY (id, username)" +
						 ");";
			db.ExecuteNonQuery(sql);
		}

	}
}
