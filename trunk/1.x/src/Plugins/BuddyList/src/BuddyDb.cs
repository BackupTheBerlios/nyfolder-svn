/* [ BuddyList/BuddyDb.cs ] NyFolder Buddy DB
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
	public static class BuddyDb {
		// ============================================
		// PUBLIC Accounts Methods
		// ============================================
		/// Return All The Stored Accounts (Name)
		public static string[] Accounts() {
			AccountsTable table = new AccountsTable();
			string[] accounts = table.GetAllAccounts();
			table.Dispose();
			return(accounts);
		}

		/// Get ID Of The Account
		public static int AccountId (string username) {
			AccountsTable table = new AccountsTable();
			int id = table.GetUserId(username);
			table.Dispose();
			return(id);
		}

		/// Get Account Password From User ID
		public static string AccountPassword (int id) {
			AccountsTable table = new AccountsTable();
			string password = table.GetPassword(id);
			table.Dispose();
			return(password);
		}

		/// Get Account Password From UserName
		public static string AccountPassword (string username) {
			AccountsTable table = new AccountsTable();
			string password = table.GetPassword(username);
			table.Dispose();
			return(password);
		}

		/// Set Account Password From User Id
		public static void SetAccountPassword (int id, string password) {
			AccountsTable table = new AccountsTable();
			table.SetPassword(id, password);
			table.Dispose();
		}

		/// Set Account Password From UserName
		public static void SetAccountPassword (string username, string password) {
			AccountsTable table = new AccountsTable();
			table.SetPassword(username, password);
			table.Dispose();
		}

		/// Add New Account Into DB Table
		public static int AddAccount (string username, string password) {
			AccountsTable table = new AccountsTable();
			int id = table.Insert(username, password);
			table.Dispose();
			return(id);
		}

		/// Remove Account From Id
		public static void RemoveAccount (int id) {
			AccountsTable table = new AccountsTable();
			table.Remove(id);
			table.Dispose();
		}

		/// Remove Account From Id
		public static void RemoveAccount (string username) {
			AccountsTable table = new AccountsTable();
			table.Remove(username);
			table.Dispose();
		}

		// ============================================
		// PUBLIC Users Methods
		// ============================================
		public static string[] AccountUsers (int id) {
			UsersTable table = new UsersTable();
			string[] users = table.GetAllUsers(id);
			table.Dispose();
			return(users);
		}

		public static string[] AccountAcceptedUsers (int id) {
			UsersTable table = new UsersTable();
			string[] users = table.GetAcceptedUsers(id);
			table.Dispose();
			return(users);
		}

		public static string[] AccountNotAcceptedUsers (int id) {
			UsersTable table = new UsersTable();
			string[] users = table.GetNotAcceptedUsers(id);
			table.Dispose();
			return(users);
		}

		public static void AddUser (int id, string username, bool accept) {
			UsersTable table = new UsersTable();
			table.Insert(id, username, accept);
			table.Dispose();
		}

		public static bool UserIsInList (int id, string username) {
			UsersTable table = new UsersTable();
			bool present = table.IsPresent(id, username);
			table.Dispose();
			return(present);
		}

		// ============================================
		// PUBLIC Accounts/Users Methods
		// ============================================
		public static string[] AccountUsers (string username) {
			return(AccountUsers(AccountId(username)));
		}

		public static string[] AccountAcceptedUsers (string username) {
			return(AccountAcceptedUsers(AccountId(username)));
		}

		public static string[] AccountNotAcceptedUsers (string username) {
			return(AccountNotAcceptedUsers(AccountId(username)));
		}
	}
}
