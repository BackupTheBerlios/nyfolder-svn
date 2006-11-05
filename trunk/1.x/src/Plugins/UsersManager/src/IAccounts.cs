/* [ UsersManager/IAccounts.cs ] NyFolder Accounts DB Interface
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

using Niry;
using Niry.Utils;

using NyFolder;
using NyFolder.Utils;
using NyFolder.Protocol;

namespace NyFolder.Plugins.UsersManager {
	public interface IAccounts {
		// ============================================
		// PUBLIC Methods
		// ============================================
		string[] GetAllAccounts();
		string GetUserPassword (string username);
		string GetUserName (int id);
		int GetUserId (string username);
		int Insert (string username, string password);
//		int Remove (string username);
//		void Remove (int id);

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
