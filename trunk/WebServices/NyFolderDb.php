<?php
#CREATE TABLE login (
#	uid INT(9) NOT NULL AUTO_INCREMENT,
#	username CHAR(32) NOT NULL,
#	password CHAR(32) NOT NULL,
#	status INT(2) NOT NULL,
#	last_update CHAR(10) NOT NULL,
#	PRIMARY KEY (uid)
#);

# ToDo:
# =========================================
# Add 

#CREATE TABLE user_info (
#	uid INT(9) NOT NULL,
#	ip CHAR(15) NOT NULL,
#	port INT(6) NOT NULL,
#	magic CHAR(32) NOT NULL,
#	PRIMARY KEY (uid)
#);

require_once("utils.php");

class NyFolderDb {
	var $DB_NAME = 'nyfolder';
	var $DB_HOST = 'db.berlios.de';
	var $DB_USERNAME = 'nyfolder';
	var $MySQL;

	# ===============================================
	# Initialize/Destroy Db Connection
	# ===============================================
	function Initialize() {
		$this->MySQL = @mysql_connect($this->DB_HOST, $this->DB_USERNAME, 'wf36693xI4');
		if (!$this->MySQL) return(false);
		@mysql_select_db($this->DB_NAME, $this->MySQL);
		return(true);
	}

	function Destroy() {
		@mysql_close($this->MySQL);
	}

	# ===============================================
	# Login/Logout
	# ===============================================
	function Login ($username, $password) {
		# Logout User if it's Already In
		if ($this->CheckUpdate($username) === false)
			$this->Logout($username);

		$uid = $this->SqlSelectOneValue("uid", "login", 
										"username=" . SqlQuote($username) . 
										" AND " .
										"password=" . SqlQuote($password));
		if ($uid === false) return(false);

		$ip = RealIP();
		$magic = $this->GenerateMagic($uid, $username, $ip);

		if ($this->SetIpAndMagic($username, $ip, $magic) === false)
			return(false);

		if ($this->SetStatus($username, 0) === false)
			return(false);

		if ($this->Update($username) === false)
			return(false);

		return($magic);
	}

	function Logout ($username) {
		if (($uid = $this->GetUid($username)) === false)
			return(false);
		$this->SetStatus($username, 0);
		return($this->SqlDelete("user_info", "uid=" . SqlQuote($uid)));
	}

	function Authenticate ($username, $magic) {
		if (($this->GetMagic($username) == $magic) ? true : false);
	}

	function Register ($username, $password) {
		if (($uid = $this->GetUid($username)) !== false)
			return(false);
		return($this->SqlInsert("login", "username, password", 
								SqlQuote($username) . ", " . SqlQuote($password)));
	}

	function GenerateMagic ($uid, $username, $ip) {
		$today = date("D M j G:i:s T Y");
		return(md5($today . sha1($uid) . sha1($username) . sha1($ip)));
	}

	# ===============================================
	# Connect/Disconnect & Update
	# ===============================================
	function Connect ($username, $port) {
		if ($this->SetStatus($username, 1) === false)
			return(false);
		if ($this->Update($username) === false)
			return(false);
		return($this->SetPort($username, $port));
	}

	function Disconnect ($username) {
		$this->UnsetPort($username, $port);
		return($this->SetStatus($username, 0));
	}

	function Update ($username) {
		return($this->SetLastUpdateTime($username, time()));
	}

	function CheckUpdate ($username) {
		$update = time() - $this->GetLastUpdate($username);
		return(($update > 300) ? false : true);
	}

	# ===============================================
	# SET/Unset
	# ===============================================
	function SetLastUpdateTime ($username, $time) {
		return($this->SqlUpdateOneValue("login", "last_update=" . SqlQuote($time), 
										"username=" . SqlQuote($username)));
	}

	function SetStatus ($username, $status) {
		return($this->SqlUpdateOneValue("login", "status=" . SqlQuote($status), 
										"username=" . SqlQuote($username)));
	}

	function SetPort ($username, $port) {
		if (($uid = $this->GetUid($username)) === false)
			return(false);
		return($this->SqlUpdateOneValue("user_info", "port=" . SqlQuote($port), 
										"uid=" . SqlQuote($uid)));
	}

	function SetIpAndMagic ($username, $ip, $magic) {
		if (($uid = $this->GetUid($username)) === false)
			return(false);
		return($this->SqlInsert("user_info", "uid, ip, magic", SqlQuote($uid) . 
								"," . SqlQuote($ip) . ", " . SqlQuote($magic)));
	}

	function UnsetPort ($username) {
		return($this->SetPort($username, 0));
	}

	# ===============================================
	# GET
	# ===============================================
	function GetUid ($username) {
		return($this->SqlSelectOneValue("uid", "login", "username=". SqlQuote($username)));
	}

	function GetStatus ($username) {
		return($this->SqlSelectOneValue("status", "login", "username=". SqlQuote($username)));
	}

	function GetLastUpdate ($username) {
		return($this->SqlSelectOneValue("last_update", "login", "username=". SqlQuote($username)));
	}

	function GetIp ($username) {
		if (($uid = $this->GetUid($username)) === false)
			return(false);
		return($this->SqlSelectOneValue("ip", "user_info", "uid=". SqlQuote($username)));
	}

	function GetPort ($username) {
		if (($uid = $this->GetUid($username)) === false)
			return(false);
		return($this->SqlSelectOneValue("port", "user_info", "uid=". SqlQuote($username)));
	}

	function GetMagic ($username) {
		if (($uid = $this->GetUid($username)) === false)
			return(false);
		return($this->SqlSelectOneValue("magic", "user_info", "uid=". SqlQuote($username)));
	}

	# ===============================================
	# PRIVATE SQL Methods
	# ===============================================
	function SqlSelectOneValue ($what, $from, $where) {
		$sql = sprintf("SELECT `%s` FROM %s WHERE %s", $what, $from, $where);
#		echo "<b>" . $sql . "</b><br />";
		if (!($result = mysql_query($sql, $this->MySQL)))
			return(false);

		if (mysql_num_rows($result) == 0)
			return(false);

		if ($row = mysql_fetch_assoc($result))
			return($row[$what]);
		return(false);
	}

	function SqlUpdateOneValue ($table, $set, $where) {
		$sql = sprintf("UPDATE %s SET %s WHERE %s", $table, $set, $where);
		return($this->ExecuteNonQuery($sql));
	}

	function SqlInsert ($table, $what, $values) {
		$sql = sprintf("INSERT INTO %s (%s) VALUES (%s)", $table, $what, $values);
		return($this->ExecuteNonQuery($sql));
	}

	function SqlDelete ($table, $where) {
		$sql = sprintf("DELETE FROM %s WHERE %s", $table, $where);
		return($this->ExecuteNonQuery($sql));
	}

	function SqlDeleteAll ($table) {
		$sql = sprintf("DELETE * FROM %s", $table);
		return($this->ExecuteNonQuery($sql));
	}

	function ExecuteNonQuery ($sql) {
#		echo "<b>" . $sql . "</b><br />";
		if (!($result = mysql_query($sql, $this->MySQL)))
			return(false);
		return(true);
	}
}

/*
$db = new NyFolderDb;
if ($db->Initialize() === true) {
	echo "DB Connected <br />";
#	if ($db->Register("Theo", md5("Prova")) === true)
#		echo "Register OK<br />";		
	if (($magic = $db->Login("Theo", md5("Prova"))) !== false) {
		echo "Login OK - " . $magic . "<br />";

#		if ($db->Connect("Theo") === true)
#			echo "Connected OK<br />";

		if ($db->Disconnect("Theo") === true)
			echo "Disconnected OK<br />";

		$db->Logout("Theo");
	}
	echo mysql_error();
	$db->Destroy();
} else {
	echo "DB Not Connected <br />";
}
*/
?>
